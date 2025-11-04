//Bước 1: Nhập email đăng ký, mật khẩu mới và xác nhận mật khẩu mới
function handleForgotPasswordStep1() {
    const form = document.getElementById("forgotPasswordStep1Form");
    if (!form) return;

    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        //Lấy giá trị email, password và confirmedPassword từ form
        const Email = document.getElementById("email").value.trim();
        const NewPassword = document.getElementById("newPassword").value.trim();
        const ConfirmedNewPassword = document.getElementById("confirmedNewPassword").value.trim();
        const msg = document.getElementById("msg");

        try {
            //Gửi yêu cầu đổi mật khẩu đến server
            const res = await fetch("/api/AuthApi/forgotPass-step1", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Email, NewPassword, ConfirmedNewPassword })
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
                    // Kiểm tra nếu có lỗi ở ConfirmedNewPassword
                    if (data.errors.ConfirmedNewPassword) {
                        const errorMessage = data.errors.ConfirmedNewPassword[0];
                        msg.textContent = errorMessage;
                        return;
                    }
                }
            }

            //Nếu thành công, chuyển sang bước 2
            if (data.success) {
                //Lưu tạm thời email và pass
                localStorage.setItem("Email", Email);
                setTimeout(() => window.location.href = "/Account/ForgotPasswordStep2", 1000);
            }
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
        }
    });
}

//Bước 2: Xác thực mã OTP
function handleForgotPasswordStep2() {
    const form1 = document.getElementById("forgotPasswordStep2Form");
    if (!form1) return;

    const email = localStorage.getItem("Email");
    const msg = document.getElementById("msg");

    //Hiển thị thông báo
    msg.textContent = "Mã xác thực đã được gửi đến " + email;
    msg.className = "alert alert-success text-center mb-4";

    //****Xử lý gửi mã OTP****
    form1.addEventListener("submit", async function (e) {
        e.preventDefault();

        //Lấy giá trị otp từ form
        const otp = document.getElementById("otp").value;

        try {
            //Gửi yêu cầu xác thực OTP đến server
            const res = await fetch("/api/AuthApi/forgotPass-step2", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Email: email, Otp: otp })
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


    //****Xử lý gửi lại mã OTP****
    const form2 = document.getElementById("resendOtpForm");
    form2.addEventListener("submit", async function (e) {
        e.preventDefault();

        var otp = "000000";

        try {
            //Gửi yêu cầu gửi lại mã OTP đến server
            const res = await fetch("/api/AuthApi/forgotPass-resend-otp", {
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

// ===== Gọi tất cả hàm =====
document.addEventListener("DOMContentLoaded", () => {
    handleForgotPasswordStep1();
    handleForgotPasswordStep2();
});