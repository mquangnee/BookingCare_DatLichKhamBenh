using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class UserDtos
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class UserManagementDtos2
    {
    }

}
