using System.ComponentModel.DataAnnotations;

namespace BookingCare.Models.ViewModel
{
    public class CompleteRegisterViewModel
    {
        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(50, ErrorMessage = "{0} không được quá 50 ký tự!")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        public DateTime BirthOfDate { get; set; }

        [Required(ErrorMessage = "{0} vui lòng không để trống!")]
        [StringLength(100, ErrorMessage = "{0} không được quá 100 ký tự!")]
        public string Address { get; set; }

        //public string Otp { get; set; }
    }
}
