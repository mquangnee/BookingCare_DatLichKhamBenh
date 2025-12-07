// Lấy các thành phần từ View
const totalDoctors = document.getElementById("totalDoctors");
const totalPatients = document.getElementById("totalPatients");
const totalApptToday = document.getElementById("totalApptToday");
const totalCanceledApptToday = document.getElementById("totalCanceledApptToday");
const totalCount = document.getElementById("totalCount");
const ctxDaily = document.getElementById("bookingChart");
const ctxStatus = document.getElementById("appointmentPieChart");

// Tổng quan dashboard
async function loadDashboard() {
    try {
        const res = await fetch("/api/admin/dashboard/summary");
        const body = await handleResponse(res);

        //Hiển thị dữ liệu
        const data = body.data;
        totalDoctors.textContent = data.totalDoctors;
        totalPatients.textContent = data.totalPatients;
        totalApptToday.textContent = data.totalApptToday;
        totalCanceledApptToday.textContent = data.totalCanceledApptToday;
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
}

// Biểu đồ thống kê lịch hẹn
async function loadAppointmentAreaChart() {
    try {
        const res = await fetch("/api/admin/dashboard/appointments/daily");
        const body = await handleResponse(res);

        // Lấy dữ liệu
        const data = body.data;
        const labels = data.map(x => x.date);
        const values = data.map(x => x.total);

        new Chart(ctxDaily, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: "Số lịch đặt",
                    lineTension: 0.3,
                    backgroundColor: "rgba(78, 115, 223, 0.05)",
                    borderColor: "rgba(78, 115, 223, 1)",
                    pointRadius: 3,
                    pointBackgroundColor: "rgba(78, 115, 223, 1)",
                    data: values
                }]
            },
            options: {
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{ gridLines: { display: false } }],
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            precision: 0
                        }
                    }]
                },
                legend: { display: false }
            }
        });
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
}

// Biểu đồ thống kê trạng thái lịch hẹn
async function loadAppointmentPieChart() {
    try {
        const res = await fetch("/api/admin/dashboard/appointments/status");
        const body = await handleResponse(res);

        // Lấy dữ liệu
        const data = body.data;
        totalCount.textContent = "Tổng lịch khám: " + data.totalCount;

        new Chart(ctxStatus, {
            type: "doughnut",
            data: {
                labels: ["Chờ khám", "Đã khám", "Đã hủy"],
                datasets: [{
                    label: "Lịch khám (10 ngày gần nhất)",
                    data: [data.waitingCount, data.successCount, data.canceledCount],
                    backgroundColor: [
                        "rgba(255, 205, 86, 0.8)",  // vàng
                        "rgba(75, 192, 192, 0.8)", // xanh ngọc
                        "rgba(255, 99, 132, 0.8)"   // đỏ
                    ],
                    borderColor: [
                        "rgba(255, 205, 86, 1)",
                        "rgba(75, 192, 192, 1)",
                        "rgba(255, 99, 132, 1)"
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: "bottom",
                    },
                    title: {
                        display: true,
                        text: "Tỷ lệ lịch khám 10 ngày gần nhất"
                    }
                }
            }
        });
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
}

// Xử lý phản hồi từ server
async function handleResponse(res) {
    let body = null;
    try {
        body = await res.json();
    } catch {
        body = null;
    }

    // Bad Request
    if (res.status === 400) {
        throw body?.message || "Dữ liệu gửi lên không hợp lệ.";
    }
    // Unauthorized
    if (res.status === 401) {
        alert("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
        location.href = "/Account/Login";
        return;
    }
    // Forbidden
    if (res.status === 403) {
        throw "Bạn không có quyền truy cập chức năng này.";
    }
    // API not found
    if (res.status === 404) {
        throw body?.message || "API không tồn tại.";
    }
    // Server Error
    if (res.status >= 500) {
        throw body?.message || "Lỗi máy chủ. Vui lòng thử lại sau.";
    }
    // Business Error (success = false)
    if (!body.success) {
        throw body?.message || "Xử lý thất bại.";
    }
    // SUCCESS
    return body;
}

//Gọi lần đầu khi load trang
loadDashboard();
loadAppointmentAreaChart();
loadAppointmentPieChart();

//Cập nhật lại mỗi 5s
setInterval(loadDashboard, 5000);