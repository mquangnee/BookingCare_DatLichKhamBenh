//Bước 1: Nhập email đăng ký, mật khẩu mới và xác nhận mật khẩu mới
function handleChangePasswordStep1() {
    const form = document.getElementById("changePasswordStep1Form");
    if (!form) return;

    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        //Lấy giá trị email, password và confirmedPassword từ form
        const OldPassword = document.getElementById("oldPassword").value.trim();
        const NewPassword = document.getElementById("newPassword").value.trim();
        const ConfirmNewPassword = document.getElementById("confirmNewPassword").value.trim();

        if (NewPassword !== ConfirmNewPassword) {
            alert("Mật khẩu xác nhận không khớp!");
            return;
        }

        try {
            //Gửi yêu cầu đổi mật khẩu đến server
            const res = await fetch("/Patient/api/ChangePasswordApi/changePass-step1", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ OldPassword, NewPassword, ConfirmNewPassword })
            });

            //Xử lý phản hồi từ server
            const result = await res.json();
            alert(result.message);
            if (res.ok && result.success) {
                setTimeout(() => window.location.href = "/Patient/AccountManager/ChangePasswordStep2", 1000);
            }
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
        }
    });
}

//Bước 2: Xác thực mã OTP
function handleChangePasswordStep2() {
    const form1 = document.getElementById("changePasswordStep2Form");
    if (!form1) return;

    alert("Mã xác thực đã được gửi tới email của bạn!");

    //****Xử lý gửi mã OTP****
    form1.addEventListener("submit", async function (e) {
        e.preventDefault();

        //Lấy giá trị otp từ form
        const otp = document.getElementById("otp").value;

        try {
            //Gửi yêu cầu xác thực OTP đến server
            const res = await fetch("/Patient/api/ChangePasswordApi/changePass-step2", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ otp })
            });

            //Xử lý phản hồi từ server
            const result = await res.json();

            //Hiển thị thông báo
            alert(result.message);

            //Nếu thành công, chuyển đến trang đăng nhập
            if (result.success) {
                setTimeout(() => window.location.href = "/Patient/Home/Index", 1000);
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
            const res = await fetch("/Patient/api/ChangePasswordApi/changePass-resend-otp", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ otp })
            });

            //Xử lý phản hồi từ server
            const result = await res.json();

            //Hiển thị thông báo
            alert(result.message);
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
        }
    });
}

// ===== Gọi tất cả hàm =====
document.addEventListener("DOMContentLoaded", () => {
    handleChangePasswordStep1();
    handleChangePasswordStep2();
});