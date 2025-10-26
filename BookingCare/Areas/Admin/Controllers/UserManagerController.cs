using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> PatientManager()
        {
            var patients = _dbContext.Users
                            .Include(u => u.Patient)
                            .Where(u => u.Patient != null)
                            .ToList();
            return View(patients);
        }
        public async Task<IActionResult> DoctorManager()
        {
            var doctors = _dbContext.Users
                            .Include(u => u.Doctor)
                            .Where(u => u.Doctor != null)
                            .ToList();
            return View(doctors);
        }
        public IActionResult PatientDetails(string id)
        {
            var user = _dbContext.Users.Include(u => u.Patient).FirstOrDefault(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            return PartialView("_PatientDetail", user);
        }
        public IActionResult DoctorDetails(string id)
        {
            var user = _dbContext.Users.Include(u => u.Doctor).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return PartialView("_DoctorDetail", user);
        }
        [HttpPost]
        public async Task<IActionResult> LockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Khóa tài khoản", "Tài khoản của bạn đã bị khóa bởi quản trị viên. Vui lòng liên hệ để biết thêm chi tiết."));
            return RedirectToAction("PatientManager");
        }
        [HttpPost]
        public async Task<IActionResult> UnlockAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.SetLockoutEndDateAsync(user, null);
            Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Mở khóa tài khoản", "Tài khoản của bạn đã được mở khóa bởi quản trị viên. Bạn có thể đăng nhập lại."));
            return RedirectToAction("PatientManager");
        }
    }
}
