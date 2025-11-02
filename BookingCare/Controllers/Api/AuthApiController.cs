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

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Xác nhận đặt lại mật khẩu - BookingCare",
                $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Đặt lại mật khẩu BookingCare</h2>
                        <p>Xin chào,</p>
                        <p>Bạn đã yêu cầu <strong>đặt lại mật khẩu</strong> cho tài khoản BookingCare của mình.</p>
                        <p style='margin-top: 15px;'>
                            Mã OTP của bạn là:
                        </p>
                        <p style='font-size: 22px; text-align: center; margin: 15px 0;'>
                            <strong style='color: #e63946; letter-spacing: 2px;'>{otp}</strong>
                        </p>
                        <p style='text-align: center; color: #555;'>Mã có hiệu lực trong <strong>5 phút</strong>.</p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.<br/>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>"));
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

                //Gửi email thông báo đổi mật khẩu thành công
                _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Thông báo đổi mật khẩu - BookingCare",
                    $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                        <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                            <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Đổi mật khẩu thành công</h2>
                            <p>Xin chào,</p>
                            <p>Bạn đã <strong>đổi mật khẩu</strong> tài khoản BookingCare thành công.</p>
                            <p style='color: #555;'>
                                Bạn có thể đăng nhập lại bằng mật khẩu mới để tiếp tục sử dụng các dịch vụ của BookingCare.
                            </p>
                            <hr style='margin: 25px 0; border: none; border-top: 1px solid #eee;' />
                            <p style='font-size: 14px; color: #666; text-align: center;'>
                                Trân trọng,<br/>
                                <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                            </p>
                        </div>
                    </div>"));
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

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Xác nhận mã OTP - BookingCare",
                $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Xác thức tài khoản BookingCare</h2>
                        <p>Xin chào,</p>
                        <p>Bạn đang thực hiện xác thực tài khoản trên hệ thống <strong>BookingCare</strong>.</p>
                        <p style='margin-top: 15px;'>
                            Mã OTP của bạn là:
                        </p>
                        <p style='font-size: 22px; text-align: center; margin: 15px 0;'>
                            <strong style='color: #e63946; letter-spacing: 2px;'>{otp}</strong>
                        </p>
                        <p style='text-align: center; color: #555;'>Mã có hiệu lực trong <strong>5 phút</strong>.</p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Nếu bạn không yêu cầu xác thực tài khoản, vui lòng bỏ qua email này.<br/>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>"));
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

                //Gửi email thông báo đăng ký thành công
                _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Đăng ký tài khoản BookingCare",
                    $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Chào mừng đến với BookingCare</h2>
                        <p>Xin chào <strong>{dto.FullName}</strong>,</p>
                        <p>Bạn đã đăng ký tài khoản trên hệ thống <strong>BookingCare</strong> thành công.</p>
                        <p>Chúc bạn có những trải nghiệm tốt nhất cùng <strong>BookingCare</strong>!</p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>")); 
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

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Gửi lại mã OTP - BookingCare",
                $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Xác thực tài khoản BookingCare</h2>
                        <p>Xin chào,</p>
                        <p>Bạn đang thực hiện xác thực tài khoản trên hệ thống <strong>BookingCare</strong>.</p>
                        <p style='margin-top: 15px;'>
                            Mã OTP của bạn là:
                        </p>
                        <p style='font-size: 22px; text-align: center; margin: 15px 0;'>
                            <strong style='color: #e63946; letter-spacing: 2px;'>{otp}</strong>
                        </p>
                        <p style='text-align: center; color: #555;'>Mã có hiệu lực trong <strong>5 phút</strong>.</p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Nếu bạn không yêu cầu xác thực tài khoản, vui lòng bỏ qua email này.<br/>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>"));
            return Ok();
        }

        [HttpPost("forgotPass-resend-otp")]
        public IActionResult ForgotPassResendOtp([FromBody] ForgetPasswordStep2Dtos dto)
        {
            //Tạo và gửi mã OTP qua email
            string otp = new Random().Next(100000, 999999).ToString();
            _otpService.SetOtp(dto.Email, otp);

            //Gửi email xác nhận OTP
            _ = Task.Run(() => _emailSender.SendEmailAsync(dto.Email, "Gửi lại mã OTP - BookingCare",
                $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Đặt lại mật khẩu BookingCare</h2>
                        <p>Xin chào,</p>
                        <p>Bạn đã yêu cầu <strong>đặt lại mật khẩu</strong> cho tài khoản BookingCare của mình.</p>
                        <p style='margin-top: 15px;'>
                            Mã OTP của bạn là:
                        </p>
                        <p style='font-size: 22px; text-align: center; margin: 15px 0;'>
                            <strong style='color: #e63946; letter-spacing: 2px;'>{otp}</strong>
                        </p>
                        <p style='text-align: center; color: #555;'>Mã có hiệu lực trong <strong>5 phút</strong>.</p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.<br/>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>"));
            return Ok();
        }
    }
}
