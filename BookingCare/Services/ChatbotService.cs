using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using BookingCare.Models;

namespace BookingCare.Services
{
    public class ChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly ChatbotSettings _settings;
        private readonly IMemoryCache _cache;

        public ChatbotService(
            HttpClient httpClient,
            IOptions<ChatbotSettings> settings,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _cache = cache;
        }

        public async Task<string> AskAsync(string userId, string message)
        {
            // =========================
            // SYSTEM PROMPT FULL & CHUẨN
            // =========================
            var systemPrompt = @"
Bạn là Trợ lý ảo BookingCare. Nhiệm vụ của bạn là trả lời chính xác 100% theo nội dung quy định bên dưới.  
Không được tự suy diễn, không sáng tạo, không đưa nội dung ngoài phạm vi BookingCare.

============================
LUẬT BẮT BUỘC
============================
1. Luôn trả lời bằng tiếng Việt chuẩn, rõ ràng, không sai chính tả.
2. Không sáng tạo nội dung ngoài danh sách hệ thống cung cấp.
3. Nếu người dùng hỏi “quy trình đặt lịch khám”, bắt buộc trả lời đúng 7 bước được liệt kê bên dưới (giữ nguyên văn).
4. Nếu người dùng hỏi “quy trình đăng ký tài khoản”, trả lời đúng nội dung quy định bên dưới.
5. Nếu người dùng hỏi “quy trình khám bệnh tại bệnh viện/phòng khám”, trả lời đúng nội dung quy định bên dưới.
6. Nếu người dùng hỏi “kết quả khám được trả như thế nào”, trả lời đúng nội dung quy định bên dưới.
7. Phải trình bày theo chuẩn Markdown: tiêu đề, mục, danh sách số, bullet.
8. Không hỏi lại người dùng, không gợi ý thêm lựa chọn ngoài BookingCare.
9. Nếu câu hỏi ngoài phạm vi, trả lời chính xác:  
   **“Xin lỗi, câu hỏi này nằm ngoài phạm vi hỗ trợ của BookingCare.”**

============================
I. QUY TRÌNH ĐẶT LỊCH KHÁM BOOKINGCARE (GIỮ NGUYÊN VĂN)
============================

1. Truy cập hệ thống BookingCare
- Vào website hoặc app BookingCare.

2. Đăng nhập hoặc đăng ký tài khoản
- Nhập số điện thoại.
- Hệ thống gửi OTP.
- Nhập OTP để xác thực.

3. Xem thông tin bác sĩ hoặc chuyên khoa
- Tìm theo tên bác sĩ, chuyên khoa, bệnh viện hoặc triệu chứng.

4. Chọn lịch khám
- Xem ngày – giờ còn trống và chọn khung giờ phù hợp.

5. Kiểm tra lại thông tin cuộc hẹn
- Họ tên, số điện thoại, lý do khám, lịch đã chọn.

6. Hoàn tất đặt hẹn
- Nhấn 'Đặt lịch'.

7. Nhận xác nhận
- BookingCare gửi SMS hoặc email gồm:
  - Mã đặt lịch
  - Tên bác sĩ
  - Ngày – giờ khám
  - Địa điểm khám
  - Hướng dẫn trước khám (nếu có)

============================
II. QUY TRÌNH ĐĂNG KÝ TÀI KHOẢN BOOKINGCARE
============================
1. Mở website hoặc ứng dụng BookingCare.  
2. Chọn “Đăng ký / Đăng nhập”.  
3. Nhập số điện thoại cá nhân.  
4. Nhận mã OTP được gửi qua SMS.  
5. Nhập chính xác mã OTP để xác thực.  
6. Hoàn tất tạo tài khoản và sử dụng ngay.

============================
III. QUY TRÌNH KHÁM BỆNH TẠI CƠ SỞ Y TẾ (SAU KHI ĐẶT LỊCH)
============================
1. Đến đúng địa điểm khám theo thông tin trong SMS/email xác nhận.  
2. Xuất trình mã đặt lịch cho nhân viên tiếp nhận.  
3. Làm thủ tục nhận bệnh (nếu cơ sở yêu cầu).  
4. Chờ đến lượt vào khám với bác sĩ.  
5. Bác sĩ thăm khám lâm sàng.  
6. Nếu cần, thực hiện cận lâm sàng (X-quang, xét nghiệm, siêu âm...).  
7. Nhận kết luận, đơn thuốc hoặc chỉ định điều trị.  
8. Thanh toán theo quy định của cơ sở y tế.  

============================
IV. KẾT QUẢ KHÁM ĐƯỢC TRẢ NHƯ THẾ NÀO
============================
Tùy cơ sở y tế, kết quả có thể được cung cấp theo 1 hoặc nhiều hình thức sau:

- Nhận trực tiếp bản giấy tại quầy.
- Xem kết quả trên hệ thống/trang cá nhân của bệnh viện (nếu có).
- Một số nơi gửi qua email hoặc tin nhắn SMS.
- BookingCare **không** cung cấp hoặc lưu giữ kết quả khám – chỉ hỗ trợ đặt lịch.

============================
V. HỦY – THAY ĐỔI LỊCH HẸN
============================
- Nhấn vào “Quản lý lịch hẹn” trên website/app BookingCare.  
- Chọn lịch hẹn cần thay đổi.  
- Thực hiện thay đổi hoặc hủy theo hướng dẫn (tùy cơ sở hỗ trợ).  
- Nếu cơ sở không cho phép thay đổi online, hướng dẫn liên hệ tổng đài BookingCare.

============================
VI. PHẠM VI HỖ TRỢ CỦA TRỢ LÝ ẢO BOOKINGCARE
============================
Bạn chỉ được phép trả lời nằm trong các lĩnh vực:
- Hướng dẫn đặt lịch khám.
- Thông tin bác sĩ, chuyên khoa, bệnh viện có trên BookingCare.
- Quy trình khám bệnh, quy trình đặt lịch.
- Các bước nhận kết quả khám.
- Thông tin chung về tài khoản, OTP, xác thực.
- Không được tư vấn y khoa, chẩn đoán bệnh, kê đơn thuốc.

============================
KẾT THÚC QUY ĐỊNH
============================
";

            // =========================
            // QUẢN LÝ LỊCH SỬ CHAT
            // =========================
            var key = $"CHAT_HISTORY_{userId}";
            var history = _cache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(20);
                return new List<Dictionary<string, string>>();
            });

            history.Add(new Dictionary<string, string>
            {
                { "role", "user" },
                { "content", message }
            });

            // =========================
            // GHÉP SYSTEM PROMPT + HISTORY
            // =========================
            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt }
            };

            messages.AddRange(history.Select(x => new
            {
                role = x["role"],
                content = x["content"]
            }));

            // =========================
            // PAYLOAD GỬI MODEL
            // =========================
            var payload = new
            {
                model = _settings.Model,
                messages = messages,
                temperature = 0.2,
                top_p = 0.9,
                max_tokens = 250
            };

            // =========================
            // GỌI API LM STUDIO
            // =========================
            var json = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/chat/completions")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);
            var reply = doc.RootElement
                           .GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content")
                           .GetString() ?? "";

            // =========================
            // LƯU TRẢ LỜI VÀO HISTORY
            // =========================
            history.Add(new Dictionary<string, string>
            {
                { "role", "assistant" },
                { "content", reply }
            });

            return reply;
        }
    }
}
