using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Doctor.Controllers.Api
{
    [Area("Doctor")]
    [Route("api/doctor/home")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class HomeApiController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeApiController(DataContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //=============================================
        // 1. LẤY DANH SÁCH LỊCH ĐẶT KHÁM CỦA BÁC SĨ
        // GET: /api/doctor/home
        //=============================================
        [HttpGet]
        public async Task<IActionResult> GetAppointmentSchedule(string? date = "", string? search = "", string filter = "Tất cả", int page = 1, int pageSize = 10)
        {
            //Lấy thông tin bác sĩ
            var userId = _userManager.GetUserId(User);
            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không thể lấy thông tin bác sĩ!"
                });
            }

            //Mặc định hiển thị lịch khám ngày hôm nay
            DateOnly dateSchedule;
            if (string.IsNullOrEmpty(date))
            {
                dateSchedule = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                dateSchedule = DateOnly.Parse(date);
            }

            //Lấy danh sách lịch đặt
            var appointments = _dbContext.Appointments
                                .Include(a => a.Patient)
                                    .ThenInclude(d => d.User)
                                .Where(a => a.DoctorId == doctor.Id && a.AppointmentDate == dateSchedule);

            //Lọc lịch
            if (filter == "Chờ khám")
            {
                appointments = appointments.Where(a => a.Status == "Chờ khám" || a.Status == "Đang khám");
            }
            else if (filter == "Đã khám")
            {
                appointments = appointments.Where(a => a.Status == "Hoàn thành");
            }
            else if (filter == "Đã hủy")
            {
                appointments = appointments.Where(a => a.Status == "Đã hủy");
            }

            //Tìm kiếm theo tên bệnh nhân hoặc lý do khám
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                appointments = appointments.Where(a => a.Patient.User.FullName.Contains(search) || a.ReasonForVisit.Contains(search));
            }

            //Tổng số lịch khám
            var totalAppointments = await appointments.CountAsync();

            //Lấy danh sách hiển thị ở trang muốn xem
            var listAppointments = appointments
                        .AsEnumerable() // Chuyển sang chạy trên bộ nhớ
                        .OrderByDescending(a => a.AppointmentDate)
                        .ThenBy(a =>
                            TimeSpan.Parse(a.AppointmentTime.Split('-')[0].Trim()))
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(a => new AppointmentScheduleDtos
                        {
                            AppointmentId = a.Id,
                            AppointmentDate = a.AppointmentDate,
                            AppointmentTime = a.AppointmentTime,
                            ReasonForVisit = a.ReasonForVisit,
                            Status = a.Status,
                            PatientId = a.PatientId,
                            PatientName = a.Patient.User.FullName
                        })
                        .ToList();
            return Ok(new
            {
                success = true,
                data = new
                {
                    totalAppointments,
                    listAppointments
                }
            });
        }

        //=============================================
        // 2. HỦY LỊCH ĐẶT KHÁM
        // PUT: /api/doctor/home/id
        //=============================================
        [HttpPut("{appointmentId}")]
        public async Task<IActionResult> CancelAppt(int appointmentId)
        {
            //Lấy thông tin lịch đặt
            var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
            if (appointment == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Không thể lấy thông tin lịch đặt!"
                });
            }

            //Lấy ngày, giờ thời điểm hủy => so sánh với thời gian đặt => nếu chưa đến thời gian đặt => cho phép hủy
            var now = DateTime.Now;

            //Lấy thông tin ngày, giờ đặt
            //1. Ngày đặt
            var date = appointment.AppointmentDate;
            //2. Giờ đặt
            var parts = appointment.AppointmentTime.Split('-');
            var startTime = TimeOnly.Parse(parts[0]);

            //Tạo DateTime để so sánh
            var startDateTime = date.ToDateTime(startTime);
            if (now >= startDateTime)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Đã đến giờ khám, không thể hủy lịch đặt!"
                });
            }

            //Hủy lịch
            appointment.Status = "Đã hủy";
            appointment.UpdatedAt = DateTime.Now;
            _dbContext.Appointments.Update(appointment);
            await _dbContext.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Hủy lịch đặt thành công!"
            });
        }
    }
}
