using BookingCare.Models;
using BookingCare.Models.ViewModel;
using BookingCare.Repository;
using BookingCare.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly OtpService _otpService;
        public AccountController(DataContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, OtpService otpService)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        //    if (result.Succeeded)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);
        //        var roles = await _userManager.GetRolesAsync(user);
        //        if (roles.Contains("Admin"))
        //        {
        //            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        //        }
        //        else if (roles.Contains("Doctor"))
        //        {
        //            return RedirectToAction("Index", "Doctor", new { area = "Doctor" });
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Patient", new { area = "Patient" });
        //        }
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Đăng nhập không thành công!";
        //        return View(model);
        //    }
        //}

        //-----Đăng ký-----
        // Bước 1: Nhập email và mật khẩu
        [HttpGet]
        public IActionResult RegisterStep1()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> RegisterStep1(RegisterViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    if (await _userManager.FindByEmailAsync(model.Email) != null)
        //    {
        //        ModelState.AddModelError(string.Empty, "Email đã được sử dụng!");
        //        return View(model);
        //    }
        //    // Tạo và gửi mã OTP
        //    string otp = new Random().Next(100000, 999999).ToString();
        //    _otpService.SetOtp(model.Email, otp);
        //    await _emailSender.SendEmailAsync(model.Email, "Mã OTP xác thực", $@"<p>Mã OTP của bạn là: <strong style='color:red; font-weight:bold;'>{otp}</strong>.</p><p>Mã có hiệu lực trong 5 phút.</p>");
        //    //Lưu tạm thông tin đăng ký vào TempData để sử dụng ở bước tiếp theo
        //    HttpContext.Session.SetString("Email", model.Email); //Lưu email và password vào session để dùng cho các bước tiếp theo
        //    HttpContext.Session.SetString("Password", model.Password);
        //    //Chuyển sang bước nhập mã OTP
        //    return RedirectToAction("RegisterOTP", "Account");
        //}
        // Bước 2: Nhập mã OTP
        [HttpGet]
        public IActionResult RegisterStep2()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult RegisterStep2(string otp)
        //{
        //    if (HttpContext.Session.GetString("Email") == null || HttpContext.Session.GetString("Password") == null)
        //    {
        //        return RedirectToAction("RegisterEmailPassword", "Account");
        //    }
        //    string email = HttpContext.Session.GetString("Email");
        //    string password = HttpContext.Session.GetString("Password");
        //    string cachedOtp = _otpService.GetOtp(email);
        //    if (cachedOtp == null || cachedOtp != otp)
        //    {
        //        ModelState.AddModelError(string.Empty, "Mã OTP không hợp lễ hoặc hết hạn!");
        //        return View();
        //    }
        //    //OTP hợp lệ, chuyển sang bước đăng ký hoàn tất
        //    return RedirectToAction("RegisterComplete", "Account");
        //}

        //Bước 3: Hoàn tất đăng ký
        [HttpGet]
        public IActionResult RegisterStep3()
        {
            //if (HttpContext.Session.GetString("Email") == null || HttpContext.Session.GetString("Password") == null)
            //{
            //    return RedirectToAction("RegisterEmailPassword", "Account");
            //}
            //ViewBag.Email = HttpContext.Session.GetString("Email");
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RegisterStep3(CompleteRegisterViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    if (HttpContext.Session.GetString("Email") == null || HttpContext.Session.GetString("Password") == null)
        //    {
        //        return RedirectToAction("RegisterEmailPassword", "Account");
        //    }
        //    string email = HttpContext.Session.GetString("Email");
        //    string password = HttpContext.Session.GetString("Password");
        //    string cachedOtp = _otpService.GetOtp(email);
        //    if (cachedOtp == null)
        //    {
        //        ModelState.AddModelError(string.Empty, "Mã OTP đã hết hạn!");
        //        return View(model);
        //    }
        //    // Tạo user mới
        //    var user = new ApplicationUser
        //    {
        //        UserName = email,
        //        Email = email,
        //        FullName = model.FullName,
        //        DateOfBirth = model.BirthOfDate,
        //        Gender = model.Gender,
        //        Address = model.Address,
        //        PhoneNumber = model.PhoneNumber
        //    };
        //    var result = await _userManager.CreateAsync(user, password);
        //    if (result.Succeeded)
        //    {
        //        _otpService.RemoveOtp(email); //Xóa OTP sau khi đăng ký thành công
        //        await _userManager.AddToRoleAsync(user, "Patient"); //Gán role Patient
        //        var patientEntity = new Patient //Tạo bản ghi trong bảng Patient
        //        {
        //            UserId = user.Id,
        //            MedicalHistory = "Chưa có tiền sử bệnh"
        //        };
        //        //Lưu bản ghi Patient vào CSDL
        //        _dbContext.Patients.Add(patientEntity);
        //        await _dbContext.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
        //        return RedirectToAction("Login", "Account");
        //    }
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error.Description);
        //    }
        //    return View(model);
        //}

        //-----Quên mật khẩu-----
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Kiểm tra email tồn tại
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email không tồn tại trong hệ thống!");
                return View(model);
            }
            // Tạo và gửi mã OTP
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(model.Email, otp);
            await _emailSender.SendEmailAsync(model.Email, "Mã OTP đặt lại mật khẩu",
                $@"<p>Mã OTP của bạn là: <strong style='color:red; font-weight:bold;'>{otp}</strong>.</p><p>Mã có hiệu lực trong 5 phút.</p>");
            //Lưu tạm email và mật khẩu mới vào Session để sử dụng ở bước tiếp theo
            HttpContext.Session.SetString("ResetEmail", model.Email);
            HttpContext.Session.SetString("NewPassword", model.NewPassword);
            //Chuyển sang bước nhập mã OTP
            return RedirectToAction("ResetPasswordOTP", "Account");
        }
        [HttpGet]
        public IActionResult ResetPasswordOTP()
        {
            if (HttpContext.Session.GetString("ResetEmail") == null || HttpContext.Session.GetString("NewPassword") == null)
            {
                return RedirectToAction("ResetPassword", "Account");
            }
            ViewBag.Email = HttpContext.Session.GetString("ResetEmail");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPasswordOTP(string otp)
        {
            if (HttpContext.Session.GetString("ResetEmail") == null || HttpContext.Session.GetString("NewPassword") == null)
            {
                return RedirectToAction("ForgotPassword", "Account");
            }
            string email = HttpContext.Session.GetString("ResetEmail");
            string newPassword = HttpContext.Session.GetString("NewPassword");
            string cachedOtp = _otpService.GetOtp(email);
            //Kiểm tra OTP
            if (cachedOtp == null || cachedOtp != otp)
            {
                ModelState.AddModelError(string.Empty, "Mã OTP không hợp lệ hoặc đã hết hạn!");
                return View();
            }
            //Đặt lại mật khẩu mới
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Người dùng không tồn tại!");
                return View();
            }
            //Xóa mật khẩu cũ và thêm mật khẩu mới
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, newPassword);
            if (result.Succeeded)
            {
                _otpService.RemoveOtp(email); //Xóa OTP sau khi đặt lại mật khẩu thành công
                //Xóa session tạm
                HttpContext.Session.Remove("ResetEmail");
                HttpContext.Session.Remove("NewPassword");
                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("ResetPassword");
        }
    }
}