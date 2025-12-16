//Lấy số bác sĩ, số bệnh nhân, số lịch khám đã đặt hôm nay, số lịch khám đã hủy
async function loadDashboard() {
    try {
        const res = await fetch("/Admin/api/DashboardApi/index");

        const data = await res.json();

        //Nếu lỗi dữ liệu
        if (!data.success) {
            console.error("Lỗi:", data.message);
            alert(data.message);
            return;
        }

        //Lấy các thành phần từ View
        const totalDoctors = document.getElementById("totalDoctors");
        const totalPatients = document.getElementById("totalPatients");
        const totalApptToday = document.getElementById("totalApptToday");
        const totalCanceledApptToday = document.getElementById("totalCanceledApptToday");

        //Hiển thị dữ liệu
        totalDoctors.textContent = data.totalDoctors;
        totalPatients.textContent = data.totalPatients;
        totalApptToday.textContent = data.totalApptToday;
        totalCanceledApptToday.textContent = data.totalCanceledApptToday;
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
}

//Gọi lần đầu khi load trang
loadDashboard();

//Cập nhật lại mỗi 5s
setInterval(loadDashboard, 5000);