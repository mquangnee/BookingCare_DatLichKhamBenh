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
    public class SpecialtyApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public SpecialtyApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Lấy danh sách chuyên khoa
        [HttpGet("getAll")]
        public async Task<IActionResult> GetSpecialties()
        {
            var listSpecialties = await _dbContext.Specialties.Select(s => new { s.Id, s.Name }).ToListAsync();
            return Ok(listSpecialties);
        }
 

    }
}
