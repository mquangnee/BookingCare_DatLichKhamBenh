using BookingCare.Models;
using BookingCare.Models.ViewModel;
using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookingCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public UserManagerController(DataContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        //---QUẢN LÝ TÀI KHOẢN BỆNH NHÂN---
        //Hiển thị danh sách bệnh nhân
        public async Task<IActionResult> PatientManager()
        {
            var patients = _dbContext.Users
                            .Include(u => u.Patient)
                            .Where(u => u.Patient != null)
                            .ToList();//Lấy danh sách người dùng có vai trò là bệnh nhân
            return View(patients);
        }
        //Lấy chi tiết bệnh nhân
        public IActionResult PatientDetails(string id)
        {
            var patient = _dbContext.Users.Include(u => u.Patient).FirstOrDefault(u => u.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            return PartialView("_PatientDetail", patient);
        }

        //---QUẢN LÝ TÀI KHOẢN BÁC SĨ---
        //Hiển thị danh sách bác sĩ
        public async Task<IActionResult> DoctorManager()
        {
            var doctors = _dbContext.Users
                            .Include(u => u.Doctor)
                            .Where(u => u.Doctor != null)
                            .ToList();//Lấy danh sách người dùng có vai trò là bác sĩ
            return View(doctors);
        }
        
        //Lấy chi tiết bác sĩ bao gồm chuyên khoa và phòng khám
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
                return NotFound();
            }
            return PartialView("_DoctorDetail", doctor);
        }
        //Thêm tài khoản bác sĩ
        [HttpGet]
        public IActionResult AddDoctor()
        {
            // Lấy danh sách chuyên khoa
            var specialties = _dbContext.Specialties.ToList();
            ViewBag.Specialties = new SelectList(specialties, "Id", "Name");

            // Lấy danh sách phòng kèm số lượng bác sĩ trong mỗi phòng
            var rooms = _dbContext.Rooms
                .Select(r => new
                {
                    r.Id,
                    Name = r.Name,
                    DoctorCount = _dbContext.Doctors.Count(d => d.RoomId == r.Id)
                })
                .ToList();

            // Tạo danh sách SelectListItem có format "Phòng 101 (1/2 bác sĩ)"
            var roomItems = rooms.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = $"{r.Name} ({r.DoctorCount}/2 bác sĩ)",
                Disabled = r.DoctorCount >= 2 // nếu đủ 2 bác sĩ thì không chọn được
            }).ToList();
            ViewBag.Rooms = roomItems;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctor(DoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra phòng còn trống không
            int doctorCountInRoom = _dbContext.Doctors.Count(d => d.RoomId == model.RoomId);
            if (doctorCountInRoom >= 2)
            {
                ModelState.AddModelError("RoomId", "Phòng này đã đủ 2 bác sĩ, vui lòng chọn phòng khác.");
                return View(model);
            }
            
            if (await _userManager.FindByEmailAsync(model.Email) == null)//Kiểm tra email đã tồn tại chưa
            {
                var doctor = new ApplicationUser
                {
                    UserName = model.Email, //Tên đăng nhập = email
                    Email = model.Email,
                    EmailConfirmed = true,
                    FullName = model.FullName,
                    DateOfBirth = model.BirthOfDate,
                    Gender = model.Gender,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await _userManager.CreateAsync(doctor, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(doctor, "Doctor"); //Gán role Doctor
                    var specialty = await _dbContext.Specialties.FirstOrDefaultAsync(s => s.Id == model.SpecialtyId);
                    var room = await _dbContext.Rooms.FirstOrDefaultAsync(r => r.Id == model.RoomId);
                    var doctorEntity = new Doctor //Tạo bản ghi trong bảng Doctors
                    {
                        UserId = doctor.Id,
                        Degree = model.Degree,
                        YearsOfExp = model.YearsOfExp,
                        SpecialtyId = specialty.Id,
                        RoomId = room.Id
                    };
                    await _dbContext.Doctors.AddAsync(doctorEntity); //Thêm bản ghi vào bảng Doctors
                }
                _dbContext.SaveChanges();
                Task.Run(() => _emailSender.SendEmailAsync(model.Email, "Thông báo tài khoản BookingCare", 
                    $@"<p>Xin chào <strong>{model.FullName}</strong>,</p>
                    <p>Tài khoản bác sĩ của bạn đã được tạo thành công. Thông tin đăng nhập:</p>
                    <ul>
                        <li>Tài khoản: <span style='background-color:yellow; font-weight:bold;'>{model.Email}</span></li>
                        <li>Mật khẩu: <span style='background-color:yellow; font-weight:bold;'>{model.Password}</span></li>
                    </ul>
                    <p>Trân trọng,<br/>BookingCare Team</p>"));
                // Thêm thông báo thành công
                TempData["SuccessMessage"] = "Tạo bác sĩ thành công!";
                return RedirectToAction("DoctorManager", "UserManager");
            }
            else
            {
                ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống!");
                return View(model);
            }
        }
        //Cập nhật thông tin bác sĩ
        [HttpGet]
        public async Task<IActionResult> UpdateDoctor(string id)
        {
            var doctor = _dbContext.Users.Include(d => d.Doctor).FirstOrDefault(d => d.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }
            var model = new DoctorViewModel
            {
                Email = doctor.Email,
                Password = null,
                FullName = doctor.FullName,
                Gender = doctor.Gender,
                BirthOfDate = doctor.DateOfBirth,
                Address = doctor.Address,
                PhoneNumber = doctor.PhoneNumber,
                Degree = doctor.Doctor.Degree,
                YearsOfExp = doctor.Doctor.YearsOfExp,
                SpecialtyId = doctor.Doctor.SpecialtyId,
                RoomId = doctor.Doctor.RoomId
            };
            // Lấy danh sách chuyên khoa
            ViewBag.Specialties = new SelectList(_dbContext.Specialties.ToList(), "Id", "Name", model.SpecialtyId);

            // Lấy danh sách phòng kèm số lượng bác sĩ trong mỗi phòng
            var rooms = _dbContext.Rooms.Select(r => new {r.Id, r.Name, DoctorCount = _dbContext.Doctors.Count(d => d.RoomId == r.Id)}).ToList();
            ViewBag.Rooms = new SelectList(rooms, "Id", "Text", model.RoomId);
            var roomListForDropdown = rooms.Select(r => new { r.Id, Text = $"{r.Name} ({r.DoctorCount}/2)" }).ToList();
            ViewBag.Rooms = new SelectList(roomListForDropdown, "Id", "Text", model.RoomId);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctor(DoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Lấy danh sách chuyên khoa
                ViewBag.Specialties = new SelectList(_dbContext.Specialties.ToList(), "Id", "Name", model.SpecialtyId);

                // Lấy danh sách phòng kèm số lượng bác sĩ trong mỗi phòng
                var rooms = _dbContext.Rooms.Select(r => new { r.Id, r.Name, DoctorCount = _dbContext.Doctors.Count(d => d.RoomId == r.Id) }).ToList();
                ViewBag.Rooms = new SelectList(rooms, "Id", "Text", model.RoomId);
                var roomListForDropdown = rooms.Select(r => new { r.Id, Text = $"{r.Name} ({r.DoctorCount}/2)" }).ToList();
                ViewBag.Rooms = new SelectList(roomListForDropdown, "Id", "Text", model.RoomId);
                return View(model);
            }
            var doctor = await _dbContext.Users.Include(d => d.Doctor).FirstOrDefaultAsync(d => d.Email == model.Email);
            if (doctor == null)
            {
                return NotFound();
            }
            if (doctor.Doctor.RoomId != model.RoomId)
            {
                // Kiểm tra phòng còn trống không
                int doctorCountInRoom = _dbContext.Doctors.Count(d => d.RoomId == model.RoomId);
                if (doctorCountInRoom == 2)
                {
                    ModelState.AddModelError("RoomId", "Phòng này đã đủ 2 bác sĩ, vui lòng chọn phòng khác.");
                    // Lấy danh sách chuyên khoa
                    ViewBag.Specialties = new SelectList(_dbContext.Specialties.ToList(), "Id", "Name", model.SpecialtyId);

                    // Lấy danh sách phòng kèm số lượng bác sĩ trong mỗi phòng
                    var rooms = _dbContext.Rooms.Select(r => new { r.Id, r.Name, DoctorCount = _dbContext.Doctors.Count(d => d.RoomId == r.Id) }).ToList();
                    ViewBag.Rooms = new SelectList(rooms, "Id", "Text", model.RoomId);
                    var roomListForDropdown = rooms.Select(r => new { r.Id, Text = $"{r.Name} ({r.DoctorCount}/2)" }).ToList();
                    ViewBag.Rooms = new SelectList(roomListForDropdown, "Id", "Text", model.RoomId);
                    return View(model);
                }
            }
            doctor.Address = model.Address;
            doctor.PhoneNumber = model.PhoneNumber;
            doctor.Doctor.Degree = model.Degree;
            doctor.Doctor.YearsOfExp = model.YearsOfExp;
            doctor.Doctor.SpecialtyId = model.SpecialtyId;
            doctor.Doctor.RoomId = model.RoomId;
            doctor.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            Task.Run(() => _emailSender.SendEmailAsync(model.Email, "Cập nhật tài khoản BookingCare", 
                $@"<p>Xin chào <strong>{model.FullName}</strong>,</p>
                <p>Thông tin của bạn đã cập nhật. Vui lòng truy cập hệ thống để xem thông tin mới.</p>
                <p>Trân trọng,<br/>BookingCare Team</p>"));
            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Thông tin bác sĩ đã được cập nhật!";
            return RedirectToAction("DoctorManager");
        }

        //Khóa tài khoản
        [HttpPost]
        public async Task<IActionResult> LockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue); //Khóa tài khoản vô thời hạn
            user.UpdatedAt = DateTime.Now;
            await _userManager.UpdateAsync(user); //Cập nhật thời gian update
            Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Khóa tài khoản", 
                "Tài khoản của bạn đã bị khóa bởi quản trị viên. Vui lòng liên hệ để biết thêm chi tiết."));
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("Doctor"))
            {
                return RedirectToAction("DoctorManager");
            }
            else
            {
                return RedirectToAction("PatientManager");
            }
        }
        //Mở khóa tài khoản
        [HttpPost]
        public async Task<IActionResult> UnlockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.SetLockoutEndDateAsync(user, null); //Mở khóa tài khoản
            user.UpdatedAt = DateTime.Now;
            await _userManager.UpdateAsync(user); //Cập nhật thời gian update
            Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Mở khóa tài khoản", 
                "Tài khoản của bạn đã được mở khóa bởi quản trị viên. Bạn có thể đăng nhập lại."));
            var role = await _userManager.GetRolesAsync(user);
            if (role.Contains("Doctor"))
            {
                return RedirectToAction("DoctorManager");
            }
            else
            {
                return RedirectToAction("PatientManager");
            }
        }
    }
}
