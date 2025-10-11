using System.ComponentModel;
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
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        [DisplayName("Xác nhận mật khẩu")]
        public string ConfirmedPassword { get; set; }
    }
}
