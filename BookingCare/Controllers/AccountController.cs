using BookingCare.Models;
using BookingCare.Repository;
using BookingCare.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        //====ĐĂNG XUẤT====//
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Đăng xuất tài khoản thành công";
            return RedirectToAction("Login", "Account");
        }

        //====QUÊN MẬT KHẨU====//
        //Bước 1: Nhập email đăng ký, mật khẩu mới và xác nhận mật khẩu mới
        [HttpGet]
        public IActionResult ForgotPasswordStep1()
        {
            return View();
        }

        //Bước 2: Xác thực mã OTP
        [HttpGet]
        public IActionResult ForgotPasswordStep2()
        {
            return View();
        }

        //====ĐĂNG NHẬP TÀI KHOẢN====//
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //====ĐĂNG KÝ TÀI KHOẢN 3 BƯỚC====//
        //Bước 1: Đăng ký tài khoản bằng email và mật khẩu
        [HttpGet]
        public IActionResult RegisterStep1()
        {
            return View();
        }

        //Bước 2: Xác thực mã OTP
        [HttpGet]
        public IActionResult RegisterStep2()
        {
            return View();
        }

        //Buớc 3: Hoàn tất đăng ký với thông tin cá nhân
        [HttpGet]
        public IActionResult RegisterStep3()
        {
            return View();
        }
    }
}