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
    [Route("api/doctor/appointment-detail")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class AppointmentDetailApiController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentDetailApiController(DataContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //=============================================
        // 1. CHI TIẾT LỊCH KHÁM
        // GET: /api/doctor/appointment-detail/id
        //=============================================
        [HttpGet("{appointmentId}")]
        public async Task<IActionResult> GetAppointmentDetail(int appointmentId)
        {
            // Lấy thông tin lịch khám và bệnh nhân
            var appointment = await _dbContext.Appointments
                                .Include(a => a.Patient)
                                    .ThenInclude(p => p.User)
                                .FirstOrDefaultAsync(a => a.Id == appointmentId);
            if (appointment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Lịch khám không tồn tại."
                });
            }
            var detail = new AppointmentScheduleDetailDtos
            {
                AppointmentId = appointment.Id,
                Status = appointment.Status,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                ReasonForVisit = appointment.ReasonForVisit,
                PatientId = appointment.Patient.Id,
                PatientName = appointment.Patient.User.FullName,
                DateOfBirth = appointment.Patient.User.DateOfBirth,
                Gender = appointment.Patient.User.Gender,
                MedicalHistory = appointment.Patient.MedicalHistory
            };
            return Ok(new
            {
                success = true,
                data = detail
            });
        }
    }
}
