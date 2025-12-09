using BookingCare.Areas.Admin.Controllers.Api;
using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BookingCare.Areas.Doctors.Controllers.Api
{
    [Area("Doctor")]
    [Route("Doctors/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class AppoimentsMainController : Controller
    {
        private readonly DataContext _dbContext;

        public AppoimentsMainController(DataContext dataContext)
        {
            _dbContext = dataContext;
        }
        [HttpGet("list")]
        public IActionResult GetAppointments()
        {
            var data = _dbContext.Appointments
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .Select(a => new
                {
                    id = a.Id,
                    rawDate = a.AppointmentDate,
                    date = a.AppointmentDate.ToString("dd/MM/yyyy"),
                    time = a.AppointmentTime,
                    patientName = a.Patient != null && a.Patient.User != null
                        ? a.Patient.User.FullName
                        : "Chưa có thông tin",
                    reason = a.ReasonForVisit,
                    status = a.Status
                })
                .OrderBy(a => a.rawDate)
                .Select(a => new
                {
                    a.id,
                    a.date,
                    a.time,
                    a.patientName,
                    a.reason,
                    a.status
                })
                .ToList();

            return Ok(new { success = true, appointments = data });
        }

        [HttpGet("index")]
        public IActionResult GetStats()
        {
            try
            {
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
                return Ok(new { success = true, totalPatients, totalApptToday, totalCanceledApptToday });
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
    }
}
