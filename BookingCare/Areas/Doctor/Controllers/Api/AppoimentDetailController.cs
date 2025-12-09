using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Doctors.Controllers.Api
{
    [Area("Doctors")]
    [Route("Doctors/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class AppoimentDetailController : Controller
    {
        private readonly DataContext _dbContext;

        public AppoimentDetailController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("detail/{id}")]
        public IActionResult GetDetail(int id)
        {
            var appt = _dbContext.Appointments
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .FirstOrDefault(a => a.Id == id);

            if (appt == null)
                return NotFound(new { success = false });

            var data = new
            {
                patientName = appt.Patient.User.FullName,
                dob = appt.Patient.User.DateOfBirth.ToString("dd/MM/yyyy"),
                gender = appt.Patient.User.Gender,  
                phone = appt.Patient.User.PhoneNumber,
                address = appt.Patient.User.Address,
                date = appt.AppointmentDate.ToString("dd/MM/yyyy"),
                time = appt.AppointmentTime,
                reason = appt.ReasonForVisit,
                status = appt.Status
            };


            return Ok(new { success = true, data });
        }
    }

}
