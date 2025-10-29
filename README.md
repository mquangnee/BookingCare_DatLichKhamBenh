## Mục lục
- [1. Giới thiệu bản thân](#1-giới-thiệu-bản-thân)
- [2. Giới thiệu dự án](#2-giới-thiệu-dự-án)
- [3. Kiến trúc và công nghệ sử dụng](#3-kiến-trúc-và-công-nghệ-sử-dụng)
- [4. Database](#4-database)
- [5. Các chức năng chính](#5-các-chức-năng-chính)
  - [5.1. Chức năng người dùng (Bệnh nhân)](#51-chức-năng-người-dùng-bệnh-nhân)
  - [5.2. Chức năng bác sĩ](#52-chức-năng-bác-sĩ)
  - [5.3. Chức năng quản trị viên (Admin)](#53-chức-năng-quản-trị-viên-admin)
- [6. Một số tính năng nổi bật mình thực hiện](#6-các-tính-năng-mình-thực-hiện)
- [7. Cấu trúc dự án](#6-cấu-trúc-dự-án)
- [8. Cách chạy dự án](#7-cách-chạy-dự-án)
- [9. Liên hệ](#8-liên-hệ)

---

## 1. Giới thiệu bản thân

Xin chào!  
Mình tên là **Nguyễn Minh Quang**, sinh viên năm 4 chuyên ngành **Kỹ thuật phần mềm** tại **Trường Công nghệ thông tin và Truyền thông - Trường Đại học Công nghiệp Hà Nội**.  
Hiện tại mình đang tìm kiếm vị trí **Thực tập sinh Lập trình viên C# / ASP.NET** để có cơ hội áp dụng kiến thức đã học và phát triển kỹ năng chuyên môn.

- GPA: 3.26
- Ngôn ngữ sử dụng chính: **C#, Java**
- Email: [nguyenminhquangg03012004@gmail.com]  
- GitHub: [https://github.com/mquangnee](https://github.com/mquangnee)

---

## 2. Giới thiệu dự án

**BookingCare** là dự án website mô phỏng nền tảng đặt lịch khám bệnh trực tuyến, nơi bệnh nhân có thể:
- Đăng ký, đăng nhập tài khoản
- Tìm kiếm bác sĩ, chuyên khoa, và đặt lịch khám  
- Nhận email xác nhận sau khi đặt lịch thành công  
Hệ thống hướng đến việc xây dựng một môi trường tiện ích, đáp ứng nhu cầu đặt lịch khám bệnh nhanh chóng, đồng thời tối ưu quy trình làm việc của các cơ sở y tế.

---

## 3. Kiến trúc và công nghệ sử dụng

| Thành phần | Công nghệ |
|-------------|-----------|
| **Ngôn ngữ** | C# |
| **Framework** | ASP.NET Core MVC 8.0 |
| **Database** | SQL Server |
| **Template mail** | Razor View |
| **Frontend** | HTML, CSS, JavaScript (jQuery, AJAX), Bootstrap 5 |
| **IDE** | Visual Studio 2022 |

---

## 4. Database

![Database_BookingCare_2025-10-29T12_09_01 728Z](https://github.com/user-attachments/assets/82232b26-0055-48ca-a985-8cc943a3cd34)

---

## 5. Các chức năng chính

### 5.1. Chức năng người dùng (Bệnh nhân)
- Đăng ký tài khoản, xác thực OTP qua email  
- Đăng nhập, ghi nhớ đăng nhập  
- Cập nhật thông tin cá nhân  
- Xem danh sách bác sĩ, chuyên khoa 
- Đặt lịch khám, nhận email xác nhận  
- Hủy lịch hoặc xem lại lịch khám trước đó  

---

### 5.2. Chức năng bác sĩ
- Đăng nhập bằng tài khoản được admin cấp  
- Cập nhật hồ sơ cá nhân (học vị, chuyên khoa, kinh nghiệm, ảnh đại diện, mô tả ngắn)  
- Xem danh sách lịch hẹn của bệnh nhân  
- Cập nhật trạng thái lịch khám (chờ khám, đã khám, hủy)  
- Tạo đơn thuốc, trả kết quả khám bệnh cho bệnh nhân qua email 

---

### 5.3. Chức năng quản trị viên (Admin)
- Quản lý tài khoản người dùng (Bệnh nhân, Bác sĩ, Quản trị viên)  
- Tạo mới, chỉnh sửa, khóa/mở tài khoản  
- Quản lý thông tin bác sĩ: chuyên khoa, học vị, kinh nghiệm  
- Quản lý thuốc (thêm, sửa, xóa)  
- Xem thống kê tổng quan: số lượng bác sĩ, bệnh nhân, lịch khám  

---

## 6. Các tính năng mình thực hiện

| Tính năng | Mô tả |
|------------|-------|
| **Đăng nhập & đăng ký** | Sử dụng ASP.NET Core Identity, có RememberMe |
| **Xác thực OTP qua Email** | Khi đăng ký tài khoản, hệ thống gửi mã OTP để kích hoạt |
| **Dashboard** | Thống kế số lượng bác sĩ, bệnh nhân của phòng khám |
| **Quản lý tài khoản trong trang Admin** | Phân loại rõ vai trò người dùng: Bệnh nhân, Bác sĩ |
| **Quản lý thuốc** | Cho phép thêm, sửa, xóa thuốc trong hệ thống |

### 6.1 Đăng nhập & đăng ký
- **Đăng ký:**
* Bước 1: Đăng ký bằng email và mật khẩu (yêu cầu dài tối thiếu 6 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt)
<img width="800" alt="Screenshot 2025-10-29 192216" src="https://github.com/user-attachments/assets/dca309ee-9f91-46b3-b5f8-b7b045b82819" />

* Bước 2: Xác nhận OTP
 <img width="800" alt="Screenshot 2025-10-29 192250" src="https://github.com/user-attachments/assets/c000578b-c8f0-4b09-aec8-06ea047f5514" />

* Bước 3: Điền thông tin cá nhân
<img width="800" alt="Screenshot 2025-10-29 192321" src="https://github.com/user-attachments/assets/00218b63-7aac-47cc-85c2-d4fbbe8b5108" />

- Đăng nhập:
<img width="800" alt="Screenshot 2025-10-29 191940" src="https://github.com/user-attachments/assets/7ed14de8-4f88-4b81-9409-78735281e071" />

- Quên mật khẩu:
* Bước 1: Điền email đã đăng ký và mật khẩu mới, xác nhận mật khẩu mới
<img width="800" alt="Screenshot 2025-10-29 191953" src="https://github.com/user-attachments/assets/3c96bcb9-78d2-4250-a848-4b3b2efc399b" />

* Bước 2: Xác nhận OTP
<img width="800" alt="Screenshot 2025-10-29 204449" src="https://github.com/user-attachments/assets/f1a005f0-e4ae-4118-8783-9913c4975deb" />
<img width="400" alt="Screenshot 2025-10-29 192054" src="https://github.com/user-attachments/assets/82a72590-ef4f-41d9-9834-34feda621669" />

### 6.2. Dashboard
- **Trang chủ**:
<img width="800" alt="Screenshot 2025-10-29 192338" src="https://github.com/user-attachments/assets/461dd758-7ae6-46cd-a232-e37b33be84f3" />

- **Quản lý tài khoản bệnh nhân**: Admin có thể khóa/mở khóa, xem thông tin chi tiết
<img width="800" alt="Screenshot 2025-10-29 192346" src="https://github.com/user-attachments/assets/dd3f0a12-58be-4ad2-a9ea-ea2206f40ffd" />
<img width="800" alt="Screenshot 2025-10-29 192356" src="https://github.com/user-attachments/assets/16caedd4-178e-4713-ba76-2c70a8e597a1" />

- **Quản lý tài khoản bác sĩ**: Admin có thể khóa/mở khóa, xem thông tin chi tiết, thêm hoặc chỉnh sửa thông tin tài khoản
<img width="800" alt="Screenshot 2025-10-29 192403" src="https://github.com/user-attachments/assets/44708835-fd7b-4adc-baff-d34caffd2eac" />
<img width="800" alt="Screenshot 2025-10-29 192411" src="https://github.com/user-attachments/assets/95c3697c-c3e0-4f4c-ab64-e2b28778e733" />
<img width="800" alt="Screenshot 2025-10-29 192423" src="https://github.com/user-attachments/assets/d8165ea2-e7b6-4cf1-b255-8c4cb67a8fee" />
<img width="800" alt="Screenshot 2025-10-29 192435" src="https://github.com/user-attachments/assets/a96e4778-e263-497f-9d8d-75f61da54f53" />

- **Quản lý thuốc**: Đang cập nhật

---

Dự án này được thực hiện trong khuôn khổ môn học *Đồ án chuyên ngành*.
