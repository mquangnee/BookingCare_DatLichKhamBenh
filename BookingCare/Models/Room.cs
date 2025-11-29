using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int Capacity { get; set; }

        // Quan hệ 1-N với bảng Doctor
        public ICollection<Doctor> Doctors { get; set; }
    }
}
