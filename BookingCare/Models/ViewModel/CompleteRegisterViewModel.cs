using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.ViewModel
{
    public class CompleteRegisterViewModel
    {
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(50, ErrorMessage = "{0} không được quá 50 ký tự!")]
        [DisplayName("Họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DisplayName("Giới tính")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DisplayName("Ngày sinh")]
        public DateOnly BirthOfDate { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(100, ErrorMessage = "{0} không được quá 100 ký tự!")]
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
    }
}
