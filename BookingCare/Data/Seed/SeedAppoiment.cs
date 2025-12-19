using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Data.Seed
{
    public static class SeedAppointment
    {
        public static async Task SeedAsync(DataContext dbContext)
        {
            // ✅ Nếu đã có lịch thì không seed nữa
            if (await dbContext.Appointments.AnyAsync())
                return;

            // ✅ Lấy danh sách USER có role Patient
            var patientUsers = await dbContext.Users
                .Join(dbContext.UserRoles, u => u.Id, ur => ur.UserId,
                      (u, ur) => new { u, ur })
                .Join(dbContext.Roles, x => x.ur.RoleId, r => r.Id,
                      (x, r) => new { x.u, r })
                .Where(x => x.r.Name == "Patient")
                .Select(x => x.u)
                .ToListAsync();

            // ✅ Lấy danh sách bác sĩ
            var doctors = await dbContext.Doctors.ToListAsync();

            if (!patientUsers.Any() || !doctors.Any())
                return;

            // ✅ ĐẢM BẢO mỗi user Patient có 1 bản ghi trong bảng Patients
            foreach (var user in patientUsers)
            {
                if (!await dbContext.Patients.AnyAsync(p => p.UserId == user.Id))
                {
                    dbContext.Patients.Add(new Patient
                    {
                        UserId = user.Id
                    });
                }
            }

            await dbContext.SaveChangesAsync();

            // ✅ LẤY LẠI PATIENT SAU KHI ĐÃ CÓ ĐỦ
            var patients = await dbContext.Patients.ToListAsync();

            var random = new Random();

            var timeSlots = new List<string>
            {
                "08:00",
                "09:00",
                "10:00",
                "13:30",
                "14:30",
                "15:30"
            };

            var reasons = new List<string>
            {
                "Đau đầu",
                "Sốt cao",
                "Đau bụng",
                "Ho kéo dài",
                "Đau họng",
                "Mệt mỏi",
                "Buồn nôn",
                "Chóng mặt"
            };

            var statusList = new List<string>
            {
                "Chờ khám",
                "Đã xác nhận",
                "Đã hủy"
            };

            var appointments = new List<Appointment>();

            for (int i = 0; i < 10; i++)
            {
                var randomPatient = patients[random.Next(patients.Count)];
                var randomDoctor = doctors[random.Next(doctors.Count)];

                var appointment = new Appointment
                {
                    PatientId = randomPatient.Id,
                    DoctorId = randomDoctor.Id,
                    AppointmentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(random.Next(1, 7))),
                    AppointmentTime = timeSlots[random.Next(timeSlots.Count)],
                    ReasonForVisit = reasons[random.Next(reasons.Count)],
                    Status = statusList[random.Next(statusList.Count)],
                    CreatedAt = DateTime.Now
                };

                appointments.Add(appointment);
            }

            await dbContext.Appointments.AddRangeAsync(appointments);
            await dbContext.SaveChangesAsync();
        }
    }
}
