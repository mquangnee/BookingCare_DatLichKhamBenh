using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    // DTOs cho bước 1 đăng ký
    public class RegisterStep1Dtos
    {
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmedPassword { get; set; }
    }
    // DTOs cho bước 2 đăng ký
    public class RegisterStep2Dtos
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
            
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public string Otp { get; set; }
    }
    // DTOs cho bước 3 đăng ký
    public class RegisterStep3Dtos
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(50, ErrorMessage = "{0} không được quá 50 ký tự!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(100, ErrorMessage = "{0} không được quá 100 ký tự!")]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [Phone(ErrorMessage = "{0} không đúng định dạng!")]
        public string PhoneNumber { get; set; }
    }
}
