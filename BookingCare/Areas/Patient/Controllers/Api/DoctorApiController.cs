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
    public class DoctorApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public DoctorApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Lấy danh sách bác sĩ theo chuyên khoa
        [HttpGet("getAll/{specialtyId}")]
        public async Task<IActionResult> GetDoctorBySpecialty(int specialtyId)
        {
            var listDoctors = await _dbContext.Users
                                .Include(u => u.Doctor)
                                .Where(u => u.Doctor.SpecialtyId == specialtyId)
                                .Select(u => new { u.Doctor.Id, u.FullName }).ToListAsync();
            return Ok(listDoctors);
        }
    }
}
