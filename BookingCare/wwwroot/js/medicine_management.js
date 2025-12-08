// Phân trang
let currentPage = 1; //Trang hiện tại
const pageSize = 10; //Mỗi trang 10 dòng
let totalPages = 1; //Tổng số trang (sẽ tính lại sau khi gọi API)
let medicineKeyword = "";

// Lấy các thành phần trong View
// 1. Danh sách thuốc và phân trang
const tableBody = document.getElementById("medicineTableBody");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");
// 2. Thêm thuốc
const formAddMedicine = document.getElementById("formAddMedicine");
const btnAddMedicine = document.getElementById("btnAddMedicine");
const create_name = document.getElementById("create_name");
const create_unit = document.getElementById("create_unit");
const create_function = document.getElementById("create_function");
// 3. Cập nhật thuốc
const formUpdateMedicine = document.getElementById("formUpdateMedicine");
const update_name = document.getElementById("update_name");
const update_unit = document.getElementById("update_unit");
const update_func = document.getElementById("update_function");
const update_medicineId = document.getElementById("update_medicineId");
// 4. Khóa/Mở khóa thuốc
const lockId = document.getElementById("lockMedicineId");
const unlockId = document.getElementById("unlockMedicineId");
const btnLock = document.getElementById("confirmLockBtn");
const btnUnlock = document.getElementById("confirmUnlockBtn");

//====DANH SÁCH THUỐC====//
//Gọi API và render dữ liệu
async function loadMedicines(page = 1, keyword = medicineKeyword) {
    try {
        currentPage = page;
        medicineKeyword = keyword;

        const res = await fetch(`/api/admin/medicines?page=${page}&pageSize=${pageSize}&search=${medicineKeyword}`);
        const body = await handleResponse(res);

        // Cập nhật lại tổng số trang
        const data = body.data;
        totalPages = Math.ceil(data.totalMedicines / pageSize);
        renderTable(data.listMedicines);
        updatePagination();

    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
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

        //Hiển thị trạng thái thuốc
        const statusHtml =
            d.status === "Đang sử dụng"
                ? `<span class="text-white badge bg-success">Đang sử dụng</span>`
                : `<span class="text-white badge bg-secondary">Dừng sử dụng</span>`;

        //Dropdown action 
        const actionHtml = `
        <div class="btn-group">
            <button type="button" class="btn btn-sm btn-secondary dropdown-toggle" data-toggle="dropdown">
                <i class="fa-solid fa-bars"></i>
            </button>
            <div class="dropdown-menu">
                ${d.status === "Đang sử dụng"
                ? `<button class="dropdown-item stop-medicine" data-id="${d.id}">Dừng sử dụng</button>`
                : `<button class="dropdown-item activate-medicine" data-id="${d.id}">Kích hoạt lại</button>`}
                <button class="dropdown-item edit-medicine" data-id="${d.id}" data-toggle="modal" data-target="#updateMedicineModal">
                    Chỉnh sửa
                </button>
            </div>
        </div>`;

        return `
        <tr>
            <td>${d.id}</td>
            <td>${d.name}</td>
            <td>${d.unit}</td>
            <td>${d.function}</td>
            <td>${createdAt}</td>
            <td>${updatedAt}</td>
            <td>${statusHtml}</td>
            <td>${actionHtml}</td>
        </tr>`;
    }).join("");
}

//Lần đầu load
loadMedicines(currentPage);

//====PHÂN TRANG THUỐC====
//Cập nhật hiển thị phân trang
function updatePagination() {
    pageInfo.textContent = `Trang ${currentPage} / ${totalPages}`;
    prevBtn.disabled = currentPage <= 1;
    nextBtn.disabled = currentPage >= totalPages;
}
prevBtn.addEventListener("click", () => {
    if (currentPage > 1) {
        loadMedicines(currentPage - 1);
    }
});
nextBtn.addEventListener("click", () => {
    if (currentPage < totalPages) {
        loadMedicines(currentPage + 1);
    }
});

//Tìm kiếm thuốc
function searchMedicines(keyword) {
    loadMedicines(1, keyword);
}

