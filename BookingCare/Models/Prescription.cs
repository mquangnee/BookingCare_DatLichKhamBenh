namespace BookingCare.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public string Diagnosis { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        //Quan hệ 1-N với bảng Appointment
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        //Quan hệ N-N với bảng Medicine qua bảng Prescription_Detail
        public ICollection<Prescription_Detail> Prescription_Details { get; set; }
    }
}
