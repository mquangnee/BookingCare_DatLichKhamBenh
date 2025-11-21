let currentPage = 1; //Trang hiện tại
const pageSize = 10; //Mỗi trang 10 dòng
let totalPages = 1; //Tổng số trang (sẽ tính lại sau khi gọi API)
let doctorKeyword = "";  // từ khóa dùng cho search + phân trang

const tableBody = document.getElementById("doctorTableBody");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");
const modalDoctor = document.querySelector("#doctorModal #modalDoctor");

//====DANH SÁCH BÁC SĨ====//
//Gọi API và render dữ liệu
async function loadDoctors(page = 1, keyword = doctorKeyword) {
    try {
        doctorPage = page;
        doctorKeyword = keyword;

        const res = await fetch(`/Admin/api/UserApi/doctors?page=${page}&pageSize=${pageSize}&search=${doctorKeyword}`);
        const result = await res.json();

        // Cập nhật lại tổng số trang
        totalPages = Math.ceil(result.totalDoctors / pageSize);
        renderTable(result.data);
        updatePagination();

    } catch (err) {
        console.error("Lỗi tải dữ liệu:", err);
    }
}

//Tìm kiếm bác sĩ
function searchDoctors(keyword) {
    loadDoctors(1, keyword);
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
                <button class="dropdown-item edit-doctor" data-id="${d.userId}" data-toggle="modal" data-target="#updateDoctorModal">Chỉnh sửa</button>
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

//Lần đầu load
loadDoctors(currentPage);

//====PHÂN TRANG BÁC SĨ====//
//Cập nhật hiển thị phân trang
function updatePagination() {
    pageInfo.textContent = `Trang ${currentPage} / ${totalPages}`;

    prevBtn.disabled = currentPage <= 1;
    nextBtn.disabled = currentPage >= totalPages;
}

prevBtn.addEventListener("click", () => {
    if (currentPage > 1) loadDoctors(currentPage - 1);
});

nextBtn.addEventListener("click", () => {
    if (currentPage < totalPages) loadDoctors(currentPage + 1);
});


//====XEM THÔNG TIN CHI TIẾT BÁC SĨ====//
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
        const res = await fetch(`/Admin/api/UserApi/infoDoctor?id=${doctorId}`);

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
        modalDoctor.innerHTML = html;
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

    modalDoctor.innerHTML = html;
    $('#doctorModal').modal('show');
}

