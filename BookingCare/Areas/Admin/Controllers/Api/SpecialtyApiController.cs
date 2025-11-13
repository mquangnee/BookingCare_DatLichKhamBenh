using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("Admin/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SpecialtyApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public SpecialtyApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetSpecialties()
        {
            var listSpecialties = await _dbContext.Specialties.Select(s => new { s.Id, s.Name }).ToListAsync();
            return Ok(listSpecialties);
        }
    }
}
