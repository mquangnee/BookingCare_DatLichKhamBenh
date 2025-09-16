using BookingCare.Models;
using BookingCare.Models.ViewModel;
using BookingCare.Repository;
using BookingCare.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookingCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly OtpService _otpService;
        public AccountController(DataContext dbContext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender, OtpService otpService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _otpService = otpService;
        }
        public IActionResult Index()
        {
            return View();
        }

        //-----Đăng xuất-----
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Đăng xuất tài khoản thành công";
            return RedirectToAction("Login", "Account");
        }

        //-----Đăng nhập-----
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đăng nhập tài khoản thành công";
                return RedirectToAction("Index", "Account");
            }
            return View();
        }

        //-----Đăng ký-----
        // Bước 1: Nhập email và mật khẩu
        [HttpGet]
        public IActionResult RegisterEmailPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEmailPassword(RegisterViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError(string.Empty, "Email đã được sử dụng!");
                return View(model);
            }
            // Tạo và gửi mã OTP
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(model.Email, otp);
            await _emailSender.SendEmailAsync(model.Email, "Mã OTP xác thực", $"Mã OTP của bạn là: {otp}. Mã có hiệu lực trong 5 phút.");
            //Lưu tạm thông tin đăng ký vào TempData để sử dụng ở bước tiếp theo
            HttpContext.Session.SetString("Email", model.Email); //Lưu email và password vào session để dùng cho các bước tiếp theo
            HttpContext.Session.SetString("Password", model.Password);
            //Chuyển sang bước nhập mã OTP
            return RedirectToAction("RegisterOTP", "Account");
        }
        // Bước 2: Nhập mã OTP
        [HttpGet]
        public IActionResult RegisterOTP()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterOTP(string otp)
        {
            if(HttpContext.Session.GetString("Email") == null || HttpContext.Session.GetString("Password") == null)
            {
                return RedirectToAction("RegisterEmailPassword", "Account");
            }
            string email = HttpContext.Session.GetString("Email");
            string password = TempData.Peek("Password") as string;
            string cachedOtp = _otpService.GetOtp(email);
            if(cachedOtp == null || cachedOtp != otp)
            {
                ModelState.AddModelError(string.Empty, "Mã OTP không hợp lễ hoặc hết hạn!");
                return View();
            }
            //OTP hợp lệ, chuyển sang bước đăng ký hoàn tất
            return RedirectToAction("RegisterComplete", "Account");
        }

        //Bước 3: Hoàn tất đăng ký
        [HttpGet]
        public IActionResult RegisterComplete()
        {
            if(HttpContext.Session.GetString("Email") == null || HttpContext.Session.GetString("Password") == null)
            {
                return RedirectToAction("RegisterEmailPassword", "Account");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterComplete(CompleteRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (HttpContext.Session.GetString("Email") == null || HttpContext.Session.GetString("Password") == null)
            {
                return RedirectToAction("RegisterEmailPassword", "Account");
            }
            string email = HttpContext.Session.GetString("Email");
            string password = HttpContext.Session.GetString("Password");
            string cachedOtp = _otpService.GetOtp(email);
            if(cachedOtp == null)
            {
                ModelState.AddModelError(string.Empty, "Mã OTP đã hết hạn!");
                return View(model);
            }
            // Tạo user mới
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = model.FullName,
                BirthOfDate = model.BirthOfDate,
                Gender = model.Gender,
                Address = model.Address
            };
            var result = await _userManager.CreateAsync(user, password);
            if(result.Succeeded)
            {
                _otpService.RemoveOtp(email); //Xóa OTP sau khi đăng ký thành công
                await _userManager.AddToRoleAsync(user, "Patient"); //Gán role Patient
                var patientEntity = new Patient //Tạo bản ghi trong bảng Patient
                {
                    UserId = user.Id,
                    MedicalHistory = "Chưa có tiền sử bệnh"
                };
                //Lưu bản ghi Patient vào CSDL
                await _dbContext.Patients.AddAsync(patientEntity);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Login", "Account");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}
