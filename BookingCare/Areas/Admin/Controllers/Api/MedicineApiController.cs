using BookingCare.Models;
using BookingCare.Models.DTOs;
using BookingCare.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCare.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [Route("api/admin/medicines")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class MedicineApiController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public MedicineApiController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        //=============================================
        // 1. Lấy danh sách thuốc
        // GET: /api/admin/medicines
        //=============================================
        [HttpGet]
        public async Task<IActionResult> GetMedicines(string? search = "", int page = 1, int pageSize = 10)
        {
            var query = _dbContext.Medicines.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m =>
                    m.Name.Contains(search) ||
                    (m.Function != null && m.Function.Contains(search))
                );
            }

            var totalMedicines = await query.CountAsync();

            var listMedicines = await query
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
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = new
                {
                    totalMedicines,
                    listMedicines
                }
            });
        }

        //=============================================
        // 2. Thêm thuốc
        // POST: /api/admin/medicines
        //=============================================
        [HttpPost]
        public async Task<IActionResult> AddMedicine([FromBody] Add_UpdateMedicineDtos medicine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = "Vui lòng điền đầy đủ thông tin thuốc!"
                });
            }

            var med = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Name == medicine.Name);
            if (med != null)
            {
                return BadRequest(new 
                { 
                    success = false,
                    message = "Thuốc đã tồn tại trong hệ thông!" 
                });
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
            return Ok(new
            { 
                success = true,
                message = "Thêm thuốc vào hệ thống thành công!" 
            });
        }

        //=============================================
        // 3. Cập nhật thông tin thuốc
        // Bước 1: Lấy thông tin thuốc
        // GET: /api/admin/medicines/id
        //=============================================
        [HttpGet("{id}")]
        public async Task<IActionResult> UpdateMedicineDetails(int id)
        {
            var med = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (med == null)
            {
                return NotFound(new 
                { 
                    success = false,
                    message = "Không tìm thấy thuốc!" 
                });
            }

            //Thông tin thuốc
            var inforMedicine = new Add_UpdateMedicineDtos
            {
                Name = med.Name,
                Unit = med.Unit,
                Function = med.Function
            };
            var medicineId = med.Id;

            return Ok(new 
            { 
                success = true,
                data = new
                {
                    inforMedicine,
                    medicineId
                }
            });
        }

        //=============================================
        // Bước 2: Cập nhật thông tin thuốc
        // PUT: /api/admin/medicines/id
        //=============================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, [FromBody] Add_UpdateMedicineDtos update_medicine)
        {
            //Kiểm tra dữ liệu gửi về hợp lệ không
            if (!ModelState.IsValid)
            {
                return BadRequest(new 
                { 
                    success = false,
                    message = "Vui lòng điền đầy đủ thông tin thuốc!" 
                });
            }

            var med = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (med == null)
            {
                return NotFound(new 
                {
                    success = false,
                    message = "Không tìm thấy thuốc!"
                });
            }

            var isDuplicate = await _dbContext.Medicines.AnyAsync(m => m.Name == update_medicine.Name && m.Id != id);

            if (isDuplicate)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Tên thuốc đã tồn tại trong hệ thống!"
                });
            }

            //Cập nhật thông tin
            med.Name = update_medicine.Name;
            med.Unit = update_medicine.Unit;
            med.Function = update_medicine.Function;
            med.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return Ok(new 
            { 
                success = true,
                message = "Cập nhật thông tin thuốc thành công!"
            });
        }

        //=============================================
        // 4. Khóa/Mở khóa thuốc
        // PUT: /api/admin/medicines/lock/id
        //=============================================
        [HttpPut("lock/{id}")]
        public async Task<IActionResult> LockMedicine(int id)
        {
            var medicine = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound(new 
                { 
                    success = false, 
                    message = "Không tìm thấy thuốc!"
                });
            }
            medicine.Status = "Dừng sử dụng";
            medicine.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return Ok(new 
            { 
                success = true, 
                message = "Khóa thuốc thành công!" 
            });
        }

        //=============================================
        // PUT: /api/admin/medicines/unlock/id
        //=============================================
        [HttpPut("unlock/{id}")]
        public async Task<IActionResult> UnlockMedicine(int id)
        {
            var medicine = await _dbContext.Medicines.FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound(new 
                { 
                    success = false, 
                    message = "Không tìm thấy thuốc!" 
                });
            }
            medicine.Status = "Đang sử dụng";
            medicine.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return Ok(new 
            {
                success = true,
                message = "Mở khóa thuốc thành công!" 
            });
        }
    }
}
