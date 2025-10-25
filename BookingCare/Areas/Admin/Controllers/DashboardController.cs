using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _dbContext;
        public DashboardController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            int totalDoctors = (from u in _dbContext.Users
                                     join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                                     join r in _dbContext.Roles on ur.RoleId equals r.Id
                                     where r.Name == "Doctor" && u.LockoutEnd == null
                                     select u).Count();
            int totalPatients = (from u in _dbContext.Users
                                    join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                                    join r in _dbContext.Roles on ur.RoleId equals r.Id
                                    where r.Name == "Patient" && u.LockoutEnd == null
                                    select u).Count();
            ViewBag.TotalDoctors = totalDoctors;
            ViewBag.TotalPatients = totalPatients;
            return View();
        }
        //-----Thống kê số lượng cuộc hẹn trong ngày-----
        [HttpGet]
        public IActionResult GetTodayAppointmentsCount()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            int count = _dbContext.Appointments.Count(a => a.AppointmentDate == today && a.Status != "Đã hủy");
            return Json(count);
        }
        //-----Thống kê số lượng cuộc hẹn đã hủy trong ngày-----
        [HttpGet]
        public IActionResult GetTodayAppointmentsCanceledCount()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            int count = _dbContext.Appointments.Count(a => a.AppointmentDate == today && a.Status == "Đã hủy");
            return Json(count);
        }
        //[HttpGet]
        //public IActionResult 
    }
}
