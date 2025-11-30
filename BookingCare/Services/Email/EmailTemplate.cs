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

        //****Gửi lại OTP(quên mật khẩu)****
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

        //****Đặt lại mật khẩu thành công****
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

        //****Đăng ký****
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

        //****Gửi lại OPT(đăng ký)****
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
        //****Đăng ký thành công****
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

        //****Khóa tài khoản****
        public string GetAccountLockedEmailBody(string fullName, string role)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Thông báo khóa tài khoản BookingCare</h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>Tài khoản <strong>{role}</strong> của bạn trên hệ thống <strong>BookingCare</strong> đã bị <span style='color: #e63946; font-weight: bold;'>khóa tạm thời</span>.</p>
                        <p>Nếu bạn cho rằng đây là nhầm lẫn, vui lòng liên hệ với <strong>đội ngũ hỗ trợ BookingCare</strong> để được xem xét và mở khóa tài khoản.</p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>";
        }

        //****Mở khóa tài khoản****
        public string GetAccountUnlockedEmailBody(string fullName, string role)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Thông báo mở khóa tài khoản BookingCare</h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>Tài khoản <strong>{role}</strong> của bạn trên hệ thống <strong>BookingCare</strong> đã được <span style='color: #2a9d8f; font-weight: bold;'>mở khóa</span> và có thể đăng nhập lại bình thường.</p>
                        <p>Bạn có thể tiếp tục sử dụng các dịch vụ của BookingCare như:</p>
                        <ul style='color: #555; line-height: 1.6;'>
                            <li>Đặt lịch khám / Quản lý lịch khám (đối với bệnh nhân).</li>
                            <li>Quản lý ca khám và hồ sơ bệnh nhân (đối với bác sĩ).</li>
                        </ul>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Chúc bạn có trải nghiệm tốt cùng <strong style='color: #45c3d2;'>BookingCare</strong>!<br/>
                            <strong>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>";
        }

        //****Thêm tài khoản bác sĩ****
        public string GetDoctorAccountCreatedEmailBody(string fullName, string email)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 550px; margin: auto; background: #fff; border-radius: 10px; 
                                box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>
                            Tài khoản bác sĩ BookingCare đã được tạo
                        </h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>Tài khoản bác sĩ của bạn trên hệ thống <strong>BookingCare</strong> đã được khởi tạo thành công.</p>
                        <p>Thông tin đăng nhập của bạn như sau:</p>
                        <div style='background-color: #f1f9ff; border-left: 4px solid #45c3d2; padding: 10px 15px; margin: 15px 0;'>
                            <p style='margin: 5px 0;'><strong>Email:</strong> {email}</p>
                            <p style='margin: 5px 0;'><strong>Mật khẩu mặc định:</strong> <span style='color:#e63946;'>Abcd@123</span></p>
                        </div>
                        <p>Vì lý do bảo mật, bạn vui lòng đăng nhập và <strong>đổi mật khẩu ngay sau khi đăng nhập lần đầu</strong>.</p>
                        <hr style='margin: 25px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>";
        }

        //****Chỉnh sửa thông tin bác sĩ****
        public string GetDoctorInfoUpdatedEmailBody(string fullName, string email)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 550px; margin: auto; background: #fff; border-radius: 10px;
                                box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>
                            Thông tin bác sĩ đã được cập nhật
                        </h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>Thông tin tài khoản của bạn trên hệ thống <strong>BookingCare</strong> đã được cập nhật bởi bộ phận quản trị.</p>
                        <p>
                            Nếu bạn nhận thấy có điểm nào chưa chính xác hoặc cần được điều chỉnh thêm, 
                            vui lòng liên hệ với bộ phận hỗ trợ của chúng tôi để được trợ giúp.
                        </p>
                        <hr style='margin: 25px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>";
        }

        //****Đặt lịch khám thành công****
        public string getBookingSuccessEmailBody(string fullName, string doctorName, string specialtyName, DateOnly appointmentDate, string appointmentTime, string room, int bookingCode)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 550px; margin: auto; background: #fff; border-radius: 10px; 
                                box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>
                            Xác nhận đặt lịch khám thành công
                        </h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>Bạn đã <strong>đặt lịch khám bệnh thành công</strong> trên hệ thống <strong>BookingCare</strong>. 
                           Thông tin chi tiết lịch khám của bạn như sau:</p>
                        <div style='background-color: #f1f9ff; border-left: 4px solid #45c3d2; padding: 15px; margin: 20px 0;'>
                            <p style='margin: 4px 0;'><strong>Mã lịch khám:</strong> {bookingCode}</p>
                            <p style='margin: 4px 0;'><strong>Bác sĩ:</strong> {doctorName}</p>
                            <p style='margin: 4px 0;'><strong>Chuyên khoa:</strong> {specialtyName}</p>
                            <p style='margin: 4px 0;'><strong>Thời gian:</strong> {appointmentTime}, ngày {appointmentDate.ToString("dd/MM/yyyy")}</p>
                            <p style='margin: 4px 0;'><strong>Địa điểm:</strong> {room}</p>
                        </div>
                        <p style='margin-top: 10px;'>
                            Bạn vui lòng đến trước giờ khám <strong>10–15 phút</strong> để làm thủ tục tiếp đón và chuẩn bị khám.
                        </p>
                        <p style='margin-top: 10px;'>
                            Trong trường hợp bạn không thể đến đúng hẹn, vui lòng liên hệ với chúng tôi hoặc hủy lịch trên hệ thống 
                            trước giờ khám để được hỗ trợ tốt nhất.
                        </p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của <strong style='color: #45c3d2;'>BookingCare</strong>.<br/>
                            Chúc bạn luôn mạnh khỏe!
                        </p>
                    </div>
                </div>";
        }

        //****Chỉnh sửa thông tin cá nhân bệnh nhân****
        public string GetPatientUpdatedInfoEmailBody(string fullName, string email)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 550px; margin: auto; background: #fff; border-radius: 10px;
                                box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>
                            Cập nhật thông tin cá nhân thành công
                        </h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>
                            Bạn đã cập nhật thông tin cá nhân của mình trên hệ thống 
                            <strong>BookingCare</strong> thành công.
                        </p>
                        <p>
                            Việc cập nhật thông tin giúp chúng tôi phục vụ bạn tốt hơn trong quá trình 
                            đặt lịch và khám chữa bệnh.
                        </p>
                        <p>
                            Nếu bạn không thực hiện thao tác này hoặc có điều gì bất thường, 
                            vui lòng liên hệ với bộ phận hỗ trợ ngay lập tức để được trợ giúp.
                        </p>
                        <hr style='margin: 25px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>";
        }

        //****Thông báo lịch hẹn trong ngày****
        public string GetDailyAppointmentSummaryEmailBody(string fullName, string appointmentsHtml)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 550px; margin: auto; background: #fff; border-radius: 10px;
                                box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>
                            Lịch hẹn trong ngày của bạn
                        </h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>
                            Đây là danh sách các lịch hẹn của bạn trong ngày hôm nay trên hệ thống 
                            <strong>BookingCare</strong>. Vui lòng kiểm tra chi tiết và sắp xếp thời gian phù hợp.
                        </p>
                        <div style='margin: 20px 0;'>
                            {appointmentsHtml}
                        </div>
                        <p>
                            Nếu bạn cần thay đổi hoặc hủy lịch hẹn, vui lòng truy cập trang lịch hẹn của mình
                            hoặc liên hệ bộ phận hỗ trợ.
                        </p>
                        <hr style='margin: 25px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>";
        }

        //****Nhắc lịch hẹn sắp diễn ra****
        public string GetAppointmentReminderEmailBody(string fullName, string appointmentTime, string doctorName, string roomName)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 550px; margin: auto; background: #fff; border-radius: 10px;
                                box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>
                            Nhắc lịch hẹn sắp diễn ra
                        </h2>
                        <p>Xin chào <strong>{fullName}</strong>,</p>
                        <p>
                            Đây là email nhắc nhở rằng bạn có một lịch hẹn sẽ diễn ra trong 
                            <strong>30 phút nữa</strong>.
                        </p>
                        <div style='margin: 20px 0; padding: 15px; background: #f1f9ff;
                                    border-left: 4px solid #45c3d2; border-radius: 5px;'>
                            <p><strong>Thời gian:</strong> {appointmentTime}</p>
                            <p><strong>Bác sĩ:</strong> {doctorName}</p>
                            <p><strong>Phòng khám:</strong> {roomName}</p>
                        </div>
                        <p>
                            Vui lòng đến đúng giờ để quá trình khám diễn ra thuận lợi. 
                            Nếu bạn không thể tham gia, hãy hủy hoặc đổi lịch sớm để hệ thống sắp xếp lại.
                        </p>
                        <hr style='margin: 25px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>";
        }

        //****Đổi mật khẩu****
        public string getChangePasswordOtpEmailBody(string fullName, string otp)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                        <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                            <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Xác thực đổi mật khẩu BookingCare</h2>
                            <p>Xin chào <strong>{fullName}</strong>,</p>
                            <p>Bạn đã yêu cầu <strong>đổi mật khẩu</strong> cho tài khoản BookingCare của mình.</p>
                            <p style='margin-top: 15px;'>
                                Mã OTP của bạn là:
                            </p>
                            <p style='font-size: 22px; text-align: center; margin: 15px 0;'>
                                <strong style='color: #e63946; letter-spacing: 2px;'>{otp}</strong>
                            </p>
                            <p style='text-align: center; color: #555;'>Mã có hiệu lực trong <strong>5 phút</strong>.</p>
                            <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                            <p style='font-size: 14px; color: #666; text-align: center;'>
                                Nếu bạn không yêu cầu đổi mật khẩu, vui lòng bỏ qua email này.<br/>
                                Trân trọng,<br/>
                                <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                            </p>
                        </div>
                    </div>";
        }

        //****Đổi mật khẩu thành công****
        public string getChangePasswordSuccessEmailBody(string fullName)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                    <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Đổi mật khẩu thành công</h2>
                    <p>Xin chào <strong>{fullName}</strong>,</p>
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

        //****Gửi lại OTP(đổi mật khẩu)****
        public string getResendChangePassOtpEmailBody(string otp)
        {
            return $@"<div style='font-family: Arial, sans-serif; background-color: #f9f9f9; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: #fff; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); padding: 25px;'>
                        <h2 style='color: #45c3d2; text-align: center; margin-bottom: 20px;'>Đổi mật khẩu BookingCare</h2>
                        <p>Xin chào,</p>
                        <p>Bạn đã yêu cầu <strong>đổi mật khẩu</strong> cho tài khoản BookingCare của mình.</p>
                        <p style='margin-top: 15px;'>
                            Mã OTP của bạn là:
                        </p>
                        <p style='font-size: 22px; text-align: center; margin: 15px 0;'>
                            <strong style='color: #e63946; letter-spacing: 2px;'>{otp}</strong>
                        </p>
                        <p style='text-align: center; color: #555;'>Mã có hiệu lực trong <strong>5 phút</strong>.</p>
                        <hr style='margin: 20px 0; border: none; border-top: 1px solid #eee;' />
                        <p style='font-size: 14px; color: #666; text-align: center;'>
                            Nếu bạn không yêu cầu đổi mật khẩu, vui lòng bỏ qua email này.<br/>
                            Trân trọng,<br/>
                            <strong style='color: #45c3d2;'>Đội ngũ BookingCare</strong>
                        </p>
                    </div>
                </div>";
        }
    }
}
