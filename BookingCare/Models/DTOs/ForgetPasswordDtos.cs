using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class ForgetPasswordStep1Dtos
    {
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmedNewPassword { get; set; }
    }

    public class ForgetPasswordStep2Dtos
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public string Otp { get; set; }
    }
}
