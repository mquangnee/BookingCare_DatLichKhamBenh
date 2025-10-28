using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.ViewModel
{
    public class ResetPassViewModel
    {
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu mới")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        [DisplayName("Xác nhận mật khẩu mới")]
        public string ConfirmedNewPassword { get; set; }
    }
}
