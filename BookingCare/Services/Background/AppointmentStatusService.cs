using BookingCare.Repository;
using BookingCare.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Services.Background
{
    public class AppointmentStatusService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<AppointmentHub> _appointmentHubContext;

        public AppointmentStatusService(IServiceProvider serviceProvider, IHubContext<AppointmentHub> appointmentHubContext)
        {
            _serviceProvider = serviceProvider;
            _appointmentHubContext = appointmentHubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) //Chạy cho đến khi ứng dụng đóng
            {
                await UpdateAppontmentStatus();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); //1 phút chạy 1 lần
            }
        }

        //Cập nhật trạng thái lịch hẹn
        private async Task UpdateAppontmentStatus()
        {
            using var scope = _serviceProvider.CreateScope(); //Tạo scope mới
            var db = scope.ServiceProvider.GetRequiredService<DataContext>(); //Lấy service datacontext

            //Lấy thời gian hiện tại
            var now = DateTime.Now;

            //Lấy các lịch ở trạng thái "Chờ khám" hoặc "Đang khám"
            var apppointments = await db.Appointments.Where(a => a.AppointmentDate == DateOnly.FromDateTime(now) && (a.Status == "Chờ khám" || a.Status == "Đang khám")).ToListAsync();

            foreach(var appt in apppointments)
            {
                var parts = appt.AppointmentTime.Split("-");
                var startTime = TimeOnly.ParseExact(parts[0], "HH:mm"); //Lấy giờ bắt đầu lịch khám
                var endTime = TimeOnly.ParseExact(parts[1], "HH:mm"); //Lấy giờ kết thúc lịch khám

                var start = appt.AppointmentDate.ToDateTime(startTime); //Thời gian bắt đầu lịch khám (ngày + giờ)
                var end = appt.AppointmentDate.ToDateTime(endTime); //Thời gian kết thúc lịch khám (ngày + giờ)

                if (now < start)
                {
                    continue; //Chưa đến giờ khám
                }

                //Cập nhật trạng thái lịch khám
                if (appt.Status == "Chờ khám" && (now > start || now < end))
                {
                    appt.Status = "Đang khám";
                    await _appointmentHubContext.Clients.All.SendAsync("StatusChanged", appt.Id, "Đang khám"); //Gửi về client với trạng thái mới
                }
                if (appt.Status == "Đang khám" && now > end)
                {
                    appt.Status = "Hoàn thành";
                    await _appointmentHubContext.Clients.All.SendAsync("StatusChanged", appt.Id, "Hoàn thành"); //Gửi về client với trạng thái mới
                }
            }
            //Lưu xuống db
            await db.SaveChangesAsync();
        }
    }
}
