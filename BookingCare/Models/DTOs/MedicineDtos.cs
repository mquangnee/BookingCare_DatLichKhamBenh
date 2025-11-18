using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.DTOs
{
    public class MedicineDtos
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; }

        [Required]
        [StringLength(200)]
        public string Function { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Đang sử dụng";
    }


    public class Add_UpdateMedicineDtos
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; }

        [Required]
        [StringLength(200)]
        public string Function { get; set; }
    }
}
