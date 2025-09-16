using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class SupportStaff
    {
        [Key]
        public int SupportStaffId { get; set; }
        [Required]
        public string Department { get; set; }
        //Khóa ngoài với bảng AspNetUsers
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
