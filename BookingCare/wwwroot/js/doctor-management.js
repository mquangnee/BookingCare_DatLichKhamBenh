let currentPage = 1; //Trang hiện tại
const pageSize = 10; //Mỗi trang 10 dòng
let totalPages = 1; //Tổng số trang (sẽ tính lại sau khi gọi API)

const tableBody = document.getElementById("doctorTableBody");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");
const modalPatient = document.querySelector("#doctorModal #modalDoctor");

//Gọi API và render dữ liệu
async function loadDoctors(page = 1) {
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

//====DANH SÁCH BÁC SĨ====//
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

//====PHÂN TRANG BÁC SĨ====
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
        loadDoctors(currentPage);
    }
});

//Khi bấm “Sau”
nextBtn.addEventListener("click", () => {
    if (currentPage < totalPages) {
        currentPage++;//Tăng 1 trang
        loadDoctors(currentPage);
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

    html += `
        <p><strong>Mã bác sĩ: </strong>${data.doctorId}</p>
        <p><strong>Họ tên: </strong>${data.fullName}</p>
        <p><strong>Email: </strong>${data.email}</p>
        <p><strong>Số điện thoại: </strong>${data.phoneNumber}</p>
        <p><strong>Ngày sinh: </strong>${data.dateOfBirth}</p>
        <p><strong>Giới tính: </strong>${data.gender}</p>
        <p><strong>Địa chỉ: </strong>${data.address}</p>
        <p><strong>Bằng cấp: </strong>${data.degree}</p>
        <p><strong>Chuyên khoa: </strong>${data.specialtyName}</p>
        <p><strong>Số năm kinh nghiệm: </strong>${data.yearsOfExp}</p>
        <p><strong>Phòng khám: </strong>${data.roomName}</p>
    </div>
    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
    </div>`;

    modalPatient.innerHTML = html;
    $('#doctorModal').modal('show');
}

//Lần đầu load
loadDoctors(currentPage);

//====THÊM BÁC SĨ====//
const btnAddDoctor = document.getElementById("btnAddDoctor");
btnAddDoctor.addEventListener("click", async function () {
    //Lấy dữ liệu từ modal
    const body = {
        email: document.getElementById("email").value.trim(),
        fullName: document.getElementById("fullName").value.trim(),
        gender: document.getElementById("gender").value,
        dateOfBirth: document.getElementById("dateOfBirth").value,
        address: document.getElementById("address").value.trim(),
        phoneNumber: document.getElementById("phoneNumber").value.trim(),
        specialtyId: document.getElementById("specialty").value,
        degree: document.getElementById("degree").value,
        yearsOfExp: document.getElementById("yearsOfExp").value,
        roomId: document.getElementById("room").value
    };

    if (!body.email || !body.fullName || !body.gender || !body.dateOfBirth || !body.address || !body.phoneNumber || !body.specialtyId || !body.degree || !body.yearsOfExp || !body.roomId) {
        alert("Vui lòng điền đầy đủ thông tin!");
        return; // Không gọi API nữa
    }

    try {
        //Gửi yêu cầu thêm tài khoản bác sĩ đến server
        const res = await fetch(`/Admin/api/UserManagementApi/create`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        //Xử lý phản hồi từ server
        const result = await res.json();

        if (result.success) {
            alert(result.message);
            document.getElementById("formAddDoctor").reset(); //Làm rỗng modal
            $('#addDoctorModal').modal('hide');
            loadDoctors();
        } else {
            alert(result.message);
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});

//====LOAD DROPDOWNS CHUYÊN KHOA, PHÒNG CHỨC NĂNG THÊM BÁC SĨ====//
//Khi modal "Thêm bác sĩ" mở
$('#addDoctorModal').on('shown.bs.modal', function (e) {
    loadDropdowns();
});

//Hàm loadDropdowns
async function loadDropdowns() {
    try {
        const specialtySelect = document.getElementById("specialty");
        const roomSelect = document.getElementById("room");

        // Reset options
        specialtySelect.innerHTML = `<option value="">-- Chọn chuyên khoa --</option>`;
        roomSelect.innerHTML = `<option value="">-- Chọn phòng khám --</option>`;

        // Gọi API chuyên khoa
        const resSpecialty = await fetch("/Admin/api/SpecialtyApi/getAll");
        if (!resSpecialty.ok) {
            alert("Không thể tải danh sách chuyên khoa");
        }
        const specialties = await resSpecialty.json();

        //Thêm dropdown chuyên khoa
        specialties.forEach(specialty => {
            const opt = document.createElement("option");
            opt.value = specialty.id;
            opt.textContent = specialty.name;
            specialtySelect.appendChild(opt);
        });

        // Gọi API phòng
        const resRoom = await fetch("/Admin/api/RoomApi/getAll");
        if (!resRoom.ok) {
            alert("Không thể tải danh sách phòng khám");
        }
        const rooms = await resRoom.json();

        //Thêm dropdown phòng khám
        rooms.forEach(room => {
            const opt = document.createElement("option");
            opt.value = room.id;
            opt.textContent = `${room.name} (${room.currentDoctorCount}/2)`;
            roomSelect.appendChild(opt);
        });
    } catch (error) {
        console.error("Lỗi loadDropdowns:", error);
        alert("Không thể tải dữ liệu chuyên khoa hoặc phòng khám!");
    }
}

//Khóa tài khoản
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".lock-account"); //Tìm đúng nút "Khóa"
    if (!btn) return;

    const doctorId = btn.dataset.id; //Lấy giá trị data-id
    if (!doctorId) {
        alert("Không thể lấy Id bác sĩ!");
        return;
    }

    try {
        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/Admin/api/UserManagementApi/lock/${doctorId}`, { method: "PUT" });

        //Thông tin chi tiết
        const data = await res.json();

        //Hiển thị thông báo
        alert(data.message);
        loadDoctors();
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});

//Mở khóa tài khoản
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".unlock-account"); //Tìm đúng nút "Mở khóa"
    if (!btn) return;

    const doctorId = btn.dataset.id; //Lấy giá trị data-id
    if (!doctorId) {
        alert("Không thể lấy Id bác sĩ!");
        return;
    }

    try {
        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/Admin/api/UserManagementApi/unlock/${doctorId}`, { method: "PUT" });

        //Thông tin chi tiết
        const data = await res.json();

        //Hiển thị thông báo
        alert(data.message);
        loadDoctors();
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});