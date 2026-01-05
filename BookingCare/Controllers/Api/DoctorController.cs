using BookingCare.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Controllers.Api
{
    [ApiController]
    [Route("api/AllDoctor")]
    public class DoctorController : ControllerBase
    {

        private readonly DataContext _dbContext;
        public DoctorController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await (
             from d in _dbContext.Doctors
             join u in _dbContext.Users on d.UserId equals u.Id
             join s in _dbContext.Specialties on d.SpecialtyId equals s.Id
             select new
             {
                 id = d.Id,
                 name = u.FullName,
                 imageUrl = d.AvatarUrl,
                 specialty = s.Name
             }
         ).ToListAsync();

            return Ok(doctors);

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorDetail(int id)
        {
            var doctor = await (
                    from d in _dbContext.Doctors
                    join u in _dbContext.Users on d.UserId equals u.Id
                    join s in _dbContext.Specialties on d.SpecialtyId equals s.Id
                    join r in _dbContext.Rooms on d.RoomId equals r.Id
                    where d.Id == id
                    select new
                    {
                        id = d.Id,
                        name = u.FullName,
                        imageUrl = d.AvatarUrl,
                        degree = d.Degree,
                        specialty = s.Name,
                        roomName = d.Room.Name,
                        YearOfExp = d.YearsOfExp
                     
                    }
                ).FirstOrDefaultAsync();

            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }
    }

    }

