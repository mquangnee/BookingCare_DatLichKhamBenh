namespace BookingCare.Services.Email
{
    public interface IEmailTemplate
    {
        //====TEMPLATE EMAIL====//
        //****Quên mật khẩu****
        string getForgotPassOtpEmailBody(string otp);
        string getResendForgotPassOtpEmailBody(string otp);
        string getSuccessForgotPassEmailBody();

        //****Đăng ký****
        string getRegisterOtpEmailBody(string otp);
        string getResendRegisterOtpEmailBody(string otp);
        string getSuccessRegisterEmailBody(string fullName);
    }
}
