using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using BookingCare.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;

        public UserManagementApiController(DataContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailTemplate emailTemplate)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate; 
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

            // Gói dữ liệu cần thiết
            var result = new DoctorInfoDtos
            {
                UserId = doctor.Id,
                FullName = doctor.FullName,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber,
                DateOfBirth = doctor.DateOfBirth,
                Gender = doctor.Gender,
                Address = doctor.Address,
                DoctorId = doctor.Doctor.Id,
                Degree = doctor.Doctor.Degree,
                YearsOfExp = doctor.Doctor.YearsOfExp,
                SpecialtyName = doctor.Doctor.Specialty.Name,
                RoomName = doctor.Doctor.Room.Name
            };
            
            return Ok(result);
        }

        //Thêm tài khoản bác sĩ
        [HttpPost("create")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Vui lòng điền đầy đủ thông tin bác sĩ!" });
            }

            //Kiểm tra phòng còn trống không
            int doctorCountInRoom = _dbContext.Doctors.Count(d => d.RoomId == doctor.RoomId);
            if(doctorCountInRoom >= 2)
            {
                return BadRequest(new { success = false, message = "Phòng này đã đủ 2 bác sĩ, vui lòng chọn phòng khác!" });
            }

            var user = await _userManager.FindByEmailAsync(doctor.Email);
            if (user != null)
            {
                return BadRequest(new { success = false, message = "Email đã tồn tại trong hệ thông!" });
            }

            //Tạo đối tượng ApplicationUser mới
            var newDoctor = new ApplicationUser
            {
                UserName = doctor.Email,
                Email = doctor.Email,
                FullName = doctor.FullName,
                Gender = doctor.Gender,
                DateOfBirth = doctor.DateOfBirth,
                Address = doctor.Address,
                PhoneNumber = doctor.PhoneNumber
            };

            //Tạo tài khoản Bác sĩ với mật khẩu mặc định "Abcd@123"
            var result = await _userManager.CreateAsync(newDoctor, "Abcd@123");
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newDoctor, "Doctor"); //Gán vai trò Bác sĩ

                //Tạo bản ghi mới trong bảng Doctors
                var doctorEntity = new Doctor
                {
                    UserId = newDoctor.Id,
                    Degree = doctor.Degree,
                    YearsOfExp = doctor.YearsOfExp,
                    SpecialtyId = doctor.SpecialtyId,
                    RoomId = doctor.RoomId
                };
                await _dbContext.Doctors.AddAsync(doctorEntity);
                await _dbContext.SaveChangesAsync();

                //Nội dung email
                var body = _emailTemplate.GetDoctorAccountCreatedEmailBody(doctor.FullName, doctor.Email);

                //Gửi email tạo tài khoản thành công
                _ = Task.Run(() => _emailSender.SendEmailAsync(doctor.Email, "Tài khoản bác sĩ - BookingCare", body));
                return Ok(new { success = true, message = "Tạo tài khoản Bác sĩ thành công!" });
            }
            return BadRequest(new { success = false, message = "Tạo tài khoản Bác sĩ không thành công!" });
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

            // Gói dữ liệu cần thiết
            var result = new PatientInfoDtos
            {
                UserId = patient.Id,
                FullName = patient.FullName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Address = patient.Address,
                PatientId = patient.Patient.Id,
                MedicalHistory = patient.Patient.MedicalHistory
            };

            return Ok(result);
        }

        //Khóa tài khoản
        [HttpPut("lock/{id}")]
        public async Task<IActionResult> LockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng!" });
            }
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); //Khóa tài khoản vô thời hạn
            user.UpdatedAt = DateTime.Now;
            await _userManager.UpdateAsync(user); //Cập nhật tài khoản người dùng
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("Doctor")) //Nếu là tài khoản Bác sĩ
            {
                var body = _emailTemplate.GetAccountLockedEmailBody(user.FullName, "Bác sĩ");
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo khóa tài khoản - BookingCare", body));
                return Ok(new { success = true, message = "Khóa tài khoản thành công!" });
            }
            else //Nếu là tài khoản Bệnh nhân
            {
                var body = _emailTemplate.GetAccountLockedEmailBody(user.FullName, "Bệnh nhân");
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo khóa tài khoản - BookingCare", body));
                return Ok(new { success = true, message = "Khóa tài khoản thành công!" });
            }
        }

        //Mở khóa tài khoản
        [HttpPut("unlock/{id}")]
        public async Task<IActionResult> UnlockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy người dùng!" });
            }
            await _userManager.SetLockoutEndDateAsync(user, null); //Mở khóa
            user.UpdatedAt = DateTime.Now;
            await _userManager.UpdateAsync(user); //Cập nhật tài khoản người dùng
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("Doctor")) //Nếu là tài khoản Bác sĩ
            {
                var body = _emailTemplate.GetAccountUnlockedEmailBody(user.FullName, "Bác sĩ");
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo mở khóa tài khoản - BookingCare", body));
                return Ok(new { success = true, message = "Mở khóa tài khoản thành công!" });
            }
            else //Nếu là tài khoản Bệnh nhân
            {
                var body = _emailTemplate.GetAccountUnlockedEmailBody(user.FullName, "Bệnh nhân");
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo mở khóa tài khoản - BookingCare", body));
                return Ok(new { success = true, message = "Mở khóa tài khoản thành công!" });
            }
        }
    }
}
