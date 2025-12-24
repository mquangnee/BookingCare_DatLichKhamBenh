// Phân trang
let currentPage = 1; //Trang hiện tại
const pageSize = 10; //Mỗi trang 10 dòng
let totalPages = 1; //Tổng số trang (sẽ tính lại sau khi gọi API)
let doctorKeyword = "";  //Từ khóa dùng cho search + phân trang

// Lấy các thành phần trong View
// 1. Danh sách bác sĩ và phân trang
const tableBody = document.getElementById("doctorTableBody");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");
// 2. Modal xem thông tin chi tiết bác sĩ
const modalDoctor = document.querySelector("#doctorModal #modalDoctor");
// 3. Thêm tài khoản bác sĩ
const btnAddDoctor = document.getElementById("btnAddDoctor");
const formAddDoctor = document.getElementById("formAddDoctor");
const create_email = document.getElementById("create_email");
const create_fullName = document.getElementById("create_fullName");
const create_gender = document.getElementById("create_gender");
const create_dateOfBirth = document.getElementById("create_dateOfBirth");
const create_address = document.getElementById("create_address");
const create_phoneNumber = document.getElementById("create_phoneNumber");
const create_specialty = document.getElementById("create_specialty");
const create_degree = document.getElementById("create_degree");
const create_yearsOfExp = document.getElementById("create_yearsOfExp");
const create_room = document.getElementById("create_room");
// 4. Cập nhật thông tin bác sĩ
const formUpdateDoctor = document.getElementById("formUpdateDoctor");
const update_email = document.getElementById("update_email");
const update_fullName = document.getElementById("update_fullName");
const update_gender = document.getElementById("update_gender");
const update_dateOfBirth = document.getElementById("update_dateOfBirth");
const update_address = document.getElementById("update_address");
const update_phoneNumber = document.getElementById("update_phoneNumber");
const update_specialty = document.getElementById("update_specialty");
const update_degree = document.getElementById("update_degree");
const update_yearsOfExp = document.getElementById("update_yearsOfExp");
const update_room = document.getElementById("update_room");
const update_userId = document.getElementById("update_userId");
// 5. Khóa/Mở khóa tài khoản bác sĩ
const lockId = document.getElementById("lockDoctorId");
const unlockId = document.getElementById("unlockDoctorId");
const btnLock = document.getElementById("confirmLockBtn");
const btnUnlock = document.getElementById("confirmUnlockBtn");

