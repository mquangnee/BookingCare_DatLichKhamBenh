async function loadAppointmentAreaChart() {
    try {
        const res = await fetch("/Admin/api/DashboardApi/booking-last-10days-1");
        const data = await res.json();

        const labels = data.map(x => x.date);
        const values = data.map(x => x.total);

        const ctx = document.getElementById("bookingChart");
        new Chart(ctx, {
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
    } catch (err) {
        console.error("Lỗi tải dữ liệu biểu đồ:", err);
    }
}

async function loadAppointmentPieChart() {
    try {
        const res = await fetch("/Admin/api/DashboardApi/booking-last-10days-2");
        const data = await res.json();

        const ctx = document.getElementById("appointmentPieChart");
        const totalCount = document.getElementById("totalCount");

        totalCount.textContent = "Tổng lịch khám: " + data.totalCount;

        new Chart(ctx, {
            type: "doughnut", // có thể đổi sang "pie" nếu bạn thích
            data: {
                labels: ["Chờ khám", "Đã khám", "Đã hủy"],
                datasets: [{
                    label: "Lịch khám (10 ngày gần nhất)",
                    data: [data.waitingCount, data.successCount, data.canceledCount],
                    backgroundColor: [
                        "rgba(75, 192, 192, 0.8)", // xanh ngọc
                        "rgba(255, 205, 86, 0.8)",  // vàng
                        "rgba(255, 99, 132, 0.8)"   // đỏ
                    ],
                    borderColor: [
                        "rgba(75, 192, 192, 1)",
                        "rgba(255, 205, 86, 1)",
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
        console.error("Lỗi khi tải biểu đồ:", error);
        alert("Không thể tải dữ liệu biểu đồ!");
    }
}

//Gọi lần đầu khi load trang
loadAppointmentAreaChart();
loadAppointmentPieChart();