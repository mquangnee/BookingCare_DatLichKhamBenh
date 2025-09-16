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

        //Lưu OTP vào bộ nhớ đệm với thời gian hết hạn là 5 phút
        public void SetOtp(string Email, string OTP)
        {
            _memoryCache.Set(Email, OTP, TimeSpan.FromMinutes(5));
        }
        //Lấy OTP từ bộ nhớ đệm
        public string GetOtp(string Email)
        {
            _memoryCache.TryGetValue(Email, out string OTP);
            return OTP;
        }
        //Xóa OTP khỏi bộ nhớ đệm sau khi xác thực thành công hoặc hết hạn
        public void RemoveOtp(string Email)
        {
            _memoryCache.Remove(Email);
        }
    }
}
