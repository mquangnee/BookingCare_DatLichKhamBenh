using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class ChangePasswordStep1Dtos
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmNewPassword { get; set; }
    }
    public class ChangePasswordStep2Dtos 
    {
        [Required]
        public string Otp { get; set; }
    }
}
