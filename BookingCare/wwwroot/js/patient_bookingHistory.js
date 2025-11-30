let currentPage = 1; //Trang hiện tại
const pageSize = 10; //Mỗi trang 10 dòng
let totalPages = 1; //Tổng số trang (sẽ tính lại sau khi gọi API)
let nameKeyword = "";  //Từ khóa dùng cho search + phân trang
let filter = "Tất cả";
let timer;

const searchInput = document.getElementById("searchInput");
const tableBody = document.getElementById("apptTableBody");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");

//====DANH SÁCH LỊCH ĐẶT====//
//Gọi Api và render dữ liệu
async function loadAppointments(page = 1, keyword = nameKeyword, filterSelect = filter) {
    try {
        currentPage = page;
        nameKeyword = keyword;
        filter = filterSelect;

        //Gửi yêu cầu lấy dữ liệu về server
        const res = await fetch(`/Patient/api/BookingHistoryApi/bookingHistory?page=${page}&pageSize=${pageSize}&search=${nameKeyword}&filter=${filter}`);
        const result = await res.json();

        //Hiển thị thông báo (nếu lỗi)
        if (!res.ok) {
            alert(data.message);
            return
        }
        
        //Cập nhật lại tổng số trang
        totalPages = Math.ceil(result.totalAppt / pageSize);
        renderTable(result.data);
        updatePagination();
    } catch (err) {
        alert('Không thể kết nối dữ liệu. Vui lòng thử lại.');
        console.error(err);
    }
}

//Tìm kiếm theo tên bác sĩ
if (searchInput) {
    searchInput.addEventListener("keyup", function () {
        clearTimeout(timer);
        timer = setTimeout(() => {
            const keyword = this.value.trim();
            loadAppointments(1, keyword); //Tìm kiếm từ trang 1
        }, 300); //Chờ 300ms sau khi gõ mới gửi yêu cầu
    });
}

//Tìm kiếm theo trạng thái
document.getElementById("statusFilter").addEventListener("change", function () {
    loadAppointments(1, nameKeyword, this.value); //Tìm kiếm từ trang 1
});

//Xuất bảng
function renderTable(data) {
    if (!data || data.length == 0) {
        tableBody.innerHTML = '<tr><td colspan="8">Không có dữ liệu</td></tr>';
        return;
    }

    tableBody.innerHTML = data.map((d, index) => {
        //STT
        const stt = (currentPage - 1) * pageSize + index + 1;
        
        //Format ngày đặt
        const appointmentDate = new Date(d.appointmentDate).toLocaleDateString("vi-VN");

        //Hiển thị action theo trạng thái lịch đặt
        const actionHtml = getActionButton(d.status, d.appointmentId);

        //Hiển thị trạng thái
        const statusHtml = getStatusBadge(d.status, d.appointmentId);

        return `
        <tr>
            <td>${stt}</td>
            <td>${appointmentDate}</td>
            <td>${d.appointmentTime}</td>
            <td>${d.doctorName}</td>
            <td>${d.roomName}</td>
            <td>${d.reasonForVisit}</td>
            <td>${statusHtml}</td>
            <td>${actionHtml}</td>
        </tr>`;
    }).join("");
}

//Hiển thị nút theo trạng thái
function getActionButton(status, appointmentId) {
    if (status == "Chờ khám") {
        return `
            <button class="btn btn-danger btn-sm cancel-appointment" data-id="${appointmentId}">
                <i class="fa-solid fa-ban"></i> Hủy lịch
            </button>
        `;
    }

    //Disable nút
    return `
        <button class="btn btn-secondary btn-sm" disabled>
            Hủy lịch
        </button>
    `;
}

//Hiển thị trạng thái
function getStatusBadge(status, appointmentId) {
    switch (status) {
        case "Chờ khám":
            return `<span id="appt-${appointmentId}" class="badge-status badge-pending">${status}</span>`;
        case "Đang khám":
            return `<span id="appt-${appointmentId}" class="badge-status badge-info">${status}</span>`;
        case "Hoàn thành":
            return `<span id="appt-${appointmentId}" class="badge-status badge-confirmed">${status}</span>`;
        case "Đã hủy":
            return `<span id="appt-${appointmentId}" class="badge-status badge-canceled">${status}</span>`;
    }
}

//====PHÂN TRANG LỊCH ĐẶT====//
//Cập nhật hiển thị phân trang
function updatePagination() {
    pageInfo.textContent = `Trang ${currentPage} / ${totalPages}`;
    prevBtn.disabled = currentPage <= 1;
    nextBtn.disabled = currentPage >= totalPages;
}
prevBtn.addEventListener("click", () => {
    if (currentPage > 1) loadAppointments(currentPage - 1);
});
nextBtn.addEventListener("click", () => {
    if (currentPage < totalPages) loadAppointments(currentPage + 1);
});

//Lần đầu load
loadAppointments(currentPage);

//===HỦY LỊCH ĐẶT===//
const cancelApptId = document.getElementById("cancelApptId");
const cancelBtn = document.getElementById("confirmCancelBtn");

//1. Hiển thị xác nhận
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".cancel-appointment"); //Tìm đúng nút "Hủy"
    if (!btn) return;

    //Lưu ID lịch khám vào modal xác nhận
    const appointmentId = btn.dataset.id;
    cancelApptId.value = appointmentId;

    //Hiển thị modal
    $('#confirmCancelModal').modal("show");
});
//2. Hủy lịch hẹn
cancelBtn.addEventListener("click", async function () {
    const apptId = cancelApptId.value;
    if (!apptId) {
        alert("ID lịch hẹn không hợp lệ!");
        return;
    }
    try {
        //Gửi yêu cầu hủy lịch về server
        const res = await fetch(`/Patient/api/BookingHistoryApi/cancelBooking/${apptId}`, { method: "PUT" });
        const data = await res.json();

        //Hiển thị thông báo
        $('#confirmCancelModal').modal('hide');
        alert(data.message);
        loadAppointments();
    } catch (err) {
        alert('Không thể kết nối dữ liệu. Vui lòng thử lại.');
        console.error(err);
    }
});
//===CẬP NHẬT TRẠNG THÁI LỊCH HẸN===//
//Kết nối với hub
const connection = new signalR.HubConnectionBuilder().withUrl("/appointmentHub").build();
connection.start().then(() => {
    console.log("SignalR Connected");
});

//Cập nhật giao diện
connection.on("StatusChanged", (appointmentId, newStatus) => {
    const status = document.getElementById(`appt-${appointmentId}"`);
    status.textContent = newStatus;
    if (newStatus == "Đang khám") {
        status.className = "badge-status badge-info`";
    }
    if (newStatus == "Hoàn thành") {
        status.className = "badge-status badge-confirmed";
    }
});