using BookingCare.Models.DTOs;
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
    public class UserManagementApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public UserManagementApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        //====QUẢN LÝ TÀI KHOẢN BÁC SĨ====//
        [HttpGet("doctor")]
        public async Task<IActionResult> GetDoctors(int page = 1, int pageSize = 10)
        {
            //Lấy danh sách người dùng có vai trò là bác sĩ
            var doctors = _dbContext.Users
                            .Include(u => u.Doctor)
                            .Where(u => u.Doctor != null);

            //Tổng số bác sĩ
            var totalDoctors = await doctors.CountAsync();

            //Lấy danh sách hiển thị ở trang muốn xem
            var data = await doctors
                            .OrderByDescending(d => d.Doctor.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(u => new UserDtos
                            {
                                Id = u.Doctor.Id,
                                FullName = u.FullName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                CreatedAt = u.CreatedAt,
                                UpdatedAt = u.UpdatedAt
                            })
                            .ToListAsync();

            return Ok(new { totalDoctors, data });
        }

        //====QUẢN LÝ TÀI KHOẢN BỆNH NHÂN====//
        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients (int page = 1, int pageSize = 10)
        {
            //Lấy danh sách người dùng có vai trò là bác sĩ
            var patients = _dbContext.Users
                            .Include(u => u.Patient)
                            .Where(u => u.Patient != null);

            //Tổng số bác sĩ
            var totalPatients = await patients.CountAsync();

            //Lấy danh sách hiển thị ở trang muốn xem
            var data = await patients
                            .OrderByDescending(d => d.Patient.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(u => new UserDtos
                            {
                                Id = u.Patient.Id,
                                FullName = u.FullName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                CreatedAt = u.CreatedAt,
                                UpdatedAt = u.UpdatedAt
                            })
                            .ToListAsync();

            return Ok(new { totalPatients, data });
        }
    }
}
