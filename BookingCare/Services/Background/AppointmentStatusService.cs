using BookingCare.Repository;
using BookingCare.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace BookingCare.Services.Background
{
    public class AppointmentStatusService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<AppointmentHub> _hub;

        public AppointmentStatusService(IServiceProvider serviceProvider, IHubContext<AppointmentHub> hub)
        {
            _serviceProvider = serviceProvider;
            _hub = hub;
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
            var apppointments = await db.Appointments
                                .Where(a =>
                                    a.AppointmentDate <= DateOnly.FromDateTime(now) &&
                                    (a.Status == "Chờ khám" || a.Status == "Đang khám"))
                                .ToListAsync();

            foreach (var appt in apppointments)
            {
                var parts = appt.AppointmentTime.Split("-");
                var startTime = TimeOnly.ParseExact(parts[0], "HH:mm");
                var endTime = TimeOnly.ParseExact(parts[1], "HH:mm");

                var start = appt.AppointmentDate.ToDateTime(startTime);
                var end = appt.AppointmentDate.ToDateTime(endTime);

                if (now < start)
                    continue;

                if (now >= start && now < end)
                {
                    if (appt.Status != "Đang khám")
                    {
                        appt.Status = "Đang khám";
                        await _hub.Clients.All.SendAsync("StatusChanged", appt.Id, "Đang khám");
                    }
                }
                else if (now >= end)
                {
                    if (appt.Status != "Hoàn thành")
                    {
                        appt.Status = "Hoàn thành";
                        await _hub.Clients.All.SendAsync("StatusChanged", appt.Id, "Hoàn thành");
                    }
                }
            }

            //Lưu xuống db
            await db.SaveChangesAsync();
        }
    }
}
