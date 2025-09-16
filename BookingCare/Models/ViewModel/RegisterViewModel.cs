using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        public string ConfirmedPassword { get; set; }
    }
}
