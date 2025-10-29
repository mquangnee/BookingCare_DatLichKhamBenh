using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.ViewModel
{
    public class DoctorViewModel
    {
        //Thông tin đăng nhập
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        [DisplayName("Tên đăng nhập")]
        public string Email { get; set; }
        [DisplayName("Mật khẩu")]
        public string? Password { get; set; }

        //Thông tin cơ bản
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(100, ErrorMessage = "{0} không quá 100 ký tự!")]
        [DisplayName("Họ tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(15, ErrorMessage = "{0} không quá 15 ký tự!")]
        [DisplayName("Giới tính")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DisplayName("Ngày sinh")]
        public DateOnly BirthOfDate { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(200, ErrorMessage = "{0} không quá 200 ký tự!")]
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        [DisplayName("Số điện thoại")]
        public string PhoneNumber { get; set; }

        //Thông tin chuyên môn
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(50, ErrorMessage = "{0} không quá 50 ký tự!")]
        [DisplayName("Bằng cấp")]
        public string Degree { get; set; }
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [DisplayName("Số năm kinh nghiệm")]
        public int YearsOfExp { get; set; }
        [Required(ErrorMessage = "Chuyên khoa vui lòng không để trống!")]
        public int SpecialtyId { get; set; }
        [Required(ErrorMessage = "Phòng vui lòng không để trống!")]
        public int RoomId { get; set; }
    }
}
