using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/rooms")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoomApiController : ControllerBase
    {
        private readonly DataContext _dbContext;
        
        public RoomApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        //=============================================
        // 1. Lấy danh sách phòng khám
        // GET: /api/admin/rooms
        //=============================================
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var listRooms = await _dbContext.Rooms.Select(r => new { r.Id, r.Name, CurrentDoctorCount = r.Doctors.Count() }).ToListAsync();
            return Ok(new
            {
                success = true,
                data = listRooms
            });
        }
    }
}
