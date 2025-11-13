//Bước 1: Đăng ký email, password và gửi mã OTP
function handleRegisterStep1() {
    const form = document.getElementById("registerStep1Form");
    if (!form) return;
    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        //Lấy giá trị email và password từ form
        const Email = document.getElementById("email").value.trim();
        const Password = document.getElementById("password").value.trim();
        const ConfirmedPassword = document.getElementById("confirmedPassword").value.trim();
        const msg = document.getElementById("msg");

        try {
            //Gửi yêu cầu đăng ký email và password đến server
            const res = await fetch("/api/AuthApi/register-step1", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Email, Password, ConfirmedPassword })
            });

            //Xử lý phản hồi từ server
            const data = await res.json();

            //Hiển thị thông báo
            msg.textContent = data.message;
            if (res.ok) {//Nếu thành công
                msg.className = "alert alert-success text-center";
            } else { //Nếu thất bại
                msg.className = "alert alert-danger text-center";
                if (data.errors) {
                    // Kiểm tra nếu có lỗi ở ConfirmedPassword
                    if (data.errors.ConfirmedPassword) {
                        const errorMessage = data.errors.ConfirmedPassword[0];
                        msg.textContent = errorMessage;
                        return;
                    }
                }
            }

            //Nếu thành công, chuyển sang bước 2
            if (data.success) {
                //Lưu tạm thời email và pass
                localStorage.setItem("Email", Email);
                setTimeout(() => window.location.href = "/Account/RegisterStep2", 1000);
            }
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
        }
    });
}

//Bước 2: Xác thực mã OTP
function handleRegisterStep2() {
    const form1 = document.getElementById("registerStep2Form");
    if (!form1) return;

    const email = localStorage.getItem("Email");
    const msg = document.getElementById("msg");

    //Hiển thị thông báo
    msg.textContent = "Mã xác thực đã được gửi đến " + email;
    msg.className = "alert alert-success text-center mb-4";

    //****Xử lý gửi mã OTP****
    form1.addEventListener("submit", async function (e) {
        e.preventDefault();

        //Lấy giá trị email từ localStorage và OTP từ form
        const otp = document.getElementById("otp").value;

        //Kiểm tra email tồn tại
        if (!email) {
            msg.textContent = "Không tìm thấy email trong phiên làm việc. Vui lòng quay lại bước 1.";
            msg.style.color = "red";
            return setTimeout(() => window.location.href = "/Account/RegisterStep1", 2000);
        }

        try {
            //Gửi yêu cầu xác thực OTP đến server
            const res = await fetch("/api/AuthApi/register-step2", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, otp })
            });

            //Xử lý phản hồi từ server
            const data = await res.json();

            //Hiển thị thông báo
            msg.textContent = data.message;
            if (res.ok) {//Nếu thành công
                msg.className = "alert alert-success text-center";
            } else { //Nếu thất bại
                msg.className = "alert alert-danger text-center";
            }

            //Nếu thành công, chuyển sang bước 3
            if (data.success) {
                setTimeout(() => window.location.href = "/Account/RegisterStep3", 1000);
            }
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
        }
    });

    //****Xử lý gửi lại mã OTP****
    const form2 = document.getElementById("resendOtpForm");
    form2.addEventListener("submit", async function (e) {
        e.preventDefault();

        //Kiểm tra email tồn tại
        if (!email) {
            msg.textContent = "Không tìm thấy email trong phiên làm việc. Vui lòng quay lại bước 1.";
            msg.style.color = "red";
            return setTimeout(() => window.location.href = "/Account/RegisterStep1", 2000);
        }

        var otp = "000000";
        try {
            //Gửi yêu cầu gửi lại mã OTP đến server
            const res = await fetch("/api/AuthApi/register-resend-otp", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, otp })
            });

            //Hiển thị thông báo
            if (res.ok) {
                msg.textContent = "Mã xác thực đã được gửi lại đến " + email;
                msg.className = "alert alert-success text-center mb-4";
            }
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
        }
    });
}

//Bước 3: Hoàn tất đăng ký
function handleRegisterStep3() {
    const form = document.getElementById("registerStep3Form");
    if (!form) return;
    form.addEventListener("submit", async function (e) {
        e.preventDefault();
        const email = localStorage.getItem("Email");
        const msg = document.getElementById("msg");

        if (!email) {
            alert("Phiên đăng ký đã hết hạn, vui lòng thực hiện lại");
            return window.location.href = "/Account/RegisterStep1";
        }

        //Lấy dữ liệu từ form
        const body = {
            email: email,
            fullName: document.getElementById("fullName").value.trim(),
            gender: document.getElementById("gender").value,
            dateOfBirth: document.getElementById("dateOfBirth").value,
            address: document.getElementById("address").value.trim(),
            phoneNumber: document.getElementById("phoneNumber").value.trim()
        }

        try {
            //Gửi yêu cầu hoàn tất đăng ký đến server
            const res = await fetch("/api/AuthApi/register-step3", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(body)
            });

            //Xử lý phản hồi từ server
            const data = await res.json();

            //Hiển thị thông báo
            msg.textContent = data.message;
            if (res.ok) {//Nếu thành công
                msg.className = "alert alert-success text-center";
            } else { //Nếu thất bại
                msg.className = "alert alert-danger text-center";
            }

            //Nếu thành công, chuyển đến trang đăng nhập
            if (data.success) {
                localStorage.removeItem("Email");
                setTimeout(() => window.location.href = "/Account/Login", 1000);
            }
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
        }
    });
}

// ===== Gọi tất cả hàm =====
document.addEventListener("DOMContentLoaded", () => {
    handleRegisterStep1();
    handleRegisterStep2();
    handleRegisterStep3();
});