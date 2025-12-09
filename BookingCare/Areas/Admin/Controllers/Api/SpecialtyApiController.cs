using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/specialties")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SpecialtyApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public SpecialtyApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        //=============================================
        // 1. Lấy danh sách chuyên khoa
        // GET: /api/admin/specialties
        //=============================================
        [HttpGet]
        public async Task<IActionResult> GetSpecialties()
        {
            var listSpecialties = await _dbContext.Specialties.Select(s => new { s.Id, s.Name }).ToListAsync();
            return Ok(new
            {
                success = true,
                data = listSpecialties
            });
        }
    }
}
