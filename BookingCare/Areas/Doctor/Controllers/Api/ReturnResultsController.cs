using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingCare.DTOs;

namespace BookingCare.Areas.Doctor.Controllers.Api   // ✅ Doctor (số ít)
{
    [Area("Doctor")]                                 // ✅ Doctor
    [Route("Doctor/api/[controller]")]               // ✅ Doctor/api
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class ReturnResultsController : Controller
    {
        private readonly DataContext _dbContext;

        public ReturnResultsController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("submit-result/{appointmentId}")]
        public IActionResult SubmitResult(int appointmentId, [FromBody] ExamResultRequest model)
        {
            var appt = _dbContext.Appointments.FirstOrDefault(a => a.Id == appointmentId);
            if (appt == null)
                return NotFound(new { success = false, message = "Không tìm thấy lịch khám" });

            var prescription = new Prescription
            {
                AppointmentId = appointmentId,
                Diagnosis = model.Diagnosis,
                CreatedAt = DateTime.Now,
                Prescription_Details = new List<Prescription_Detail>()
            };

            foreach (var item in model.Medicines)
            {
                var detail = new Prescription_Detail
                {
                    Quantity = item.Quantity,
                    Dosage = item.Dosage,
                    Instructions = item.Instructions,
                    MedicineId = item.MedicineId
                };

                prescription.Prescription_Details.Add(detail);
            }

            _dbContext.Prescriptions.Add(prescription);
            appt.Status = "Đã khám";

            _dbContext.SaveChanges();

            return Ok(new { success = true });
        }
        [HttpGet("get-medicines")]
        public IActionResult GetMedicines()
        {
            var medicines = _dbContext.Medicines
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Unit
                })
                .ToList();

            return Ok(medicines);
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
