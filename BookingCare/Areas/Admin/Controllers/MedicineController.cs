using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MedicineController : Controller
    {
        private readonly DataContext _dbContext;
        public MedicineController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }   

        //Hiển thị trang quản lý thuốc
        [HttpGet]
        public IActionResult Index()
        {
            var medicines = _dbContext.Medicines.ToList();
            return View(medicines);
        }
    }
}
