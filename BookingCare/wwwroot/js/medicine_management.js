let currentPage = 1; //Trang hiện tại
const pageSize = 10; //Mỗi trang 10 dòng
let totalPages = 1; //Tổng số trang (sẽ tính lại sau khi gọi API)
let medicineKeyword = "";

const tableBody = document.getElementById("medicineTableBody");
const prevBtn = document.getElementById("prevPage");
const nextBtn = document.getElementById("nextPage");
const pageInfo = document.getElementById("pageInfo");

//====DANH SÁCH THUỐC====//
//Gọi API và render dữ liệu
async function loadMedicines(page = 1, keyword = medicineKeyword) {
    try {
        currentPage = page;
        medicineKeyword = keyword;

        const res = await fetch(`/Admin/api/MedicineApi/medicines?page=${page}&pageSize=${pageSize}&search=${medicineKeyword}`);
        const result = await res.json();

        // Cập nhật lại tổng số trang
        totalPages = Math.ceil(result.totalMedicines / pageSize);
        renderTable(result.data);
        updatePagination();

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
const btnAddMedicine = document.getElementById("btnAddMedicine");
btnAddMedicine.addEventListener("click", async function () {
    //Lấy dữ liệu từ modal
    const body = {
        name: document.getElementById("create_name").value.trim(),
        unit: document.getElementById("create_unit").value,
        function: document.getElementById("create_function").value.trim()
    };

    try {
        //Gửi yêu cầu thêm thuốc đến server
        const res = await fetch(`/Admin/api/MedicineApi/create`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        //Xử lý phản hồi từ server
        const result = await res.json();

        if (result.success) {
            alert(result.message);
            document.getElementById("formAddMedicine").reset(); //Làm rỗng modal
            $('#addMedicineModal').modal('hide');
            loadMedicines();
        } else {
            alert(result.message);
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
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
        const res = await fetch(`/Admin/api/MedicineApi/updateInfoMedicine?id=${medicineId}`);

        //Thông tin chi tiết
        const data = await res.json();

        //Kiểm tra dữ liệu
        if (!res.ok) {
            alert("Không thể lấy thông tin thuốc!");
            return;
        }

        //Lấy các thành phần trong modal
        const name = document.getElementById("update_name");
        const unit = document.getElementById("update_unit");
        const func = document.getElementById("update_function");

        //Lưu user ID
        document.getElementById("update_medicineId").value = data.medicineId;

        //Hiển thị thông tin
        name.value = data.result.name;
        unit.value = data.result.unit;
        func.value = data.result.function;

        //Hiển thị modal
        $("#updateMedicineModal").modal("show");

    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
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
        const body = {
            name: document.getElementById("update_name").value.trim(),
            unit: document.getElementById("update_unit").value,
            function: document.getElementById("update_function").value.trim()
        };

        //Gửi yêu cầu lấy thông tin chi tiết về server 
        const res = await fetch(`/Admin/api/MedicineApi/update/${medicineId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        //Xử lý phản hồi từ server
        const result = await res.json();

        if (result.success) {
            alert(result.message);
            document.getElementById("formUpdateMedicine").reset(); //Làm rỗng modal
            $('#updateMedicineModal').modal('hide');
            loadMedicines(currentPage);
        } else {
            alert(result.message);
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});

//====KHÓA/MỞ KHÓA THUỐC====//
const lockId = document.getElementById("lockMedicineId");
const unlockId = document.getElementById("unlockMedicineId");
const btnLock = document.getElementById("confirmLockBtn");
const btnUnlock = document.getElementById("confirmUnlockBtn");

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
        const res = await fetch(`/Admin/api/MedicineApi/lock/${medicineId}`, { method: "PUT" });

        //Thông tin chi tiết
        const data = await res.json();

        //Hiển thị thông báo
        $('#confirmLockModal').modal('hide');
        alert(data.message);
        loadMedicines();
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
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
        const res = await fetch(`/Admin/api/MedicineApi/unlock/${medicineId}`, { method: "PUT" });

        //Thông tin chi tiết
        const data = await res.json();

        //Hiển thị thông báo
        $('#confirmUnlockModal').modal('hide');
        alert(data.message);
        loadMedicines();
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.")
    }
});