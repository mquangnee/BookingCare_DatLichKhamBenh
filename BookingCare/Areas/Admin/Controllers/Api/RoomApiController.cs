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
    public class RoomApiController : ControllerBase
    {
        private readonly DataContext _dbContext;
        
        public RoomApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetRooms()
        {
            var listRooms = await _dbContext.Rooms.Select(r => new { r.Id, r.Name, CurrentDoctorCount = r.Doctors.Count() }).ToListAsync();
            return Ok(listRooms);
        }
    }
}
