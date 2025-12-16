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
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //Thông tin cơ bản
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(15)]
        public string Gender { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        //Thông tin chuyên môn
        [Required]
        [StringLength(50)]
        public string Degree { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int YearsOfExp { get; set; }

        [Required]
        public int SpecialtyId { get; set; }

        [Required]
        public int RoomId { get; set; }
    }

    //Dto cập nhật thông tin bác sĩ
    public class UpdateDoctor
    {
        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        //Thông tin chuyên môn
        [Required]
        [StringLength(50)]
        public string Degree { get; set; }

        [Range(1, int.MaxValue)]
        [Required]
        public int YearsOfExp { get; set; }

        [Required]
        public int SpecialtyId { get; set; }

        [Required]
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