//====DANH SÁCH BÁC SĨ====//
// Tải danh sách bác sĩ
async function loadDoctors(page = 1, keyword = doctorKeyword) {
    try {
        currentPage = page;
        doctorKeyword = keyword;

        //Gửi yêu cầu lấy dữ liệu về server
        const res = await fetch(`/api/admin/users/doctors?page=${page}&pageSize=${pageSize}&search=${doctorKeyword}`);
        const body = await handleResponse(res);

        // Cập nhật lại tổng số trang
        const data = body.data;
        totalPages = Math.ceil(data.totalDoctors / pageSize);
        renderTable(data.listDoctor);
        updatePagination();
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
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
        // Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(` /api/admin/users/doctors/${doctorId}`);
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
    let html = `
        <div class="modal-header">
            <h5 class="modal-title" > Thông tin bác sĩ</h5>
            <button type="button" class="close" data-dismiss="modal">&times;</button>
        </div>`;

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
        <div class="modal-body text-center">
            <img src="${data.avatarUrl ?? "/images/doctors/avatar_default.jpg"}" class="rounded-circle border shadow mb-3" style="width:130px;height:130px;object-fit:cover" />
        </div> 
        <div class="modal-body">
            <p><strong>Mã bác sĩ:</strong> ${data.doctorId}</p>
            <p><strong>Họ tên:</strong> ${data.fullName}</p>
            <p><strong>Email:</strong> ${data.email}</p>
            <p><strong>Số điện thoại:</strong> ${data.phoneNumber}</p>
            <p><strong>Ngày sinh:</strong> ${data.dateOfBirth}</p>
            <p><strong>Giới tính:</strong> ${data.gender}</p>
            <p><strong>Địa chỉ:</strong> ${data.address}</p>
            <p><strong>Bằng cấp:</strong> ${data.degree}</p>
            <p><strong>Chuyên khoa:</strong> ${data.specialtyName}</p>
            <p><strong>Số năm kinh nghiệm:</strong> ${data.yearsOfExp}</p>
            <p><strong>Phòng khám:</strong> ${data.roomName}</p>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-dismiss="modal">Đóng</button>
        </div>`;

    modalDoctor.innerHTML = html;
    $('#doctorModal').modal('show');
}

//====THÊM BÁC SĨ====//
btnAddDoctor.addEventListener("click", async function () {
    try {
        const formData = new FormData();

        formData.append("Email", create_email.value.trim());
        formData.append("FullName", create_fullName.value.trim());
        formData.append("Gender", create_gender.value);
        formData.append("DateOfBirth", create_dateOfBirth.value);
        formData.append("Address", create_address.value.trim());
        formData.append("PhoneNumber", create_phoneNumber.value.trim());
        formData.append("SpecialtyId", create_specialty.value);
        formData.append("Degree", create_degree.value);
        formData.append("YearsOfExp", create_yearsOfExp.value);
        formData.append("RoomId", create_room.value);

        const avatarFile = document.getElementById("create_avatar").files[0];
        if (avatarFile) {
            formData.append("Avatar", avatarFile);
        }

        const res = await fetch("/api/admin/users/doctors", {
            method: "POST",
            body: formData
        });

        const body = await handleResponse(res);

        alert(body.message);
        formAddDoctor.reset();
        $("#previewCreateAvatar").attr("src", "/images/default-avatar.png");
        $("#addDoctorModal").modal("hide");
        loadDoctors();

    } catch (error) {
        console.error("Lỗi:", error);
        alert(error);
    }
});

//Khi modal "Thêm bác sĩ" mở
$('#addDoctorModal').on('shown.bs.modal', async function () {
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
        const res = await fetch(`/api/admin/users/doctors/${doctorId}/edit`);
        const body = await handleResponse(res);

        //Lưu user ID
        const data = body.data;
        update_userId.value = data.userId;

        // Hiển thị avatar hiện tại
        document.getElementById("previewUpdateAvatar").src = data.avatarUrl ?? "/images/doctors/avatar_default.jpg";

        //Hiển thị thông tin
        update_email.value = data.email;
        update_fullName.value = data.fullName;
        update_gender.value = data.gender;
        update_dateOfBirth.value = data.dateOfBirth;
        update_address.value = data.address;
        update_phoneNumber.value = data.phoneNumber;
        update_specialty.value = data.specialtyId.toString();
        update_degree.value = data.degree;
        update_yearsOfExp.value = data.yearsOfExp;
        update_room.value = data.roomId.toString();

        //Hiển thị modal
        $("#updateDoctorModal").modal("show");
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
});

document.getElementById("update_avatar").addEventListener("change", function (e) {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById("previewUpdateAvatar").src = e.target.result;
    };
    reader.readAsDataURL(file);
});


//2. Cập nhật thông tin
document.addEventListener("click", async function (e) {
    const btn = e.target.closest("#btnUpdateDoctor");
    if (!btn) return;

    const userId = update_userId.value;
    if (!userId) {
        alert("Không thể lấy Id bác sĩ!");
        return;
    }

    try {
        const formData = new FormData();

        formData.append("Address", update_address.value.trim());
        formData.append("PhoneNumber", update_phoneNumber.value.trim());
        formData.append("SpecialtyId", update_specialty.value);
        formData.append("Degree", update_degree.value);
        formData.append("YearsOfExp", update_yearsOfExp.value);
        formData.append("RoomId", update_room.value);

        const avatarFile = document.getElementById("update_avatar").files[0];
        if (avatarFile) {
            formData.append("Avatar", avatarFile);
        }

        const res = await fetch(`/api/admin/users/doctors/${userId}/edit`, {
            method: "PUT",
            body: formData
        });

        const body = await handleResponse(res);

        alert(body.message);
        formUpdateDoctor.reset();
        $("#updateDoctorModal").modal("hide");
        loadDoctors(currentPage);

    } catch (error) {
        console.error("Lỗi:", error);
        alert(error);
    }
});

//====KHÓA/MỞ KHÓA TÀI KHOẢN BÁC SĨ====//
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
        const res = await fetch(`/api/admin/users/lock/${doctorId}`, { method: "PUT" });
        const body = await handleResponse(res);

        //Hiển thị thông báo
        $('#confirmLockModal').modal('hide');
        alert(body.message);
        loadDoctors();
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
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
        const res = await fetch(`/api/admin/users/unlock/${doctorId}`, { method: "PUT" });
        const body = await handleResponse(res);

        //Hiển thị thông báo
        $('#confirmUnlockModal').modal('hide');
        alert(body.message);
        loadDoctors();
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
});

//Load ảnh
document.getElementById("create_avatar").addEventListener("change", function (e) {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById("previewCreateAvatar").src = e.target.result;
    };
    reader.readAsDataURL(file);
});

//====HÀM LOAD DROPDOWNS====//
async function loadDropdownsCreate() {
    try {
        // Reset options
        create_specialty.innerHTML = `<option value="">-- Chọn chuyên khoa --</option>`;
        create_room.innerHTML = `<option value="">-- Chọn phòng khám --</option>`;

        // Gọi API chuyên khoa
        const resSpecialty = await fetch("/api/admin/specialties");
        const bodySpecialty = await handleResponse(resSpecialty);

        //Thêm dropdown chuyên khoa
        const specialties = bodySpecialty.data;
        specialties.forEach(specialty => {
            const opt = document.createElement("option");
            opt.value = specialty.id;
            opt.textContent = specialty.name;
            create_specialty.appendChild(opt);
        });

        // Gọi API phòng
        const resRoom = await fetch("/api/admin/rooms");
        const bodyRoom = await handleResponse(resRoom);

        //Thêm dropdown phòng khám
        const rooms = bodyRoom.data;
        rooms.forEach(room => {
            const opt = document.createElement("option");
            opt.value = room.id;
            opt.textContent = `${room.name} (${room.currentDoctorCount}/2)`;
            create_room.appendChild(opt);
        });
    } catch (error) {
        console.error("Lỗi loadDropdowns:", error);
        alert("Không thể tải dữ liệu chuyên khoa hoặc phòng khám!");
    }
}

async function loadDropdownsUpdate() {
    try {
        // Reset options
        update_specialty.innerHTML = `<option value="">-- Chọn chuyên khoa --</option>`;
        update_room.innerHTML = `<option value="">-- Chọn phòng khám --</option>`;

        // Gọi API chuyên khoa
        const resSpecialty = await fetch("/api/admin/specialties");
        const bodySpecialty = await handleResponse(resSpecialty);

        //Thêm dropdown chuyên khoa
        const specialties = bodySpecialty.data;
        specialties.forEach(specialty => {
            const opt = document.createElement("option");
            opt.value = specialty.id;
            opt.textContent = specialty.name;
            update_specialty.appendChild(opt);
        });

        // Gọi API phòng
        const resRoom = await fetch("/api/admin/rooms");
        const bodyRoom = await handleResponse(resRoom);

        //Thêm dropdown phòng khám
        const rooms = bodyRoom.data;
        rooms.forEach(room => {
            const opt = document.createElement("option");
            opt.value = room.id;
            opt.textContent = `${room.name} (${room.currentDoctorCount}/2)`;
            update_room.appendChild(opt);
        });
    } catch (error) {
        console.error("Lỗi loadDropdowns:", error);
        alert("Không thể tải dữ liệu chuyên khoa hoặc phòng khám!");
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