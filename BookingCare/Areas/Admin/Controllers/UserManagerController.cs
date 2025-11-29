using BookingCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        public UserManagerController() {}
        
        //---QUẢN LÝ TÀI KHOẢN BỆNH NHÂN---
        //Hiển thị danh sách bệnh nhân
        public IActionResult PatientManager()
        {
            return View();
        }

        //---QUẢN LÝ TÀI KHOẢN BÁC SĨ---
        //Hiển thị danh sách bác sĩ
        public IActionResult DoctorManager()
        {
            return View();
        }
    }
}
