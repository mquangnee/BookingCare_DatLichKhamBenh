# BookingCare Backend API

## Giới thiệu

**BookingCare** là một dự án web backend mô phỏng hệ thống đặt lịch khám bệnh trực tuyến, cho phép bệnh nhân đặt lịch với bác sĩ, quản lý lịch hẹn, xem kết quả khám bệnh và hỗ trợ quản trị hệ thống y tế cơ bản. Dự án được xây dựng nhằm phục vụ mục đích học tập, rèn luyện kỹ năng lập trình backend với **ASP.NET Core Web API** và **Entity Framework Core**.

Dự án tập trung vào các chức năng cốt lõi như:

* Quản lý người dùng (Admin / Doctor / Patient)
* Đặt lịch khám bệnh
* Quản lý bác sĩ và chuyên khoa
* Quản lý kết quả khám và đơn thuốc
* Thống kê lịch hẹn theo ngày, theo bác sĩ

---

## Công nghệ sử dụng

* **Ngôn ngữ**: C#
* **Framework**: ASP.NET Core Web API
* **ORM**: Entity Framework Core
* **Cơ sở dữ liệu**: SQL Server
* **Xác thực**: ASP.NET Identity + JWT
* **Công cụ khác**:

  * Swagger (OpenAPI)
  * LINQ
  * RESTful API

---

## Kiến trúc hệ thống

Dự án được xây dựng theo mô hình **Layered Architecture**, gồm các lớp chính:

* **Controllers**: Tiếp nhận request từ client và trả response
* **Models (Entities)**: Định nghĩa các bảng dữ liệu (Doctor, Patient, Appointment, ...)
* **DTOs**: Đóng gói dữ liệu truyền giữa client và server
* **Services**: Xử lý logic nghiệp vụ
* **Data**: DbContext và migration

---

## Các chức năng chính

### 1. Xác thực & phân quyền

* Đăng ký / đăng nhập người dùng
* Phân quyền theo vai trò: Admin, Doctor, Patient
* Bảo mật API bằng JWT

### 2. Quản lý bác sĩ

* Thêm / sửa / xóa bác sĩ
* Lấy danh sách bác sĩ
* Hiển thị thông tin chi tiết bác sĩ (họ tên, chuyên khoa, ảnh đại diện)

### 3. Quản lý bệnh nhân

* Lưu thông tin bệnh nhân
* Quản lý tiền sử bệnh án

### 4. Đặt lịch khám

* Bệnh nhân đặt lịch khám theo ngày
* Gán lịch khám với bác sĩ
* Kiểm tra số lượng lịch khám trong ngày

### 5. Kết quả khám bệnh

* Xem kết quả khám theo lịch hẹn
* Hiển thị chẩn đoán, đơn thuốc

### 6. Thống kê & báo cáo

* Thống kê số lịch khám theo bác sĩ trong ngày
* Hỗ trợ lọc dữ liệu theo ngày

---

## Cài đặt & chạy dự án

### 1. Yêu cầu hệ thống

* .NET SDK 7.0 trở lên
* SQL Server
* Visual Studio 2022 / VS Code

### 2. Clone repository

```bash
git clone https://github.com/your-username/bookingcare-backend.git
cd bookingcare-backend
```

### 3. Cấu hình database

Cập nhật chuỗi kết nối trong file `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=bookingcare;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 4. Chạy migration

```bash
dotnet ef database update
```

### 5. Chạy ứng dụng

```bash
dotnet run
```

Truy cập Swagger tại:

```
https://localhost:{port}/swagger
```

---

## Hướng phát triển

* Tích hợp thanh toán online
* Triển khai Docker

---

## Tác giả

* **Nguyễn Minh Quang, Ngô Khắc Tài, Phan Văn Trường, Đậu Quốc Dũng**
* Sinh viên Công nghệ Thông tin
* Dự án phục vụ mục đích học tập và nghiên cứu

---

## Ghi chú

Dự án được xây dựng với mục đích học tập, không sử dụng cho môi trường production thực tế.

