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
    public class PatientBookingApiController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;

        public PatientBookingApiController(DataContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailTemplate emailTemplate)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate;
        }

        //====ĐẶT LỊCH KHÁM BỆNH====//
        [HttpPost("booking")]
        public async Task<IActionResult> BookMedicalAppt([FromBody] PatientBookingDtos booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Vui lòng điền đầy đủ thông tin đặt lịch!" });
            }

            //Kiểm tra lại số lịch hẹn trong ca khám đã chọn
            var count = await _dbContext.Appointments.Where(a => a.DoctorId == booking.DoctorId && a.AppointmentDate == booking.AppointmentDate && a.AppointmentTime == booking.AppointmentTime).CountAsync();
            if (count == 3)
            {
                return BadRequest(new { success = false, message = "Ca khám đã đầy! Vui lòng chọn ca khám khác!" });
            }

            //Lấy thông tin bệnh nhân, bác sĩ
            var userId = _userManager.GetUserId(User);
            var patient = await _dbContext.Patients
                            .Include(p => p.User)
                            .FirstOrDefaultAsync(p => p.UserId == userId);
            var doctor = await _dbContext.Doctors
                            .Include(d => d.User)
                            .Include(d => d.Specialty)
                            .Include(d => d.Room)
                            .FirstOrDefaultAsync(d => d.Id == booking.DoctorId);
            if(patient == null || doctor == null)
            {
                return BadRequest(new { success = false, message = "Không tìm được thông tin bác sĩ, bệnh nhân!" });
            }
            //Kiểm tra bệnh nhân có lịch đặt cùng thời điểm không
            bool checkAppt = await _dbContext.Appointments.AnyAsync(a =>
                                                        a.PatientId == patient.Id &&
                                                        a.AppointmentDate == booking.AppointmentDate &&
                                                        a.AppointmentTime == booking.AppointmentTime &&
                                                        a.Status != "Đã hủy");
            if (checkAppt)
            {
                return BadRequest(new { success = false, message = "Bạn đã có lịch khám vào thời điểm này!" });
            }

            //Thêm lịch hẹn
            var newAppt = new Appointment
            {
                AppointmentDate = booking.AppointmentDate,
                AppointmentTime = booking.AppointmentTime,
                ReasonForVisit = booking.ReasonForVisit,
                Status = "Chờ khám",
                UpdatedAt = null,
                PatientId = patient.Id,
                DoctorId = booking.DoctorId
            };
            await _dbContext.Appointments.AddAsync(newAppt);
            await _dbContext.SaveChangesAsync();

            //Gửi email xác nhận
            var body = _emailTemplate.getBookingSuccessEmailBody(patient.User.FullName, doctor.User.FullName, doctor.Specialty.Name, booking.AppointmentDate, booking.AppointmentTime, doctor.Room.Name, newAppt.Id);
            _ = Task.Run(() => _emailSender.SendEmailAsync(patient.User.Email, "Xác nhận lịch đặt - BookingCare", body));
            return Ok(new { success = true, message = "Đặt lịch khám bệnh thành công!" });
        }
    }
}
