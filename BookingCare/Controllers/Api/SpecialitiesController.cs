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
                imageUrl = s.ImageUrl,
                description = s.Description,
            }).ToListAsync();
            return Ok(specialties);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var specialty = await _dbContext.Specialties
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    imageUrl = s.ImageUrl,
                    description = s.Description
                })
                .FirstOrDefaultAsync();

            if (specialty == null)
                return NotFound();

            var doctors = await _dbContext.Doctors
                .Where(d => d.SpecialtyId == id)
                .Select(d => new
                {
                    id = d.Id,
                    name = d.User != null ? d.User.FullName : "Ẩn danh",
                    imageUrl = d.AvatarUrl,
                    degree = d.Degree,
                    roomId = d.RoomId
                })
                .ToListAsync();

            return Ok(new
            {
                specialty.id,
                specialty.name,
                specialty.imageUrl,
                specialty.description,
                doctors
            });
        }

    }
}
