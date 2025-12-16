using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using BookingCare.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Patient.Controllers.Api
{
    [Area("Patient")]
    [Route("Patient/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class BookingHistoryApiController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;

        public BookingHistoryApiController(DataContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailTemplate emailTemplate)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate;
        }

        //Lấy danh sách lịch đặt
        [HttpGet("bookingHistory")]
        public async Task<IActionResult> GetBookingHistory (string? search = "", string filter = "Tất cả", int page = 1, int pageSize = 10)
        {
            //Lấy thông tin bệnh nhân
            var userId = _userManager.GetUserId(User);
            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
            {
                return BadRequest(new { success = false, message = "Không thể lấy thông tin bệnh nhân!" });
            }

            //Lấy danh sách lịch đặt
            var appointments = _dbContext.Appointments
                                .Include(a => a.Doctor)
                                    .ThenInclude(d => d.User)
                                .Include(a => a.Doctor)
                                    .ThenInclude(d => d.Room)
                                .Where(a => a.PatientId == patient.Id);

            //Lọc lịch
            if (filter == "Đã đặt")
            {
                appointments = appointments.Where(a => a.Status == "Chờ khám" || a.Status == "Đang khám");
            } else if(filter == "Đã khám")
            {
                appointments = appointments.Where(a => a.Status == "Hoàn thành");
            } else if(filter == "Đã hủy")
            {
                appointments = appointments.Where(a => a.Status == "Đã hủy");
            }

            //Tìm kiếm theo tên bác sĩ
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                appointments = appointments.Where(a => a.Doctor.User.FullName.Contains(search) || a.ReasonForVisit.Contains(search));
            }

            //Tổng số lịch khám
            var totalAppt = await appointments.CountAsync();

            //Lấy danh sách hiển thị ở trang muốn xem
            var data = await appointments
                            .OrderByDescending(a => a.AppointmentDate)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(a => new BookingHistoryDtos
                            {
                                AppointmentId = a.Id,
                                AppointmentDate = a.AppointmentDate,
                                AppointmentTime = a.AppointmentTime,
                                ReasonForVisit = a.ReasonForVisit,
                                Status = a.Status,
                                DoctorId = a.DoctorId,
                                DoctorName = a.Doctor.User.FullName,
                                RoomId = a.Doctor.RoomId,
                                RoomName = a.Doctor.Room.Name
                            }).ToListAsync();
            return Ok(new { totalAppt, data });
        }

        //Hủy lịch đặt
        [HttpPut("cancelBooking/{appointmentId}")]
        public async Task<IActionResult> CancelAppt(int appointmentId)
        {
            //Lấy thông tin lịch đặt
            var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
            if (appointment == null)
            {
                return BadRequest(new { success = false, message = "Không thể lấy thông tin lịch đặt!" });
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
            if (now >=  startDateTime)
            {
                return BadRequest(new { success = false, message = "Đã đến giờ khám, không thể hủy lịch đặt!" });
            }

            //Hủy lịch
            appointment.Status = "Đã hủy";
            _dbContext.Appointments.Update(appointment);
            await _dbContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Hủy lịch đặt thành công!" });
        }
    }
}
