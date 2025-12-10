// Hiển thị danh sách lịch đặt và phân trang
let currentPage = 1;
let totalPages = 1;
const pageSize = 10;
const tableBody = document.getElementById("appointmentsTable");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");
// Tìm kiếm và lọc
let nameKeyword = "";
let selectedDate = "";
let filter = "Tất cả";
let timer;
const searchInput = document.getElementById("searchBox");
const dateFilter = document.getElementById("dateFilter");
const statusFilter = document.getElementById("statusFilter");
// Modal thông tin chi tiết lịch khám
const modalAppointmentDetail = document.querySelector("#AppointmentDetailModal #modalAppointmentDetail");
const patientName = document.getElementById("PatientName");
const patientDob = document.getElementById("DateOfBirth");
const gender = document.getElementById("Gender");
const appointmentDate = document.getElementById("AppointmentDate");
const appointmentTime = document.getElementById("AppointmentTime");
const medicalHistory = document.getElementById("MedicalHistory");
const reasonForVisit = document.getElementById("ReasonForVisit");
const medReportBtn = document.getElementById("MedReportBtn");

// Hiển thị danh sách khi tải trang
document.addEventListener("DOMContentLoaded", function () {
    selectedDate = new Date().toISOString().split("T")[0];
    dateFilter.value = selectedDate;

    // Mặc định load danh sách lịch đặt hôm nay
    loadAppointments(1, selectedDate, nameKeyword, filter);

    dateFilter.addEventListener("change", function () {
        selectedDate = this.value;
        loadAppointments(1, selectedDate, nameKeyword, filter);
    });
    statusFilter.addEventListener("change", function () {
        filter = this.value;
        loadAppointments(1, selectedDate, nameKeyword, filter);
    });
    searchInput.addEventListener("keyup", function () {
        clearTimeout(timer);
        timer = setTimeout(() => {
            nameKeyword = this.value.trim();
            loadAppointments(1, selectedDate, nameKeyword, filter);
        }, 300);
    });
});

// Load danh sách lịch đặt
async function loadAppointments(page = 1, date = selectedDate, keyword = "", filterSelect = "Tất cả") {
    try {
        selectedDate = date;
        currentPage = page;
        nameKeyword = keyword;
        filter = filterSelect;

        // Lấy dữ liệu từ server
        const res = await fetch(`/api/doctor/home?date=${selectedDate}&page=${page}&pageSize=${pageSize}&search=${nameKeyword}&filter=${filter}`);
        const body = await handleResponse(res);

        // Hiển thị dữ liệu
        const data = body.data;
        totalPages = Math.ceil(data.totalAppointments / pageSize);
        renderTable(data.listAppointments);
        updatePagination();
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error);
    }
}

//Xuất bảng
function renderTable(data) {
    if (!data || data.length == 0) {
        tableBody.innerHTML = '<tr><td colspan="8">Không có lịch hẹn khám hôm nay</td></tr>';
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
            <td>${d.patientName}</td>
            <td>${d.reasonForVisit}</td>
            <td style="display: flex; justify-content: center">${statusHtml}</td>
            <td>${actionHtml}</td>
        </tr>`;
    }).join("");
}

//Hiển thị nút theo trạng thái
function getActionButton(status, appointmentId) {
    if (status == "Chờ khám") {
        return `
            <button class="btn btn-info btn-sm rounded-pill detail-appointment" data- data-id="${appointmentId}">
                <i class="fa-solid fa-circle-info"></i> Chi tiết
            </button>
            <button class="btn btn-danger btn-sm rounded-pill cancel-appointment" data-id="${appointmentId}">
                <i class="fa-solid fa-ban"></i> Hủy lịch
            </button>`;
    }
    //Disable nút
    return `
        <button class="btn btn-info btn-sm rounded-pill detail-appointment" data-id="${appointmentId}">
                <i class="fa-solid fa-circle-info"></i> Chi tiết
        </button>
        <button class="btn btn-secondary btn-sm rounded-pill disabled">Hủy lịch</button>`;
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
        const res = await fetch(`/api/doctor/home/${apptId}`, { method: "PUT" });
        const body = await handleResponse(res);

        //Hiển thị thông báo
        $('#confirmCancelModal').modal('hide');
        alert(body.message);
        loadAppointments();
    } catch (err) {
        alert('Không thể kết nối dữ liệu. Vui lòng thử lại.');
        console.error(err);
    }
});

// Xem chi tiết lịch đặt và thông tin bệnh nhân
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".detail-appointment"); //Tìm đúng nút "Xem chi tiết"
    if (!btn) return;
    const appointmentId = btn.dataset.id; //Lấy giá trị data-id
    if (!appointmentId) {
        alert("Không thể lấy thông tin lịch khám!");
        return;
    }
    try {
        // Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/api/doctor/appointment-detail/${appointmentId}`);
        const body = await handleResponse(res);

        //Thông tin chi tiết
        const data = body.data;
        renderInfo(data);
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
});

//Render thông tin chi tiết
function renderInfo(data) {
    //Dữ liệu trống
    if (!data || data.length == 0) {
        alert("Không thể lấy thông tin lịch khám")
        $('#AppointmentDetailModal').modal('hide');
        return;
    }
    let dateOfBirth = new Date(data.dateOfBirth).toLocaleDateString("vi-VN");
    let apptDate = new Date(data.appointmentDate).toLocaleDateString("vi-VN")
    patientName.textContent = data.patientName;
    patientDob.textContent = dateOfBirth;
    gender.textContent = data.gender;
    appointmentDate.textContent = apptDate;
    appointmentTime.textContent = data.appointmentTime;
    medicalHistory.textContent = data.medicalHistory;
    reasonForVisit.textContent = data.reasonForVisit;
    medReportBtn.dataset.apptId = data.appointmentId;
    if (data.status == "Chờ khám" || data.status == "Đã hủy") {
        medReportBtn.disabled = true;
    } else {
        medReportBtn.disabled = false;
    }
    $('#AppointmentDetailModal').modal('show');
}

// Chuyển sang trang trả kết quả
medReportBtn.addEventListener("click", function () {
    const apptId = medReportBtn.dataset.apptId;
    window.location.href = `/Doctor/MedicalReport/Index/${apptId}`;
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