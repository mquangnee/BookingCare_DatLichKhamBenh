using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using BookingCare.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookingCare.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserApiController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;

        public UserApiController(DataContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailTemplate emailTemplate)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate; 
        }

        //=============================================
        // I. QUẢN LÝ TÀI KHOẢN BÁC SĨ
        // 1. Lấy danh sách bác sĩ
        // GET: /api/admin/users/doctors
        //=============================================
        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors(string? search = "", int page = 1, int pageSize = 10)
        {
            //Lấy danh sách người dùng có vai trò là bác sĩ
            var doctors = _dbContext.Users
                            .Include(u => u.Doctor)
                            .Where(u => u.Doctor != null);

            //Tìm bác sĩ theo tên
            if (!string.IsNullOrWhiteSpace(search))
            {
                doctors = doctors.Where(u => u.FullName.Contains(search));
            }

            //Tổng số bác sĩ
            var totalDoctors = await doctors.CountAsync();

            //Lấy danh sách hiển thị ở trang muốn xem
            var listDoctor = await doctors
                            .OrderByDescending(d => d.Doctor.Id)
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
                            }).ToListAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    totalDoctors,
                    listDoctor
                }
            });
        }

        //=============================================
        // 2. Lấy thông tin chi tiết bác sĩ
        // GET: /api/admin/users/doctors/id
        //=============================================
        [HttpGet("doctors/{id}")]
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
                return NotFound(new 
                {
                    success = false,
                    message = "Không tìm thấy bác sĩ!"
                });
            }

            // Gói dữ liệu cần thiết
            var inforDoctor = new DoctorInfoDtos
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
                RoomName = doctor.Doctor.Room.Name,
                AvatarUrl = doctor.Doctor.AvatarUrl
            };
            
            return Ok(new
            {
                success = true,
                data = inforDoctor
            });
        }

        //=============================================
        // 3. Tạo tài khoản bác sĩ
        // POST: /api/admin/users/doctors
        //=============================================
        [HttpPost("doctors")]
        public async Task<IActionResult> AddDoctor([FromForm] AddDoctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new 
                {
                    success = false, 
                    message = "Vui lòng điền đầy đủ thông tin bác sĩ!" 
                });
            }

            //Kiểm tra phòng còn trống không
            int doctorCountInRoom = _dbContext.Doctors.Count(d => d.RoomId == doctor.RoomId);
            if(doctorCountInRoom >= 2)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Phòng này đã đủ 2 bác sĩ, vui lòng chọn phòng khác!" 
                });
            }

            var user = await _userManager.FindByEmailAsync(doctor.Email);
            if (user != null)
            {
                return BadRequest(new
                { 
                    success = false,
                    message = "Email đã tồn tại trong hệ thông!" 
                });
            }

            // Lưu ảnh
            string avatarPath = "/images/doctors/avatar_default.jpg";
            if (doctor.Avatar != null)
            {
                var folder = Path.Combine("wwwroot/images/doctors");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(doctor.Avatar.FileName);
                var filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await doctor.Avatar.CopyToAsync(stream);

                avatarPath = "/images/doctors/" + fileName;
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
                var doctorEntity = new Models.Doctor
                {
                    UserId = newDoctor.Id,
                    Degree = doctor.Degree,
                    YearsOfExp = doctor.YearsOfExp,
                    SpecialtyId = doctor.SpecialtyId,
                    RoomId = doctor.RoomId,
                    AvatarUrl = avatarPath
                };
                await _dbContext.Doctors.AddAsync(doctorEntity);
                await _dbContext.SaveChangesAsync();

                //Nội dung email
                var body = _emailTemplate.GetDoctorAccountCreatedEmailBody(doctor.FullName, doctor.Email);

                //Gửi email tạo tài khoản thành công
                _ = Task.Run(() => _emailSender.SendEmailAsync(doctor.Email, "Tài khoản bác sĩ - BookingCare", body));
                return Ok(new 
                { 
                    success = true, 
                    message = "Tạo tài khoản Bác sĩ thành công!" 
                });
            }
            return BadRequest(new 
            { 
                success = false, 
                message = "Tạo tài khoản Bác sĩ không thành công!" 
            });
        }

        //=============================================
        // 4. Cập nhật thông tin bác sĩ
        // Bước 1: Lấy thông tin bác sĩ cần cập nhật
        // GET: /api/admin/users/doctors/id/edit
        //=============================================
        [HttpGet("doctors/{id}/edit")]
        public IActionResult UpdateDoctorDetails(string id)
        {
            var doctor = _dbContext.Users
                .Include(u => u.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(u => u.Doctor)
                    .ThenInclude(d => d.Room)
                .FirstOrDefault(u => u.Id == id);

            if (doctor == null)
            {
                return NotFound(new 
                { 
                    success = true,
                    message = "Không tìm thấy bác sĩ!"
                });
            }

            // Gói dữ liệu cần thiết
            var inforUpdateDoctor = new DoctorInfoUpdateDtos
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
                SpecialtyId = doctor.Doctor.SpecialtyId,
                RoomId = doctor.Doctor.RoomId,
                AvatarUrl = doctor.Doctor.AvatarUrl
            };

            return Ok(new 
            {
                success = true,
                data = inforUpdateDoctor
            });
        }

        //=============================================
        // Bước 2: Cập nhật thông tin bác sĩ
        // PUT: /api/admin/users/doctors/id/edit
        //=============================================
        [HttpPut("doctors/{id}/edit")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromForm] UpdateDoctor update_doctor)
        {
            //Kiểm tra dữ liệu gửi về hợp lệ không
            if (!ModelState.IsValid)
            {
                return BadRequest(new 
                {
                    success = false, 
                    message = "Vui lòng điền đầy đủ thông tin bác sĩ!" 
                });
            }

            var doctor = await _userManager.Users.Include(u => u.Doctor).FirstOrDefaultAsync(u => u.Id == id);
            if (doctor == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Không tìm thấy bác sĩ!"
                });
            }

            //Cập nhật thông tin
            if (doctor.Doctor.RoomId != update_doctor.RoomId)
            {
                //Kiểm tra phòng còn trống không
                int doctorCountInRoom = _dbContext.Doctors.Count(d => d.RoomId == update_doctor.RoomId);
                if (doctorCountInRoom == 2)
                {
                    return BadRequest(new 
                    {
                        success = false,
                        message = "Phòng đã đầy, vui lòng chọn phòng khác!" 
                    });
                }
                doctor.Doctor.RoomId = update_doctor.RoomId;
            }
            // Update avatar
            if (update_doctor.Avatar != null)
            {
                var folder = Path.Combine("wwwroot/images/doctors");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(update_doctor.Avatar.FileName);
                var filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await update_doctor.Avatar.CopyToAsync(stream);

                doctor.Doctor.AvatarUrl = "/images/doctors/" + fileName;
            }

            doctor.Address = update_doctor.Address;
            doctor.PhoneNumber = update_doctor.PhoneNumber;
            doctor.Doctor.Degree = update_doctor.Degree;
            doctor.Doctor.YearsOfExp = update_doctor.YearsOfExp;
            doctor.Doctor.SpecialtyId = update_doctor.SpecialtyId;
            doctor.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            //Nội dung email
            var body = _emailTemplate.GetDoctorInfoUpdatedEmailBody(doctor.FullName, doctor.Email);

            //Gửi email
            _ = Task.Run(() => _emailSender.SendEmailAsync(doctor.Email, "Cập nhật thông tin - BookingCare", body));
            return Ok(new 
            { 
                success = true, 
                message = "Cập nhật thông tin Bác sĩ thành công!" 
            });
        }

        //=============================================
        // II. QUẢN LÝ TÀI KHOẢN BỆNH NHÂN
        // 1. Lấy danh sách bệnh nhân
        // GET: /api/admin/users/patients
        //=============================================
        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients (string? search = "", int page = 1, int pageSize = 10)
        {
            //Lấy danh sách người dùng có vai trò là bác sĩ
            var patients = _dbContext.Users
                            .Include(u => u.Patient)
                            .Where(u => u.Patient != null);

            // Áp dụng search
            if (!string.IsNullOrWhiteSpace(search))
            {
                patients = patients.Where(u =>
                            u.FullName.Contains(search) ||
                            u.Email.Contains(search) ||
                            (u.PhoneNumber != null && u.PhoneNumber.Contains(search))
                );
            }

            //Tổng số bác sĩ
            var totalPatients = await patients.CountAsync();

            //Lấy danh sách hiển thị ở trang muốn xem
            var listPatients = await patients
                            .OrderByDescending(d => d.Patient.Id)
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

            return Ok(new 
            {
                success = true,
                data = new
                {
                    totalPatients,
                    listPatients
                }
            });
        }

        //=============================================
        // 2. Lấy thông tin chi tiết bệnh nhân
        // GET: /api/admin/users/patients/id
        //=============================================
        [HttpGet("patients/{id}")]
        public IActionResult PatientDetails(string id)
        {
            var patient = _dbContext.Users
                        .Include(u => u.Patient)
                        .FirstOrDefault(u => u.Id == id);

            if (patient == null)
            {
                return NotFound(new 
                {
                    success = false,
                    message = "Không tìm thấy bệnh nhân!"
                });
            }

            // Gói dữ liệu cần thiết
            var inforPatient = new PatientInfoDtos
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

            return Ok(new
            {
                success = true,
                data = inforPatient
            });
        }

        //=============================================
        // III. KHÓA/MỞ KHÓA TÀI KHOẢN BÁC SĨ VÀ BỆNH NHÂN
        // 1. Khóa tài khoản
        // PUT: /api/admin/users/lock/id
        //=============================================
        [HttpPut("lock/{id}")]
        public async Task<IActionResult> LockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new 
                {
                    success = false, 
                    message = "Không tìm thấy người dùng!" 
                });
            }
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); //Khóa tài khoản vô thời hạn
            user.UpdatedAt = DateTime.Now;
            await _userManager.UpdateAsync(user); //Cập nhật tài khoản người dùng
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("Doctor")) //Nếu là tài khoản Bác sĩ
            {
                var body = _emailTemplate.GetAccountLockedEmailBody(user.FullName, "Bác sĩ");
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo khóa tài khoản - BookingCare", body));
                return Ok(new
                {
                    success = true,
                    message = "Khóa tài khoản thành công!" 
                });
            }
            else //Nếu là tài khoản Bệnh nhân
            {
                var body = _emailTemplate.GetAccountLockedEmailBody(user.FullName, "Bệnh nhân");
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo khóa tài khoản - BookingCare", body));
                return Ok(new
                { 
                    success = true, 
                    message = "Khóa tài khoản thành công!"
                });
            }
        }

        //=============================================
        // III. KHÓA/MỞ KHÓA TÀI KHOẢN BÁC SĨ VÀ BỆNH NHÂN
        // 2. Mở khóa tài khoản
        // PUT: /api/admin/users/unlock/id
        //=============================================
        [HttpPut("unlock/{id}")]
        public async Task<IActionResult> UnlockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new 
                {
                    success = false,
                    message = "Không tìm thấy người dùng!" 
                });
            }
            await _userManager.SetLockoutEndDateAsync(user, null); //Mở khóa
            user.UpdatedAt = DateTime.Now;
            await _userManager.UpdateAsync(user); //Cập nhật tài khoản người dùng
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("Doctor")) //Nếu là tài khoản Bác sĩ
            {
                var body = _emailTemplate.GetAccountUnlockedEmailBody(user.FullName, "Bác sĩ");
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo mở khóa tài khoản - BookingCare", body));
                return Ok(new 
                { 
                    success = true, 
                    message = "Mở khóa tài khoản thành công!" 
                });
            }
            else //Nếu là tài khoản Bệnh nhân
            {
                var body = _emailTemplate.GetAccountUnlockedEmailBody(user.FullName, "Bệnh nhân");
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo mở khóa tài khoản - BookingCare", body));
                return Ok(new 
                { 
                    success = true,
                    message = "Mở khóa tài khoản thành công!" 
                });
            }
        }
    }
}
