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

        //****Khóa/Mở khóa tài khoản****
        string GetAccountLockedEmailBody(string fullname, string role);
        string GetAccountUnlockedEmailBody(string fullname, string role);
        
        //****Thêm tài khoản bác sĩ****
        string GetDoctorAccountCreatedEmailBody(string fullname, string email);

        //****Chỉnh sửa thông tin bác sĩ****
        string GetDoctorInfoUpdatedEmailBody(string fullname, string email);
    }
}
