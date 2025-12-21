using BookingCare.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Controllers.Api
{
    [ApiController]
    [Route("api/AllSpecialty")]
    public class SpecialitiesController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public SpecialitiesController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllSpecialties()
        {
            var specialties = await _dbContext.Specialties.Select(s => new
            {
                id = s.Id ,
                name = s.Name ,
                imageUrl = s.AvatarUrl,
                description = s.Description,
            }).ToListAsync();
            return Ok(specialties);
        }        
    }
}