//====THÊM BÁC SĨ====//
const btnAddDoctor = document.getElementById("btnAddDoctor");
btnAddDoctor.addEventListener("click", async function () {
    //Lấy dữ liệu từ modal
    const body = {
        email: document.getElementById("create_email").value.trim(),
        fullName: document.getElementById("create_fullName").value.trim(),
        gender: document.getElementById("create_gender").value,
        dateOfBirth: document.getElementById("create_dateOfBirth").value,
        address: document.getElementById("create_address").value.trim(),
        phoneNumber: document.getElementById("create_phoneNumber").value.trim(),
        specialtyId: document.getElementById("create_specialty").value,
        degree: document.getElementById("create_degree").value,
        yearsOfExp: document.getElementById("create_yearsOfExp").value,
        roomId: document.getElementById("create_room").value
    };

    try {
        //Gửi yêu cầu thêm tài khoản bác sĩ đến server
        const res = await fetch(`/Admin/api/UserApi/create`, {
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

//Khi modal "Thêm bác sĩ" mở
$('#addDoctorModal').on('shown.bs.modal', async function (e) {
    await loadDropdownsCreate();
});

//====CHỈNH SỬA THÔNG TIN BÁC SĨ====//
//1. Hiển thị modal chỉnh sửa thông tin bác sĩ
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".edit-doctor"); //Tìm đúng nút "Chỉnh sửa"
    if (!btn) return;

    await loadDropdownsUpdate();

    const doctorId = btn.dataset.id; //Lấy giá trị data-id
    if (!doctorId) {
        alert("Không thể lấy Id bác sĩ!");
        return;
    }

    try {
        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/Admin/api/UserApi/updateInfoDoctor?id=${doctorId}`);

        //Thông tin chi tiết
        const data = await res.json();

        //Kiểm tra dữ liệu
        if (!res.ok) {
            alert("Không thể lấy thông tin bác sĩ!");
            return;
        }

        //Lấy các thành phần trong modal
        const email = document.getElementById("update_email");
        const fullName = document.getElementById("update_fullName");
        const gender = document.getElementById("update_gender");
        const dateOfBirth = document.getElementById("update_dateOfBirth");
        const address = document.getElementById("update_address");
        const phoneNumber = document.getElementById("update_phoneNumber");
        const specialty = document.getElementById("update_specialty");
        const degree = document.getElementById("update_degree");
        const yearsOfExp = document.getElementById("update_yearsOfExp");
        const room = document.getElementById("update_room");

        //Lưu user ID
        document.getElementById("update_userId").value = data.result.userId;

        //Hiển thị thông tin
        email.value = data.result.email;
        fullName.value = data.result.fullName;
        gender.value = data.result.gender;
        dateOfBirth.value = data.result.dateOfBirth;
        address.value = data.result.address;
        phoneNumber.value = data.result.phoneNumber;
        specialty.value = data.specialtyId.toString();
        degree.value = data.result.degree;
        yearsOfExp.value = data.result.yearsOfExp;
        room.value = data.roomId.toString();

        //Hiển thị modal
        $("#updateDoctorModal").modal("show");

    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});

//2. Cập nhật thông tin
document.addEventListener("click", async function (e) {
    const btn = e.target.closest("#btnUpdateDoctor"); //Tìm đúng nút "Lưu thông tin"
    if (!btn) return;

    const userId = document.getElementById("update_userId").value; //Lấy giá trị userId
    if (!userId) {
        alert("Không thể lấy Id bác sĩ!");
        return;
    }

    try {
        //Lấy dữ liệu từ modal
        const body = {
            address: document.getElementById("update_address").value.trim(),
            phoneNumber: document.getElementById("update_phoneNumber").value.trim(),
            specialtyId: document.getElementById("update_specialty").value,
            degree: document.getElementById("update_degree").value,
            yearsOfExp: document.getElementById("update_yearsOfExp").value,
            roomId: document.getElementById("update_room").value
        };

        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/Admin/api/UserApi/update/${userId}`, { 
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        //Xử lý phản hồi từ server
        const result = await res.json();

        if (result.success) {
            alert(result.message);
            document.getElementById("formUpdateDoctor").reset(); //Làm rỗng modal
            $('#updateDoctorModal').modal('hide');
            loadDoctors(currentPage);
        } else {
            alert(result.message);
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});

//====KHÓA/MỞ KHÓA TÀI KHOẢN BÁC SĨ====//
const lockId = document.getElementById("lockDoctorId");
const unlockId = document.getElementById("unlockDoctorId");
const btnLock = document.getElementById("confirmLockBtn");
const btnUnlock = document.getElementById("confirmUnlockBtn");

/* Khóa bác sĩ */
//1. Hiển thị modal xác nhận khóa
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".lock-account"); //Tìm đúng nút "Khóa"
    if (!btn) return;

    //Lưu Id bác sĩ
    const doctorId = btn.dataset.id;
    lockId.value = doctorId;

    //Hiển thị modal
    $('#confirmLockModal').modal('show');
});
//2. Khóa bác sĩ
btnLock.addEventListener("click", async function (e) {
    //Lấy Id bác sĩ
    const doctorId = lockId.value;
    if (!doctorId) {
        alert("Không thể lấy Id bác sĩ!");
        return;
    }

    try {
        //Gửi yêu cầu khóa tài khoản bác sĩ về server
        const res = await fetch(`/Admin/api/UserApi/lock/${doctorId}`, { method: "PUT" });

        //Thông tin chi tiết
        const data = await res.json();

        //Hiển thị thông báo
        $('#confirmLockModal').modal('hide');
        alert(data.message);
        loadDoctors();
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});

/* Mở khóa bác sĩ */
//1. Hiển thị modal xác nhận mở khóa
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".unlock-account"); //Tìm đúng nút "Mở khóa"
    if (!btn) return;

    //Lưu Id bác sĩ
    const doctorId = btn.dataset.id;
    unlockId.value = doctorId;

    //Hiển thị modal
    $('#confirmUnlockModal').modal('show');
});
//2. Mở khóa bác sĩ
btnUnlock.addEventListener("click", async function (e) {
    //Lấy Id bác sĩ
    const doctorId = unlockId.value;
    if (!doctorId) {
        alert("Không thể lấy Id bác sĩ!");
        return;
    }

    try {
        //Gửi yêu cầu mở khóa tài khoản bác sĩ về server
        const res = await fetch(`/Admin/api/UserApi/unlock/${doctorId}`, { method: "PUT" });

        //Thông tin chi tiết
        const data = await res.json();

        //Hiển thị thông báo
        $('#confirmUnlockModal').modal('hide');
        alert(data.message);
        loadDoctors();
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});

//====HÀM LOAD DROPDOWNS====//
async function loadDropdownsCreate() {
    try {
        const specialtySelect = document.getElementById("create_specialty");
        const roomSelect = document.getElementById("create_room");

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

async function loadDropdownsUpdate() {
    try {
        const specialtySelect = document.getElementById("update_specialty");
        const roomSelect = document.getElementById("update_room");

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