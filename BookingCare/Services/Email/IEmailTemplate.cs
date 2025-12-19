using BookingCare.Models.DTOs;

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

        //****Đặt lịch khám bệnh thành công****
        string getBookingSuccessEmailBody(string fullName, string doctorName, string specialtyName, DateOnly appointmentDate, string appointmentTime, string room, int bookingCode);

        //****Cập nhật thông tin cá nhân bệnh nhân****
        string GetPatientUpdatedInfoEmailBody(string fullName, string email);

        //****Thông báo lịch hẹn trong ngày****
        string GetDailyAppointmentSummaryEmailBody(string fullName, string appointmentsHtml);

        //****Nhắc nhở lịch hẹn sắp tới****
        string GetAppointmentReminderEmailBody(string fullName, string appointmentTime, string doctorName, string roomName);

        //****Đôi mật khẩu****
        string getChangePasswordOtpEmailBody(string fullName, string otp);
        string getChangePasswordSuccessEmailBody(string fullName);
        string getResendChangePassOtpEmailBody(string otp);
        //****Gửi kết quả khám bệnh****
        string GeneratePrescriptionTable(List<MedPrescriptionDtos> medicines);
        string GetMedicalReportEmailBody(string patientName, string diagnosis, string instructions, List<MedPrescriptionDtos> medicines);
    }
}
