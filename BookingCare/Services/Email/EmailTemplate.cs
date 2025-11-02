using Humanizer;

namespace BookingCare.Services.Email
{
    public class EmailTemplate : IEmailTemplate
    {
        //====TEMPLATE EMAIL====//
        //****Quên mật khẩu****
        public string getForgotPassOtpEmailBody(string otp)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
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
                </div>";
        }

        public string getResendForgotPassOtpEmailBody(string otp)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
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
                </div>";
        }

        public string getSuccessForgotPassEmailBody()
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
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
                    </div>";
        }

        //****Đăng ký
        public string getRegisterOtpEmailBody(string otp)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
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
                </div>";
        }
        public string getResendRegisterOtpEmailBody(string otp)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
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
                </div>";
        }
        public string getSuccessRegisterEmailBody(string fullName)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Chào mừng đến với BookingCare</h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>Bạn đã đăng ký tài khoản trên hệ thống <strong>BookingCare</strong> thành công.</p>
                        <p>Chúc bạn có những trải nghiệm tốt nhất cùng <strong>BookingCare</strong>!</p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>";
        }
    }
}
