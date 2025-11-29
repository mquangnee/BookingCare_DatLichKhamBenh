using Microsoft.Extensions.Caching.Memory;

namespace BookingCare.Services
{
    public class OtpService
    {
        private readonly IMemoryCache _memoryCache;
        public OtpService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        //=== Quản lý OTP trong bộ nhớ đệm ===//
        //Lưu cờ xác thực OTP
        public void SetOtpFlag(string Email)
        {
            _memoryCache.Set(Email, true, TimeSpan.FromMinutes(5));
        }

        //Kiểm tra cờ xác thực OTP
        public bool IsVerifiedOtp(string Email)
        {
            return _memoryCache.Get<bool>(Email);
        }

        //Lưu OTP vào bộ nhớ đệm với thời gian hết hạn là 5 phút
        public void SetOtp(string Email, string OTP)
        {
            _memoryCache.Set($"{Email}_otp", OTP, TimeSpan.FromMinutes(5));
        }

        //Lấy OTP từ bộ nhớ đệm
        public string? GetOtp(string Email)
        {
            return _memoryCache.Get<string>($"{Email}_otp");
        }

        //Xóa OTP khỏi bộ nhớ đệm sau khi xác thực thành công hoặc hết hạn
        public void RemoveOtp(string Email)
        {
            _memoryCache.Remove($"{Email}_otp");
        }

        //=== Quản lý Password tạm thời trong bộ nhớ đệm ===//
        //Lưu password vào bộ nhớ đệm với thời gian hết hạn là 10 phút
        public void SetPassword(string Email, string Password)
        {
            _memoryCache.Set($"{Email}_password", Password, TimeSpan.FromMinutes(10));
        }

        //Lấy password từ bộ nhớ đệm
        public string? GetPassword(string Email)
        {
            return _memoryCache.Get<string>($"{Email}_password");
        }

        //Xóa password khỏi bộ nhớ đệm sau khi sử dụng
        public void RemovePassword(string Email)
        {
            _memoryCache.Remove($"{Email}_password");
        }
    }
}
