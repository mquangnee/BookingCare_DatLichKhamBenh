using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using BookingCare.Services;
using BookingCare.Services.Email;
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
        private readonly IEmailTemplate _emailTemplate;
        private readonly OtpService _otpService;
        public AuthApiController(UserManager<ApplicationUser> userManager, DataContext dbContext, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IEmailTemplate emailTemplate, OtpService otpService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _emailTemplate = emailTemplate;
            _otpService = otpService;
        }

        //====QUÊN MẬT KHẨU====//
        //Bước 1: Nhập email đăng ký, mật khẩu mới và xác nhận mật khẩu mới
        [HttpPost("forgotPass-step1")]
        public async Task<IActionResult> ForgotPasswordStep1([FromBody] ForgetPasswordStep1Dtos dto)
        {
            //Kiểm tra email tồn tại
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest(new { success = false, message = "Email không còn tồn tại trong hệ thống!" });
            }

            //Tạo mã OTP và lưu vào bộ nhớ đệm
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(dto.Email, otp);

            //Lưu mật khẩu mới tạm thời vào bộ nhớ đệm
            _otpService.SetPassword(dto.Email, dto.NewPassword);

            //Nội dung email
            var body = _emailTemplate.getForgotPassOtpEmailBody(otp);

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Xác nhận đặt lại mật khẩu - BookingCare", body));
            return Ok(new { success = true, message = "Đã gửi mã OTP đến email của bạn!" });
        }

        //Bước 2: Xác thực mã OTP
        [HttpPost("forgotPass-step2")]
        public async Task<IActionResult> ForgotPasswordStep2([FromBody] ForgetPasswordStep2Dtos dto)
        {
            string? cachedOtp = _otpService.GetOtp(dto.Email);
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
            var password = _otpService.GetPassword(dto.Email);

            //Kiểm tra mật khẩu mới có còn tồn tại không
            if (string.IsNullOrEmpty(password)) 
            {
                return BadRequest(new { success = false, message = "Mật khẩu mới đã hết hạn. Vui lòng thử lại!" });
            }

            //Kiểm tra email tồn tại
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return BadRequest(new { success = false, message = "Email không còn tồn tại trong hệ thống!" });
            }

            //Xóa mật khẩu cũ và thêm mật khẩu mới
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, password);
            if (result.Succeeded)
            {
                _otpService.RemoveOtp(dto.Email); //Xóa mã OTP khỏi bộ nhớ đệm
                _otpService.RemovePassword(dto.Email); //Xóa mật khẩu tạm thời khỏi bộ nhớ đệm

                //Nội dung email
                var body = _emailTemplate.getSuccessForgotPassEmailBody();

                //Gửi email thông báo đổi mật khẩu thành công
                _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Thông báo đổi mật khẩu - BookingCare", body));
                return Ok(new { success = true, message = "Đổi mật khẩu thành công!" });
            }
            return BadRequest(new { success = false, message = "Đổi mật khẩu không thành công!" });
        }
        
        //====ĐĂNG NHẬP TÀI KHOẢN====//
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDtos dto)
        {
            //Xác thực thông tin đăng nhập
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                //Lấy thông tin người dùng
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return BadRequest(new { success = false, message = "Đăng nhập không thành công! Vui lòng kiểm tra lại email và mật khẩu." });
                }

                //Lấy vai trò của người dùng
                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new { roles, success = true, message = "Đăng nhập thành công!"});
            }
            return BadRequest(new { success = false, message = "Đăng nhập không thành công! Vui lòng kiểm tra lại email và mật khẩu." });
        }

        //====ĐĂNG KÝ TÀI KHOẢN 3 BƯỚC====//
        //Bước 1: Đăng ký tài khoản bằng email và mật khẩu
        [HttpPost("register-step1")]
        public async Task<IActionResult> RegisterStep1([FromBody] RegisterStep1Dtos dto)
        {
            var result = await _userManager.FindByEmailAsync(dto.Email);
            if (result != null)//Kiểm tra email đã tồn tại chưa
            {
                return BadRequest(new { success = false, message = "Email đã được sử dụng" });
            }
            //Tạo và gửi mã OTP qua email
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(dto.Email, otp);

            //Lưu mật khẩu tạm thời vào bộ nhớ đệm
            _otpService.SetPassword(dto.Email, dto.Password);

            //Nội dung email
            var body = _emailTemplate.getRegisterOtpEmailBody(otp);

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Xác nhận mã OTP - BookingCare", body));
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

            //Đánh dấu đã xác thực OTP thành công
            _otpService.SetOtpFlag(dto.Email); 
            return Ok(new { success = true, message = "Xác thực OTP thành công!" });
        }

        //Buớc 3: Hoàn tất đăng ký với thông tin cá nhân
        [HttpPost("register-step3")]
        public async Task<IActionResult> RegisterStep3([FromBody] RegisterStep3Dtos dto)
        {
            if (!_otpService.IsVerifiedOtp(dto.Email))//Kiểm tra đã xác thực OTP chưa
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

                //Nội dung email
                var body = _emailTemplate.getSuccessRegisterEmailBody(dto.FullName);

                //Gửi email thông báo đăng ký thành công
                _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Đăng ký tài khoản BookingCare", body)); 
                return Ok(new { success = true, message = "Đăng ký tài khoản thành công!" });
            }
            return BadRequest(new { success = false, message = "Đăng ký tài khoản thất bại. Vui lòng thử lại!" });
        }

        //====GỬI LẠI MÃ OTP====//
        //Gửi lại mã đăng ký 
        [HttpPost("register-resend-otp")]
        public IActionResult RegisterResendOtp([FromBody] RegisterStep2Dtos dto)
        {
            //Tạo và gửi mã OTP qua email
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(dto.Email, otp);

            //Nội dung email
            var body = _emailTemplate.getResendRegisterOtpEmailBody(otp);

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Gửi lại mã OTP - BookingCare", body));
            return Ok();
        }

        [HttpPost("forgotPass-resend-otp")]
        public IActionResult ForgotPassResendOtp([FromBody] ForgetPasswordStep2Dtos dto)
        {
            //Tạo và gửi mã OTP qua email
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(dto.Email, otp);

            //Nội dung email
            var body = _emailTemplate.getResendForgotPassOtpEmailBody(otp);

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Gửi lại mã OTP - BookingCare", body));
            return Ok();
        }
    }
}
