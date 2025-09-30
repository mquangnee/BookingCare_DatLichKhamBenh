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
        public DbSet<Room> Rooms { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Quan hệ 1-1 Appointment - DoctorAvailableTime
        //    modelBuilder.Entity<Appointment>()
        //        .HasOne(a => a.DoctorAvailableTime)
        //        .WithOne(t => t.Appointment)
        //        .HasForeignKey<Appointment>(a => a.DoctorAvailableTimeId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // Quan hệ N-1 Appointment - Doctor
        //    modelBuilder.Entity<Appointment>()
        //        .HasOne(a => a.Doctor)
        //        .WithMany(d => d.Appointments)
        //        .HasForeignKey(a => a.DoctorId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // Quan hệ N-1 Appointment - Patient
        //    modelBuilder.Entity<Appointment>()
        //        .HasOne(a => a.Patient)
        //        .WithMany(p => p.Appointments)
        //        .HasForeignKey(a => a.PatientId)
        //        .OnDelete(DeleteBehavior.Restrict);
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Appointment - Patient (Cascade)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointment - Doctor (Restrict để tránh multiple cascade paths)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
