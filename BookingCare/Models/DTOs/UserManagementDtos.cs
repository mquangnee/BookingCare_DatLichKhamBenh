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
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public Doctor Doctors { get; set; }
        public Specialty Specialties { get; set; }
        public Room Rooms { get; set; }
    }

    //Dto lấy thông tin chi tiết bệnh nhân
    public class PatientInfoDtos
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public Patient Patients { get; set; }
    }
}
