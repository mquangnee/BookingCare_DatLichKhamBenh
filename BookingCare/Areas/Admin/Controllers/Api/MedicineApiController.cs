using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookingCare.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("Admin/api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class MedicineApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public MedicineApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Lấy danh sách thuốc để phân trang
        [HttpGet("medicines")]
        public async Task<IActionResult> GetMedicines(string? search = "", int page = 1, int pageSize = 10)
        {
            //Lấy danh sách thuốc
            var medicines = _dbContext.Medicines
                            .OrderBy(m => m.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(m => new MedicineDtos
                            {
                                Id = m.Id,
                                Name = m.Name,
                                Unit = m.Unit,
                                Function = m.Function,
                                CreatedAt = m.CreatedAt,
                                UpdatedAt = m.UpdatedAt,
                                Status = m.Status
                            });

            if (!string.IsNullOrWhiteSpace(search))
            {
                medicines = medicines.Where(m =>
                            m.Name.Contains(search) ||
                            (m.Function != null && m.Function.Contains(search))
                );
            }

            //Tổng số thuốc
            var totalMedicines = await medicines.CountAsync();

            //Lấy danh sách hiển thị ở trang muốn xem
            var data = await medicines.ToListAsync();

            return Ok(new { totalMedicines, data });
        }

        //Thêm thuốc
        [HttpPost("create")]
        public async Task<IActionResult> AddMedicine([FromBody] Add_UpdateMedicineDtos medicine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Vui lòng điền đầy đủ thông tin thuốc!" });
            }

            var med = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Name == medicine.Name);
            if (med != null)
            {
                return BadRequest(new { success = false, message = "Thuốc đã tồn tại trong hệ thông!" });
            }

            //Tạo đối tượng Medicine mới
            var newMedicine = new Medicine
            {
                Name = medicine.Name,
                Unit = medicine.Unit,
                Function = medicine.Function
            };

            //Thêm thuốc vào hệ thống
            await _dbContext.Medicines.AddAsync(newMedicine);
            await _dbContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Thêm thuốc vào hệ thống thành công!" });
        }

        //Cập nhật thuốc
        //1. Lấy thông tin chi tiết thuốc
        [HttpGet("updateInfoMedicine")]
        public async Task<IActionResult> UpdateMedicineDetails(int id)
        {
            var med = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (med == null)
            {
                return NotFound(new { message = "Không tìm thấy thuốc!" });
            }

            //Thông tin thuốc
            var result = new Add_UpdateMedicineDtos
            {
                Name = med.Name,
                Unit = med.Unit,
                Function = med.Function
            };
            var medicineId = med.Id;

            return Ok(new { result, medicineId });
        }

        //2. Cập nhật thông tin thuốc
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, [FromBody] Add_UpdateMedicineDtos update_medicine)
        {
            //Kiểm tra dữ liệu gửi về hợp lệ không
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Vui lòng điền đầy đủ thông tin thuốc!" });
            }

            var med = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (med == null)
            {
                return NotFound(new { message = "Không tìm thấy thuốc!" });
            }
            
            var tmp = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Name == update_medicine.Name); 
            if (tmp != null)
            {
                return BadRequest(new { success = false, message = "Tên thuốc đã tồn tại trong hệ thống!" });
            }

            //Cập nhật thông tin
            med.Name = update_medicine.Name;
            med.Unit = update_medicine.Unit;
            med.Function = update_medicine.Function;
            med.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Cập nhật thông tin thuốc thành công!" });
        }

        //Khóa thuốc
        [HttpPut("lock/{id}")]
        public async Task<IActionResult> LockMedicine(int id)
        {
            var medicine = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy thuốc!" });
            }
            medicine.Status = "Dừng sử dụng";
            medicine.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Khóa thuốc thành công!" });
        }

        //Mở khóa tài khoản
        [HttpPut("unlock/{id}")]
        public async Task<IActionResult> UnlockMedicine(int id)
        {
            var medicine = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy thuốc!" });
            }
            medicine.Status = "Đang sử dụng";
            medicine.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Mở khóa thuốc thành công!" });
        }
    }
}
