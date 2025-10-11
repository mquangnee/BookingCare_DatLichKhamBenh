using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DisplayName("Tên đăng nhập")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
