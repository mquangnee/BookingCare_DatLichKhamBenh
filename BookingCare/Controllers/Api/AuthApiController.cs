using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using BookingCare.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DataContext _dbContext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly OtpService _otpService;
        public AuthApiController(UserManager<ApplicationUser> userManager, DataContext dbContext, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, OtpService otpService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _otpService = otpService;
        }

        //====ĐĂNG NHẬP TÀI KHOẢN====//
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDtos dto)
        //{
        //               var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, lockoutOnFailure: false);
        //    if (result.Succeeded)
        //    {
        //        var user = await _userManager.FindByEmailAsync(dto.Email);
        //        var roles = await _userManager.GetRolesAsync(user);
        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Đăng nhập thành công!",
        //            data = new
        //            {
        //                userId = user.Id,
        //                email = user.Email,
        //                fullName = user.FullName,
        //                roles = roles
        //            }
        //        });
        //    }
        //    return BadRequest(new { success = false, message = "Đăng nhập không thành công! Vui lòng kiểm tra lại email và mật khẩu." });
        //}

        //====ĐĂNG KÝ TÀI KHOẢN 3 BƯỚC====//
        //Bước 1: Đăng ký tài khoản bằng email và mật khẩu
        [HttpPost("register-step1")]
        public async Task<IActionResult> RegisterStep1([FromBody] RegisterStep1Dtos dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)//Kiểm tra email đã tồn tại chưa
            {
                return BadRequest(new { success = false, message = "Email đã được sử dụng" });
            }
            //Tạo và gửi mã OTP qua email
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(dto.Email, otp);

            //Lưu mật khẩu tạm thời vào bộ nhớ đệm
            _otpService.SetPassword(dto.Email, dto.Password);

            //Gửi email xác nhận OTP
            _ = _emailSender.SendEmailAsync(dto.Email, "Xác nhận mã OTP - BookingCare",
               $@"<div style='font-family:Arial, sans-serif; color:#333; line-height:1.6;'>
                    <h2 style='color:#2a8dc5;'>BookingCare - Xác thực tài khoản</h2>
                    <p>Xin chào,</p>
                    <p>Bạn đang thực hiện xác thực tài khoản trên hệ thống <strong>BookingCare</strong>.</p>
                    <p>Mã OTP của bạn là: 
                        <strong style='color:#e74c3c; font-size:18px;'>{otp}</strong>
                    </p>
                    <p>Mã có hiệu lực trong <strong>5 phút</strong>. Vui lòng không chia sẻ mã này cho bất kỳ ai để đảm bảo an toàn tài khoản.</p>
                    <br/>
                    <p>Trân trọng,</p>
                    <p><strong>Đội ngũ BookingCare</strong></p>
                    <hr style='border:none; border-top:1px solid #ddd;'/>
                    <small style='color:#777;'>Đây là email tự động, vui lòng không trả lời lại email này.</small>
                </div>");
            return Ok(new { success = true, message = "Đã gửi mã OTP đến email của bạn!" });
        }

        //Bước 2: Xác thực mã OTP
        [HttpPost("register-step2")]
        public IActionResult RegisterStep2([FromBody] RegisterStep2Dtos dto)
        {
            var cachedOtp = _otpService.GetOtp(dto.Email);//Lấy mã OTP từ bộ nhớ đệm
            if (cachedOtp == null)
            {
                return BadRequest(new { success = false, message = "Mã OTP đã hết hạn. Vui lòng thử lại!" });
            }
            if (cachedOtp != dto.Otp)
            {
                return BadRequest(new { success = false, message = "Mã OTP không đúng. Vui lòng kiểm tra lại!" });
            }
            return Ok(new { success = true, message = "Xác thực OTP thành công!" });
        }

        //Buớc 3: Hoàn tất đăng ký với thông tin cá nhân
        [HttpPost("register-step3")]
        public async Task<IActionResult> RegisterStep3([FromBody] RegisterStep3Dtos dto)
        {
            if (!_otpService.IsVerifiedOtp(dto.Email))
            {
                return BadRequest(new { success = false, message = "Vui lòng xác thực OTP trước khi hoàn tất đăng ký!" });
            }
            var password = _otpService.GetPassword(dto.Email);//Lấy mật khẩu tạm thời từ bộ nhớ đệm
            if (string.IsNullOrEmpty(password)) //Kiểm tra mật khẩu có tồn tại không
            {
                return BadRequest(new { success = false, message = "Mật khẩu đã hết hạn. Vui lòng thử lại!" });
            }

            //Tạo đối tượng ApplicationUser mới
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
            };

            //Tạo tài khoản người dùng
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                _otpService.RemoveOtp(dto.Email); //Xóa mã OTP khỏi bộ nhớ đệm
                _otpService.RemovePassword(dto.Email); //Xóa mật khẩu tạm thời khỏi bộ nhớ đệm
                await _userManager.AddToRoleAsync(user, "Patient"); //Gán vai trò mặc định là Patient
                var patientEntity = new Patient //Tạo bản ghi trong bảng Patient
                {
                    UserId = user.Id,
                    MedicalHistory = "Chưa có tiền sử bệnh"
                };
                _dbContext.Patients.Add(patientEntity);
                await _dbContext.SaveChangesAsync();

                //Gửi email thông báo đăng ký thành công
                await _emailSender.SendEmailAsync(dto.Email, "Chào mừng đến với BookingCare!",
                $@"<div style='font-family:Arial, sans-serif; color:#333; line-height:1.6;'>
                    <h2 style='color:#2a8dc5;'>🎉 Chúc mừng bạn đã đăng ký tài khoản thành công!</h2>
                    <p>Xin chào <strong>{dto.FullName}</strong>,</p>
                    <p>Bạn đã đăng ký tài khoản trên hệ thống <strong>BookingCare</strong> thành công.</p>
                    <p>Bây giờ bạn có thể đăng nhập để đặt lịch khám, xem lịch sử khám bệnh và nhiều tiện ích khác.</p>
                    <br/>
                    <p>Chúc bạn có những trải nghiệm tốt nhất cùng <strong>BookingCare</strong>!</p>
                    <hr style='border:none; border-top:1px solid #ddd;'/>
                    <small style='color:#777;'>Đây là email tự động, vui lòng không trả lời lại email này.</small>
                </div>");
                return Ok(new { success = true, message = "Đăng ký tài khoản thành công!" });
            }
            return BadRequest(new { success = false, message = "Đăng ký tài khoản thất bại. Vui lòng thử lại!" });
        }
    }
}
