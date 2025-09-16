using BookingCare.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Repository
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<SupportStaff> SupportStaffs { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorAvailableTime> DoctorAvailableTimes { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
    }
}
