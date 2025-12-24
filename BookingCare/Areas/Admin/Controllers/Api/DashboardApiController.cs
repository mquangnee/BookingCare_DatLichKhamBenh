using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public DashboardApiController(DataContext dataContext)
        {
            _dbContext = dataContext;
        }

        //=============================================
        // 1. TỔNG QUAN DASHBOARD
        // GET: /api/admin/dashboard/summary
        //=============================================
        [HttpGet("summary")]
        public IActionResult GetSummary()
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
                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        totalDoctors,
                        totalPatients,
                        totalApptToday,
                        totalCanceledApptToday
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.ToString());
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy dữ liệu từ cơ sở dữ liệu!"
                });
            }
        }

        //=============================================
        // 2. BIỂU ĐỒ LỊCH KHÁM THEO NGÀY
        // GET: /api/admin/dashboard/appointments/daily
        //=============================================
        [HttpGet("appointments/daily")]
        public IActionResult GetAppointmentsDaily()
        {
            try
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

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.ToString());
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy dữ liệu từ cơ sở dữ liệu!"
                });
            }
        }

        //=============================================
        // 3. BIỂU ĐỒ LỊCH KHÁM TRẠNG THÁI
        // GET: /api/admin/dashboard/appointments/status
        //=============================================
        [HttpGet("appointments/status")]
        public IActionResult GetAppointmentsStatus()
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
                int successCount = recentAppointments.Count(a => a.Status == "Hoàn thành");
                int canceledCount = recentAppointments.Count(a => a.Status == "Đã hủy");
                var totalCount = recentAppointments.Count();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        waitingCount,
                        successCount,
                        canceledCount,
                        totalCount
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.ToString());
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy dữ liệu từ cơ sở dữ liệu!"
                });
            }
        }

        //=============================================
        // 4. BIỂU ĐỒ LỊCH KHÁM CỦA BÁC SĨ
        // GET: /api/admin/dashboard/appointments/appointments-by-doctor/date
        //=============================================
        [HttpGet("appointments-by-doctor")]
        public async Task<IActionResult> GetAppointmentsByDoctor([FromQuery] DateOnly date)
        {
            var doctors = await _dbContext.Doctors
                                    .Include(d => d.User)
                                    .Select(d => new
                                    {
                                        d.Id,
                                        d.User.FullName,
                                        TotalAppointments = _dbContext.Appointments.Count(a => a.DoctorId == d.Id && a.AppointmentDate == date)
                                    })
                                    .OrderByDescending(d => d.TotalAppointments)
                                    .ToListAsync();
            return Ok(new
            {
                success = true,
                data = doctors
            });
        }
    }
}
