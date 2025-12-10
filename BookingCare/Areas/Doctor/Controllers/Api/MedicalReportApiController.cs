using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using BookingCare.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Doctor.Controllers.Api
{
    [Area("Doctor")]
    [Route("api/doctor/medicalreport")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class MedicalReportApiController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;

        public MedicalReportApiController(DataContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailTemplate emailTemplate)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate;
        }

        //=============================================
        // 1. Lấy danh sách thuốc
        // GET: /api/doctor/medicalreport
        //=============================================
        [HttpGet]
        public async Task<IActionResult> GetMedical()
        {
            var medList = await _dbContext.Medicines.Where(m => m.Status != "Dừng sử dụng").ToListAsync();
            if (medList == null || medList.Count == 0)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tải được dữ liệu thuốc, vui lòng thử lại!"
                });
            }
            return Ok(new
            {
                success = true,
                data = medList
            });
        }

        //=============================================
        // 2. GỬI KẾT QUẢ KHÁM BỆNH
        // POST: /api/doctor/medicalreport
        //=============================================
        [HttpPost]
        public async Task<IActionResult> SendMedicalReport([FromBody] MedicalReportDtos dtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Vui lòng điền đầy đủ thông tin kết quả khám bệnh!"
                });
            }

            // Lấy thông tin lịch hẹn
            var appointment = await _dbContext.Appointments
                                        .Include(a => a.Patient)
                                            .ThenInclude(p => p.User)
                                        .FirstOrDefaultAsync(a => a.Id == dtos.AppointmentId);
            if (appointment == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không thể lấy thông tin lịch hẹn!"
                });
            }

            //Lưu kết quả
            var prescription = new Prescription
            {
                Diagnosis = dtos.Diagnosis,
                Instructions = dtos.Instructions,
                AppointmentId = dtos.AppointmentId
            };
            await _dbContext.Prescriptions.AddAsync(prescription);
            await _dbContext.SaveChangesAsync();
            var prescriptionDetails = new List<Prescription_Detail>();
            foreach (var med in dtos.Medicines)
            {
                var medicine = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Name == med.Name);
                if (medicine == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không thể lấy thông tin thuốc!"
                    });
                }
                var pres_detail = new Prescription_Detail
                {
                    Dosage = med.Dosage,
                    Usage = med.Usage,
                    PrescriptionId = prescription.Id,
                    MedicineId = medicine.Id
                };
                prescriptionDetails.Add(pres_detail);
            }
            await _dbContext.Prescription_Details.AddRangeAsync(prescriptionDetails);
            await _dbContext.SaveChangesAsync();
            
            //Gửi email cho bệnh nhân
            var body = _emailTemplate.GetMedicalReportEmailBody(appointment.Patient.User.FullName, dtos.Diagnosis, dtos.Instructions, dtos.Medicines);
            _ = Task.Run(() =>  _emailSender.SendEmailAsync(appointment.Patient.User.Email, "Kết quả khám bênh - BookingCare", body));
            return Ok(new
            {
                success = true,
                message = "Lưu kết quả khám bệnh thành công!"
            });
        }
    }
}
