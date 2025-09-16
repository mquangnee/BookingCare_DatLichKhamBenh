using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
