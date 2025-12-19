using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Patient.Controllers.Api
{
    [Area("Patient")]
    [Route("Patient/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class AppointmentApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public AppointmentApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("morningShift/{doctorId}/{bookingDate}")]
        public async Task<IActionResult> CheckTimeSlotMorning (int doctorId, DateOnly bookingDate)
        {
            var timeSlot = new List<string> { "07:00-07:30", "07:30-08:00", "08:00-08:30", "08:30-09:00", "09:00-09:30", "09:30-10:00", "10:00-10:30", "10:30-11:00" };
            
            //Danh sách kiểm tra
            var result = new List<object> ();

            foreach (var time in timeSlot)
            {
                var count = await _dbContext.Appointments.Where(a => a.DoctorId == doctorId && a.AppointmentDate == bookingDate && a.AppointmentTime == time).CountAsync();
                
                bool check = false;
                if (count < 3)
                {
                    check = true;
                }

                result.Add(new
                {
                    timeSlot = time,
                    check
                });
            }
            return Ok(result);
        }

        [HttpGet("eveningShift/{doctorId}/{bookingDate}")]
        public async Task<IActionResult> CheckTimeSlotEvening(int doctorId, DateOnly bookingDate)
        {
            var timeSlot = new List<string> { "13:00-13:30", "13:30-14:00", "14:00-14:30", "14:30-15:00", "15:00-15:30", "15:30-16:00", "16:00-16:30", "16:30-17:00" };

            //Danh sách kiểm tra
            var result = new List<object>();

            foreach (var time in timeSlot)
            {
                var count = await _dbContext.Appointments.Where(a => a.DoctorId == doctorId && a.AppointmentDate == bookingDate && a.AppointmentTime == time).CountAsync();

                bool check = false;
                if (count < 3)
                {
                    check = true;
                }

                result.Add(new
                {
                    timeSlot = time,
                    check
                });
            }
            return Ok(result);
        }
    }
}
