using BookingCare.Models;
using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserManagerController(DataContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<IActionResult> PatientManager()
        {
            var patients = _dbContext.Users
    .Include(u => u.Patient)
    .Where(u => u.Patient != null)
    .ToList();
            return View(patients);
        }
        public IActionResult DoctorManager()
        {
            return View();
        }
    }
}
