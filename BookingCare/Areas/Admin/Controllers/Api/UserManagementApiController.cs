using BookingCare.Models;
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
        //Lấy danh sách bác sĩ để phân trang
        [HttpGet("doctors")]
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
                            .OrderBy(d => d.Doctor.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(u => new UserDtos
                            {
                                UserId = u.Id,
                                Id = u.Doctor.Id,
                                FullName = u.FullName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                CreatedAt = u.CreatedAt,
                                UpdatedAt = u.UpdatedAt,
                                IsLocked = u.LockoutEnd
                            })
                            .ToListAsync();

            return Ok(new { totalDoctors, data });
        }

        //Lấy thông tin chi tiết bác sĩ
        [HttpGet("infoDoctor")]
        public IActionResult DoctorDetails(string id)
        {
            var doctor = _dbContext.Users
                .Include(u => u.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(u => u.Doctor)
                    .ThenInclude(d => d.Room)
                .FirstOrDefault(u => u.Id == id);

            if (doctor == null)
            {
                return NotFound(new { message = "Không tìm thấy bác sĩ!" });
            }

            // Gói dữ liệu cần thiết (DTO)
            var result = new DoctorInfoDtos
            {
                UserId = doctor.Id,
                FullName = doctor.FullName,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                Address = doctor.Address,
                Doctors = new Doctor
                {
                    Id = doctor.Doctor.Id,
                    Degree = doctor.Doctor.Degree,
                    YearsOfExp = doctor.Doctor.YearsOfExp
                },
                Specialties = new Specialty
                {
                    Name = doctor.Doctor.Specialty.Name
                },
                Rooms = new Room
                {
                    Name = doctor.Doctor.Room.Name
                }
            };
            
            return Ok(new { result });
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
                            .OrderBy(d => d.Patient.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(u => new UserDtos
                            {
                                UserId = u.Id,
                                Id = u.Patient.Id,
                                FullName = u.FullName,
                                Email = u.Email,
                                PhoneNumber = u.PhoneNumber,
                                CreatedAt = u.CreatedAt,
                                UpdatedAt = u.UpdatedAt,
                                IsLocked = u.LockoutEnd
                            })
                            .ToListAsync();

            return Ok(new { totalPatients, data });
        }

        //Lấy thông tin chi tiết bệnh nhân
        [HttpGet("infoPatient")]
        public IActionResult PatientDetails(string id)
        {
            var patient = _dbContext.Users
                        .Include(u => u.Patient)
                        .FirstOrDefault(u => u.Id == id);

            if (patient == null)
            {
                return NotFound(new { message = "Không tìm thấy bệnh nhân!" });
            }

            // Gói dữ liệu cần thiết (DTO)
            var result = new PatientInfoDtos
            {
                UserId = patient.Id,
                FullName = patient.FullName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                Patients = new Patient
                {
                    Id = patient.Patient.Id,
                    MedicalHistory = patient.Patient.MedicalHistory
                }
            };

            return Ok(new { result });
        }
    }
}
