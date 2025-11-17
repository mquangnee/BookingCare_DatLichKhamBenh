using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class ForgetPasswordStep1Dtos
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmedNewPassword { get; set; }
    }

    public class ForgetPasswordStep2Dtos
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Otp { get; set; }
    }
}
