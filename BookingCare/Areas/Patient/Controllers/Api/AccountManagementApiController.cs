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
    public class AccountManagementApiController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;

        public AccountManagementApiController(DataContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailTemplate emailTemplate)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate;
        }

        //Hiển thị thông tin bệnh nhân
        [HttpGet("accountManagement")]
        public async Task<IActionResult> GetPatientInfo()
        {
            //Lấy thông bệnh nhân
            var userId = _userManager.GetUserId(User);
            var patient = await _dbContext.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy thông tin bệnh nhân!" });
            }

            //Tạo đối tượng DTO để trả về
            var inforPatient = new PatientAccountManagementDtos
            {
                FullName = patient.User.FullName,
                Gender = patient.User.Gender,
                DateOfBirth = patient.User.DateOfBirth,
                Address = patient.User.Address,
                PhoneNumber = patient.User.PhoneNumber,
                MedicalHistory = patient.MedicalHistory
            };
            return Ok(inforPatient);
        }

        //Cập nhật thông tin bệnh nhân
        [HttpPut("updateInfor")]
        public async Task<IActionResult> UpdatePatientInfo([FromBody] PatientAccountManagementDtos dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Vui lòng điền đầy đủ thông tin!" });
            }

            //Lấy thông tin bệnh nhân
            var userId = _userManager.GetUserId(User);
            var patient = await _dbContext.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy thông tin bệnh nhân!" });
            }

            //Cập nhật thông tin bệnh nhân
            patient.User.FullName = dto.FullName;
            patient.User.Gender = dto.Gender;
            patient.User.DateOfBirth = dto.DateOfBirth;
            patient.User.Address = dto.Address;
            patient.User.PhoneNumber = dto.PhoneNumber;
            patient.MedicalHistory = dto.MedicalHistory;
            await _dbContext.SaveChangesAsync();

            //Gửi email thông báo cập nhật thông tin thành công
            var body = _emailTemplate.GetPatientUpdatedInfoEmailBody(patient.User.FullName, patient.User.Email);
            _ = Task.Run(() => _emailSender.SendEmailAsync(patient.User.Email, "Cập nhật thông tin tài khoản thành công", body));
            return Ok(new { success = true, message = "Cập nhật thông tin thành công!" });
        }
    }
}