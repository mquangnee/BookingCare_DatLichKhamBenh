//Đăng nhập
const loginForm = document.getElementById("loginForm");

loginForm.addEventListener("submit", async function (e) {
    e.preventDefault();

    //Lấy dữ liệu từ form
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value;
    const rememberMe = document.getElementById("rememberMe").checked;
    const msg = document.getElementById("msg");
    const logoutNoti = document.getElementById("logout_noti");

    if (logoutNoti) {
        logoutNoti.style.display = "none";
    }

    try {
        //Gửi yêu cầu đăng nhập đến server
        const res = await fetch("/api/AuthApi/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password, rememberMe })
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

        //Nếu thành công, chuyển đến trang chủ theo vai trò
        if (data.success) {
            if (data.roles.includes("Admin")) { //Nếu là Admin
                setTimeout(() => window.location.href = "/Admin/Dashboard/Index", 1000);
            } else if (data.roles.includes("Doctor")) { //Nếu là Bác sĩ
                setTimeout(() => window.location.href = "/Doctor/Home/Index", 1000);
            } else { //Nếu là Bệnh nhân
                setTimeout(() => window.location.href = "/Patient/Home/Index", 1000);
            }
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});