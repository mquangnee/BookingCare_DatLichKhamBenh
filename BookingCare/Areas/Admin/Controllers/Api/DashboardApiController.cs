using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("Admin/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public DashboardApiController(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        //====HIỂN THỊ SỐ BÁC SĨ, SỐ BỆNH NHÂN, SỐ LỊCH KHÁM ĐÃ ĐẶT TRONG HÔM NAY, SỐ LỊCH KHÁM ĐÃ HỦY====//
        [HttpGet("index")]
        public IActionResult GetStats()
        {
            try
            {
                //Số bác sĩ đang làm việc ở phòng khám
                var totalDoctors = (from u in _dbContext.Users
                                    join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                                    join r in _dbContext.Roles on ur.RoleId equals r.Id
                                    where r.Name == "Doctor" && u.LockoutEnd == null
                                    select u).Count();

                //Số bệnh nhân đã đăng ký
                var totalPatients = (from u in _dbContext.Users
                                     join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                                     join r in _dbContext.Roles on ur.RoleId equals r.Id
                                     where r.Name == "Patient"
                                     select u).Count();

                var today = DateOnly.FromDateTime(DateTime.Now);

                //Số lịch khám trong hôm nay 
                var totalApptToday = _dbContext.Appointments.Count(a => a.AppointmentDate == today);

                //Số lịch khám trong hôm nay đã hủy
                var totalCanceledApptToday = _dbContext.Appointments.Count(a => a.AppointmentDate == today && a.Status == "Đã hủy");
                return Ok(new { success = true, totalDoctors, totalPatients, totalApptToday, totalCanceledApptToday });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.ToString());
                return Ok(new
                {
                    success = false,
                    message = "Lỗi khi lấy dữ liệu từ cơ sở dữ liệu!",
                    totalDoctors = 0,
                    totalPatients = 0,
                    totalApptToday = 0,
                    totalCanceledApptToday = 0
                });
            }
        }

        //====BIỂU ĐỒ THỐNG KÊ SỐ LỊCH ĐẶT 10 NGÀY GẦN NHẤT====//
        [HttpGet("booking-last-10days-1")]
        public IActionResult GetBookingStatsLast10Days()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var startDate = today.AddDays(-9); // lấy 10 ngày gần nhất (bao gồm hôm nay)

            var stats = _dbContext.Appointments
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= today)
                .GroupBy(a => a.AppointmentDate)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToList();

            // Đảm bảo có đủ 10 ngày (nếu ngày nào không có thì thêm 0)
            var result = Enumerable.Range(0, 10)
                .Select(i => startDate.AddDays(i))
                .Select(date => new
                {
                    Date = date.ToString("dd/MM"),
                    Total = stats.FirstOrDefault(s => s.Date == date)?.Total ?? 0
                });

            return Ok(result);
        }
        [HttpGet("booking-last-10days-2")]
        public IActionResult GetAppointmentStatusStats()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);
                var tenDaysAgo = today.AddDays(-9); // tính cả hôm nay = 10 ngày

                // Lọc các lịch khám trong 10 ngày gần nhất
                var recentAppointments = _dbContext.Appointments
                    .Where(a => a.AppointmentDate >= tenDaysAgo && a.AppointmentDate <= today)
                    .ToList();

                // Đếm theo trạng thái
                int waitingCount = recentAppointments.Count(a => a.Status == "Chờ khám");
                int successCount = recentAppointments.Count(a => a.Status == "Đã khám");
                int canceledCount = recentAppointments.Count(a => a.Status == "Đã hủy");
                var totalCount = recentAppointments.Count();

                return Ok(new
                {
                    waitingCount,
                    successCount,
                    canceledCount,
                    totalCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dashboard Error] {ex.Message}");
                return Ok(new
                {
                    waitingCount = 0,
                    successCount = 0,
                    canceledCount = 0,
                    success = false,
                    message = "Lỗi khi lấy thống kê lịch khám."
                });
            }
        }
    }
}
