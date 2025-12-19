using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using BookingCare.Services;
using BookingCare.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Patient.Controllers.Api
{
    [Area("Patient")]
    [Route("Patient/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class ChangePasswordApi : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;
        private readonly OtpService _otpService;

        public ChangePasswordApi(DataContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailTemplate emailTemplate, OtpService otpService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate;
            _otpService = otpService;
        }

        //====Đổi mật khẩu====//
        //Bước 1: Nhập mật khẩu cũ, mật khẩu mới và xác nhận mật khẩu mới
        [HttpPost("changePass-step1")]
        public async Task<IActionResult> ChangePasswordStep1([FromBody] ChangePasswordStep1Dtos dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Vui lòng điền đầy đủ mật khẩu cũ mật khẩu mới và xác nhận mật khẩu mới!" });
            }

            //Lấy thông tin người dùng
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { success = false, message = "Người dùng không tồn tại!" });
            }

            //Kiểm tra mật khẫu cũ
            var check = await _userManager.CheckPasswordAsync(user, dto.OldPassword);
            if (!check)
            {
                return BadRequest(new { success = false, message = "Mật khẫu cũ không đúng!" });
            }

            //Tạo mã OTP và mật khẩu mới lưu vào bộ nhớ đệm
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(user.Email, otp);

            //Lưu mật khẩu mới tạm thời vào bộ nhớ đệm
            _otpService.SetPassword(user.Email, dto.NewPassword);

            //Nội dung email
            var body = _emailTemplate.getChangePasswordOtpEmailBody(user.FullName, otp);

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Xác nhận đổi mật khẩu - BookingCare", body));
            return Ok(new { success = true, message = "Đã gửi mã OTP đến email của bạn!" });
        }

        //Bước 2: Xác thực mã OTP
        [HttpPost("changePass-step2")]
        public async Task<IActionResult> ChangePasswordStep2([FromBody] ChangePasswordStep2Dtos dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Vui lòng điền mã xác thực OTP!" });
            }

            //Lấy thông tin người dùng
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { success = false, message = "Người dùng không tồn tại!" });
            }

            string? cachedOtp = _otpService.GetOtp(user.Email);
            //Kiểm tra OTP
            if (cachedOtp == null)
            {
                return BadRequest(new { success = false, message = "Mã OTP đã hết hạn. Vui lòng thử lại!" });
            }
            if (cachedOtp != dto.Otp)
            {
                return BadRequest(new { success = false, message = "Mã OTP không đúng. Vui lòng kiểm tra lại!" });
            }

            //Lấy mật khẩu tạm thời từ bộ nhớ đệm
            var password = _otpService.GetPassword(user.Email);

            //Kiểm tra mật khẩu mới có còn tồn tại không
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest(new { success = false, message = "Mật khẩu mới đã hết hạn. Vui lòng thử lại!" });
            }

            //Xóa mật khẩu cũ và thêm mật khẩu mới
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, password);
            if (result.Succeeded)
            {
                _otpService.RemoveOtp(user.Email); //Xóa mã OTP khỏi bộ nhớ đệm
                _otpService.RemovePassword(user.Email); //Xóa mật khẩu tạm thời khỏi bộ nhớ đệm

                //Nội dung email
                var body = _emailTemplate.getChangePasswordSuccessEmailBody(user.FullName);

                //Gửi email thông báo đổi mật khẩu thành công
                _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Thông báo đổi mật khẩu - BookingCare", body));
                return Ok(new { success = true, message = "Đổi mật khẩu thành công!" });
            }
            return BadRequest(new { success = false, message = "Đổi mật khẩu không thành công!" });
        }

        //====GỬI LẠI MÃ OTP====//
        [HttpPost("changePass-resend-otp")]
        public async Task<IActionResult> ChangePassResendOtp([FromBody] ChangePasswordStep2Dtos dto)
        {
            //Lấy thông tin người dùng
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { success = false, message = "Người dùng không tồn tại!" });
            }

            //Tạo và gửi mã OTP qua email
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(user.Email, otp);

            //Nội dung email
            var body = _emailTemplate.getResendChangePassOtpEmailBody(otp);

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(user.Email, "Gửi lại mã OTP - BookingCare", body));
            return Ok(new { success = true, message = "Đã gửi lại mã OTP!" });
        }
    }
}
