using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Data.Seed
{
    public static class SeedAppointment
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            // Nếu đã có lịch thì không seed nữa
            //if (await dbContext.Appointments.AnyAsync())
            //    return;

            // ===== LẤY USER ROLE PATIENT =====
            var patientUsers = await dbContext.Users
                .Join(dbContext.UserRoles, u => u.Id, ur => ur.UserId,
                      (u, ur) => new { u, ur })
                .Join(dbContext.Roles, x => x.ur.RoleId, r => r.Id,
                      (x, r) => new { x.u, r })
                .Where(x => x.r.Name == "Patient")
                .Select(x => x.u)
                .ToListAsync();

            // ===== LẤY DANH SÁCH BÁC SĨ =====
            var doctors = await dbContext.Doctors.ToListAsync();

            if (!patientUsers.Any() || !doctors.Any())
                return;

            // ===== ĐẢM BẢO MỖI USER CÓ PATIENT =====
            foreach (var user in patientUsers)
            {
                if (!await dbContext.Patients.AnyAsync(p => p.UserId == user.Id))
                {
                    dbContext.Patients.Add(new Patient
                    {
                        UserId = user.Id,
                        MedicalHistory = "Chưa có tiền sử bệnh án"
                    });
                }
            }

            await dbContext.SaveChangesAsync();

            var patients = await dbContext.Patients.ToListAsync();
            var random = new Random();

            // ===== SLOT GIỜ KHÁM =====
            var timeSlots = new List<string>
            {
                "07:00-07:30",
                "07:30-08:00",
                "08:00-08:30",
                "08:30-09:00",
                "09:00-09:30",
                "09:30-10:00",
                "10:00-10:30",
                "10:30-11:00",
                "13:00-13:30",
                "13:30-14:00",
                "14:00-14:30",
                "14:30-15:00",
                "15:00-15:30",
                "15:30-16:00",
                "16:00-16:30",
                "16:30-17:00"
            };

            var reasons = new List<string>
            {
                "Khám tổng quát",
                "Đau đầu kéo dài",
                "Sốt cao",
                "Đau bụng",
                "Ho kéo dài",
                "Mệt mỏi",
                "Chóng mặt",
                "Tái khám định kỳ"
            };

            var appointments = new List<Appointment>();

            // ===== KHOẢNG NGÀY =====
            var startDate = new DateOnly(2025, 12, 27);
            var endDate = new DateOnly(2026, 1, 5);
            var totalDays = endDate.DayNumber - startDate.DayNumber + 1;

            var today = DateOnly.FromDateTime(DateTime.Now);

            // ===== TẠO 100 LỊCH =====
            while (appointments.Count < 100)
            {
                var patient = patients[random.Next(patients.Count)];
                var doctor = doctors[random.Next(doctors.Count)];

                var date = startDate.AddDays(random.Next(totalDays));
                var timeSlot = timeSlots[random.Next(timeSlots.Count)];

                // ❌ Tránh trùng lịch bác sĩ
                bool isDuplicate = appointments.Any(a =>
                    a.DoctorId == doctor.Id &&
                    a.AppointmentDate == date &&
                    a.AppointmentTime == timeSlot
                );

                if (isDuplicate) continue;

                // ===== LOGIC TRẠNG THÁI =====
                string status;

                if (date < today)
                {
                    status = random.Next(100) < 80
                        ? "Hoàn thành"
                        : "Đã hủy";
                }
                else if (date == today)
                {
                    status = random.Next(100) < 50
                        ? "Đang khám"
                        : "Chờ khám";
                }
                else
                {
                    status = random.Next(100) < 85
                        ? "Chờ khám"
                        : "Đã hủy";
                }

                appointments.Add(new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = doctor.Id,
                    AppointmentDate = date,
                    AppointmentTime = timeSlot,
                    ReasonForVisit = reasons[random.Next(reasons.Count)],
                    Status = status,
                    CreatedAt = DateTime.Now
                });
            }

            await dbContext.Appointments.AddRangeAsync(appointments);
            await dbContext.SaveChangesAsync();
        }
    }
}
