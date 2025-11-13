using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    //Dto hiển thị danh sách
    public class UserDtos
    {
        public string UserId { get; set; }
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTimeOffset? IsLocked { get; set; }
    }

    //Dto lấy thông tin chi tiết bác sĩ
    public class DoctorInfoDtos
    {
        //Thông tin cơ bản
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        
        //Thông tin chuyên ngành
        public int DoctorId { get; set; }
        public string Degree { get; set; }
        public int YearsOfExp { get; set; }

        //Thông tin chuyên khoa
        public string SpecialtyName { get; set; }

        //Thông tin phòng
        public string RoomName { get; set; }
    }

    //DTP thêm tài khoản bác sĩ
    public class AddDoctor
    {
        //Thông tin đăng nhập
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ!")]
        public string Email { get; set; }

        //Thông tin cơ bản
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(100, ErrorMessage = "{0} không quá 100 ký tự!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(15, ErrorMessage = "{0} không quá 15 ký tự!")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(200, ErrorMessage = "{0} không quá 200 ký tự!")]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string PhoneNumber { get; set; }

        //Thông tin chuyên môn
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(50, ErrorMessage = "{0} không quá 50 ký tự!")]
        public string Degree { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số năm kinh nghiệm phải lớn hơn 0")]
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public int YearsOfExp { get; set; }

        [Required(ErrorMessage = "Chuyên khoa vui lòng không để trống!")]
        public int SpecialtyId { get; set; }

        [Required(ErrorMessage = "Phòng vui lòng không để trống!")]
        public int RoomId { get; set; }
    }

    //Dto lấy thông tin chi tiết bệnh nhân
    public class PatientInfoDtos
    {
        //Thông tin cơ bản
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

        //Thông tin tiền sử bệnh
        public int PatientId { get; set; }
        public string MedicalHistory { get; set; }
    }
}
