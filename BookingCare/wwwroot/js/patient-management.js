let currentPage = 1; //Trang hiện tại
const pageSize = 10; //Mỗi trang 10 dòng
let totalPages = 1; //Tổng số trang (sẽ tính lại sau khi gọi API)

const tableBody = document.getElementById("patientTableBody");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");
const modalPatient = document.querySelector("#patientModal #modalPatient");

//Gọi API và render dữ liệu
async function loadPatients(page = 1) {
    try {
        const res = await fetch(`/Admin/api/UserManagementApi/patients?page=${page}&pageSize=${pageSize}`);
        const result = await res.json();

        // Cập nhật lại tổng số trang
        totalPages = Math.ceil(result.totalPatients / pageSize);
        renderTable(result.data);
        updatePagination(page);

    } catch (err) {
        console.error("Lỗi tải dữ liệu:", err);
    }
}

//Render bảng
function renderTable(data) {
    if (!data || data.length == 0) {
        tableBody.innerHTML = '<tr><td colspan="8">Không có dữ liệu</td></tr>';
        return;
    }

    tableBody.innerHTML = data.map(d => {
        const createdAt = new Date(d.createdAt.split('.')[0]).toLocaleString("vi-VN");
        const updatedAt = d.updatedAt ? new Date(d.updatedAt.split('.')[0]).toLocaleString("vi-VN") : "Chưa cập nhật";

        //Kiểm tra trạng thái khóa tài khoản
        const isLocked = d.isLocked && new Date(d.isLocked) > new Date();

        //Trạng thái hiển thị badge
        const statusHtml = isLocked
            ? `<span class="text-white badge bg-secondary">Bị khóa</span>`
            : `<span class="text-white badge bg-success">Hoạt động</span>`;

        //Dropdown action 
        const actionHtml = `
        <div class="btn-group">
            <button type="button" class="btn btn-sm btn-secondary dropdown-toggle" data-toggle="dropdown">
                <i class="fa-solid fa-bars"></i>
            </button>
            <div class="dropdown-menu">
                ${isLocked
                ? `<button class="dropdown-item unlock-account" data-id="${d.userId}">Mở khóa</button>`
                : `<button class="dropdown-item lock-account" data-id="${d.userId}">Khóa</button>`}
                <button class="dropdown-item view-details" data-id="${d.userId}">Xem chi tiết</button>
            </div>
        </div>`;

        return `
        <tr>
            <td>${d.id}</td>
            <td>${d.fullName}</td>
            <td>${d.email}</td>
            <td>${d.phoneNumber ?? ""}</td>
            <td>${createdAt}</td>
            <td>${updatedAt}</td>
            <td>${statusHtml}</td>
            <td>${actionHtml}</td>
        </tr>`;
    }).join("");
}

//Cập nhật hiển thị phân trang
function updatePagination(page) {
    currentPage = page;//Ghi lại số trang hiện tại
    pageInfo.textContent = `Trang ${currentPage} / ${totalPages}`;

    //Ẩn hoặc disable nút khi cần
    prevBtn.disabled = currentPage <= 1;
    nextBtn.disabled = currentPage >= totalPages;
}

//Khi bấm “Trước”
prevBtn.addEventListener("click", () => {
    if (currentPage > 1) {
        currentPage--;//Giảm 1 trang
        loadPatients(currentPage);
    }
});

//Khi bấm “Sau”
nextBtn.addEventListener("click", () => {
    if (currentPage < totalPages) {
        currentPage++;//Tăng 1 trang
        loadPatients(currentPage);
    }
});

//Xem thông tin chi tiết
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".view-details"); //Tìm đúng nút "Xem chi tiết"
    if (!btn) return;

    const patientId = btn.dataset.id; //Lấy giá trị data-id
    if (!patientId) {
        alert("Không thể lấy Id bệnh nhân!");
        return;
    }

    try {
        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/Admin/api/UserManagementApi/infoPatient?id=${patientId}`);

        //Thông tin chi tiết
        const data = await res.json();

        //Kiểm tra dữ liệu
        if (!res.ok) {
            alert("Không thể lấy thông tin bệnh nhân!");
            return;
        }
        renderInfo(data);
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});

//Render thông tin chi tiết
function renderInfo(data) {
    let html = `
        <div class="modal-header">
            <h5 class="modal-title">Thông tin bệnh nhân</h5>
            <button type="button" class="close" data-dismiss="modal">&times;</button>
        </div>
        <div class="modal-body">`;

    //Dữ liệu trống
    if (!data || data.length == 0) {
        html += `
            <p><strong>Không thể lấy thông tin bệnh nhân</strong></p>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
        </div>`;
        modalPatient.innerHTML = html;
        $('#patientModal').modal('show');
        return;
    }

    //Dữ liệu từ BE
    const patientData = data.result;

    //Thông tin chi tiết
    const patientId = patientData.patients.id;
    const fullName = patientData.fullName;
    const email = patientData.email;
    const phoneNumber = patientData.phoneNumber;
    const dateOfBirth = new Date(patientData.dateOfBirth).toLocaleString("vi-VN");
    const gender = patientData.gender;
    const address = patientData.address;
    const medicalHistory = patientData.patients.medicalHistory;

    html += `
        <p><strong>Mã bác sĩ: </strong>${patientId}</p>
        <p><strong>Họ tên: </strong>${fullName}</p>
        <p><strong>Email: </strong>${email}</p>
        <p><strong>Số điện thoại: </strong>${phoneNumber}</p>
        <p><strong>Ngày sinh: </strong>${dateOfBirth}</p>
        <p><strong>Giới tính: </strong>${gender}</p>
        <p><strong>Địa chỉ: </strong>${address}</p>
        <p><strong>Tiền sử bệnh: </strong>${medicalHistory}</p>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
    </div>`;

    modalPatient.innerHTML = html;
    $('#patientModal').modal('show');
}

//Lần đầu load
loadPatients(currentPage);