let currentPage = 1; //Trang hiện tại
const pageSize = 10; //Mỗi trang 10 dòng
let totalPages = 1; //Tổng số trang (sẽ tính lại sau khi gọi API)

const tableBody = document.getElementById("doctorTableBody");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");
const modalPatient = document.querySelector("#doctorModal #modalDoctor");

//Gọi API và render dữ liệu
async function loadPatients(page = 1) {
    try {
        const res = await fetch(`/Admin/api/UserManagementApi/doctors?page=${page}&pageSize=${pageSize}`);
        const result = await res.json();

        // Cập nhật lại tổng số trang
        totalPages = Math.ceil(result.totalDoctors / pageSize);
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
                <button class="dropdown-item edit-doctor" data-id="${d.userId}">Chỉnh sửa</button>
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

    const doctorId = btn.dataset.id; //Lấy giá trị data-id
    if (!doctorId) {
        alert("Không thể lấy Id bác sĩ!");
        return;
    }

    try {
        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/Admin/api/UserManagementApi/infoDoctor?id=${doctorId}`);

        //Thông tin chi tiết
        const data = await res.json();

        console.log("Trạng thái response:", res.status);
        console.log("Response URL:", res.url);


        //Kiểm tra dữ liệu
        if (!res.ok) {
            alert("Không thể lấy thông tin bác sĩ!");
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
            <h5 class="modal-title" > Thông tin bác sĩ</h5>
            <button type="button" class="close" data-dismiss="modal">&times;</button>
        </div>
        <div class="modal-body">`;

    //Dữ liệu trống
    if (!data || data.length == 0) {
        html += `
            <p><strong>Không thể lấy thông tin bác sĩ</strong></p>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
        </div>`;
        modalPatient.innerHTML = html;
        $('#doctorModal').modal('show');
        return;
    }

    //Dữ liệu từ BE
    const doctorData = data.result;

    //Thông tin chi tiết
    const doctorId = doctorData.doctors.id;
    const fullName = doctorData.fullName;
    const email = doctorData.email;
    const phoneNumber = doctorData.phoneNumber;
    const dateOfBirth = new Date(doctorData.dateOfBirth).toLocaleString("vi-VN");
    const gender = doctorData.gender;
    const address = doctorData.address;
    const degree = doctorData.doctors.degree;
    const specialty = doctorData.specialties.name;
    const yearsOfExp = doctorData.doctors.yearsOfExp;
    const room = doctorData.rooms.name;

    html += `
        <p><strong>Mã bác sĩ: </strong>${doctorId}</p>
        <p><strong>Họ tên: </strong>${fullName}</p>
        <p><strong>Email: </strong>${email}</p>
        <p><strong>Số điện thoại: </strong>${phoneNumber}</p>
        <p><strong>Ngày sinh: </strong>${dateOfBirth}</p>
        <p><strong>Giới tính: </strong>${gender}</p>
        <p><strong>Địa chỉ: </strong>${address}</p>
        <p><strong>Bằng cấp: </strong>${degree}</p>
        <p><strong>Chuyên khoa: </strong>${specialty}</p>
        <p><strong>Số năm kinh nghiệm: </strong>${yearsOfExp}</p>
        <p><strong>Phòng khám: </strong>${room}</p>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
    </div>`;

    modalPatient.innerHTML = html;
    $('#doctorModal').modal('show');
}

//Lần đầu load
loadPatients(currentPage);