using BookingCare.Repository;
using BookingCare.Services.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Services.Background
{
    public class AppointmentNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AppointmentNotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested) //Chạy cho đến khi ứng dụng đóng
            {
                var now = DateTime.Now;
                //Chạy vào 6h sáng mỗi ngày
                if (now.Hour == 6 && now.Minute == 0)
                {
                    await AppointmentNotificationToday(stoppingToken);
                }
                await SendAppointmentReminder(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        //Gửi email thông báo danh sách lịch khám trong ngày
        private async Task AppointmentNotificationToday(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope(); //Tạo scope mới
            var db = scope.ServiceProvider.GetRequiredService<DataContext>(); //Lấy service datacontext
            var emailTemplate = scope.ServiceProvider.GetRequiredService<IEmailTemplate>(); //Lấy service email template
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>(); //Lấy service email sender

            //Danh sách người dùng
            var listPatient = await db.Patients.Include(p => p.User).ToListAsync();
            if (listPatient == null || listPatient.Count == 0)
            {
                return; //Không có bệnh nhân nào
            }

            //Thoát nếu có yêu cầu hủy
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            var today = DateOnly.FromDateTime(DateTime.Now);

            //Gửi thông báo lịch hẹn sắp tới
            foreach (var patient in listPatient)
            {
                //Danh sách lịch hẹn của bệnh nhân
                var appointmentsHtml = "<ul>";

                //Lấy danh sách lịch hẹn của bệnh nhân trong ngày hôm nay
                var patientAppointments = await db.Appointments.Where(a => a.PatientId == patient.Id && a.AppointmentDate == today).ToListAsync();
                if (patientAppointments == null || patientAppointments.Count == 0)
                {
                    continue; //Bệnh nhân không có lịch hẹn trong ngày
                }
                patientAppointments = patientAppointments.OrderBy(a => TimeOnly.Parse(a.AppointmentTime.Split('-')[0])).ToList();

                foreach (var appt in patientAppointments)
                {
                    //Lấy thông tin bác sĩ và phòng khám
                    var doctor = await db.Doctors
                                    .Include(d => d.Room)
                                    .Include(d => d.User)
                                    .FirstOrDefaultAsync(d => d.Id == appt.DoctorId);
                    appointmentsHtml += $"<li>{appt.AppointmentTime}: Khám với BS. {doctor.User.FullName} tại phòng {doctor.Room.Name}</li>";
                }
                appointmentsHtml += "</ul>";

                //Gửi email thông báo
                var body = emailTemplate.GetDailyAppointmentSummaryEmailBody(patient.User.FullName, appointmentsHtml);
                _ = Task.Run(() => emailSender.SendEmailAsync(patient.User.Email, "Thông báo lịch hẹn khám trong ngày", body));
                await Task.Delay(500, stoppingToken); //Delay 0.5 giây để tránh gửi email quá nhanh => spam
            }
        }

        //Gửi email lịch khám trước 30p
        private async Task SendAppointmentReminder(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope(); //Tạo scope mới
            var db = scope.ServiceProvider.GetRequiredService<DataContext>(); //Lấy service datacontext
            var emailTemplate = scope.ServiceProvider.GetRequiredService<IEmailTemplate>(); //Lấy service email template
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>(); //Lấy service email sender

            //Danh sách người dùng
            var listPatient = await db.Patients.Include(p => p.User).ToListAsync();
            if (listPatient == null || listPatient.Count == 0)
            {
                return; //Không có bệnh nhân nào
            }

            //Thoát nếu có yêu cầu hủy
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            //Lấy ngày, giờ hiện tại
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);

            //Gửi thông báo lịch hẹn sắp tới
            foreach (var patient in listPatient)
            {
                //Lấy danh sách lịch hẹn của bệnh nhân trong ngày hôm nay
                var patientAppointments = await db.Appointments.Where(a => a.PatientId == patient.Id && a.AppointmentDate == today).ToListAsync();
                if (patientAppointments == null || patientAppointments.Count == 0)
                {
                    continue; //Bệnh nhân không có lịch hẹn trong ngày
                }

                foreach (var appt in patientAppointments)
                {
                    //Lấy giờ bắt đầu
                    var parts = appt.AppointmentTime.Split('-');
                    var startTime = TimeOnly.Parse(parts[0]);

                    //Ghép lại thành ngày giờ đầy đủ
                    var appointmentDateTime = appt.AppointmentDate.ToDateTime(startTime);

                    //Tính khoảng thời gian đến lịch khám, nếu 29-30p => gửi mail thông báo
                    var diff = appointmentDateTime - now;
                    if (diff.TotalMinutes >= 29 && diff.TotalMinutes <= 31)
                    {
                        //Lấy thông tin bác sĩ và phòng khám
                        var doctor = await db.Doctors
                                    .Include(d => d.Room)
                                    .Include(d => d.User)
                                    .FirstOrDefaultAsync(d => d.Id == appt.DoctorId);

                        //Gửi email thông báo
                        var body = emailTemplate.GetAppointmentReminderEmailBody(patient.User.FullName, appt.AppointmentTime, doctor.User.FullName, doctor.Room.Name);
                        _ = Task.Run(() => emailSender.SendEmailAsync(patient.User.Email, "Thông báo lịch hẹn sắp tới", body));
                        await Task.Delay(500, stoppingToken); //Delay 0.5 giây để tránh gửi email quá nhanh => spam
                    }
                }
            }
        }
    }
}