//====THÊM THUỐC====//
btnAddMedicine.addEventListener("click", async function () {
    //Lấy dữ liệu từ modal
    const addMedicine = {
        name: create_name.value.trim(),
        unit: create_unit.value,
        function: create_function.value.trim()
    };

    try {
        //Gửi yêu cầu thêm thuốc đến server
        const res = await fetch(`/api/admin/medicines`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(addMedicine)
        });
        const body = await handleResponse(res);

        if (body.success) {
            alert(body.message);
            formAddMedicine.reset(); //Làm rỗng modal
            $('#addMedicineModal').modal('hide');
            loadMedicines();
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
});

//====CHỈNH SỬA THÔNG TIN THUỐC====//
//1. Hiển thị modal chỉnh sửa thông tin thuốc
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".edit-medicine"); //Tìm đúng nút "Chỉnh sửa"
    if (!btn) return;

    const medicineId = btn.dataset.id; //Lấy giá trị data-id
    if (!medicineId) {
        alert("Không thể lấy Id thuốc!");
        return;
    }

    try {
        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/api/admin/medicines/${medicineId}`);
        const body = await handleResponse(res);

        //Lưu user ID
        const data = body.data;
        update_medicineId.value = data.medicineId;

        //Hiển thị thông tin
        update_name.value = data.inforMedicine.name;
        update_unit.value = data.inforMedicine.unit;
        update_func.value = data.inforMedicine.function;

        //Hiển thị modal
        $("#updateMedicineModal").modal("show");
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
});

//2. Cập nhật thông tin
document.addEventListener("click", async function (e) {
    const btn = e.target.closest("#btnUpdateMedicine"); //Tìm đúng nút "Lưu thông tin"
    if (!btn) return;

    const medicineId = document.getElementById("update_medicineId").value; //Lấy giá trị medicineId
    if (!medicineId) {
        alert("Không thể lấy Id thuốc!");
        return;
    }

    try {
        //Lấy dữ liệu từ modal
        const addMedicine = {
            name: update_name.value.trim(),
            unit: update_unit.value,
            function: update_func.value.trim()
        };

        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/api/admin/medicines/${medicineId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(addMedicine)
        });
        const body = await handleResponse(res);

        if (body.success) {
            alert(body.message);
            formUpdateMedicine.reset(); //Làm rỗng modal
            $('#updateMedicineModal').modal('hide');
            loadMedicines(currentPage);
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
});

//====KHÓA/MỞ KHÓA THUỐC====//
/* Khóa thuốc */
//1. Hiển thị modal xác nhận khóa
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".stop-medicine"); //Tìm đúng nút "Khóa"
    if (!btn) return;

    //Lưu Id thuốc
    const medicineId = btn.dataset.id;
    lockId.value = medicineId;

    //Hiển thị modal
    $('#confirmLockModal').modal('show');
});
//2. Khóa thuốc
btnLock.addEventListener("click", async function (e) {
    //Lấy Id thuốc
    const medicineId = lockId.value;
    if (!medicineId) {
        alert("Không thể lấy Id thuốc!");
        return;
    }

    try {
        //Gửi yêu cầu khóa thuốc về server 
        const res = await fetch(`/api/admin/medicines/lock/${medicineId}`, { method: "PUT" });
        const body = await handleResponse(res);

        //Hiển thị thông báo
        $('#confirmLockModal').modal('hide');
        alert(body.message);
        loadMedicines();
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
    }
});

/* Mở khóa thuốc */
//1. Hiển thị modal xác nhận mở khóa
document.addEventListener("click", async function (e) {
    const btn = e.target.closest(".activate-medicine"); //Tìm đúng nút "Mở khóa"
    if (!btn) return;

    //Lưu Id thuốc
    const medicineId = btn.dataset.id;
    unlockId.value = medicineId;

    //Hiển thị modal
    $('#confirmUnlockModal').modal('show');
});
//2. Mở khóa thuốc
btnUnlock.addEventListener("click", async function (e) {
    //Lấy Id thuốc
    const medicineId = unlockId.value;
    if (!medicineId) {
        alert("Không thể lấy Id thuốc!");
        return;
    }

    try {
        //Gửi yêu cầu mở khóa thuốc về server 
        const res = await fetch(`/api/admin/medicines/unlock/${medicineId}`, { method: "PUT" });
        const body = await handleResponse(res);

        //Hiển thị thông báo
        $('#confirmUnlockModal').modal('hide');
        alert(body.message);
        loadMedicines();
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error)
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