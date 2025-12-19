let medList;
let addedMedicines = [];
// Lấy các thành phần trong view
const diagnosis = document.getElementById("diagnosis");
const instructions = document.getElementById("instructions");
const medSearchBox = document.getElementById("medSearchBox");
const suggestionList = document.getElementById("suggestionList");
const medName = document.getElementById("medName");
const dosage = document.getElementById("dosage");
const usage = document.getElementById("usage");
const addMedicineBtn = document.getElementById("addMedicineBtn");
const medicineTable = document.getElementById("medicineTable");
const sendMedReportBtn = document.getElementById("sendMedReportBtn");
const appointmentId = window.location.pathname.split("/").pop();

// Load dữ liệu thuốc trong hệ thống
async function loadMedicines() {
    try {
        const res = await fetch("/api/doctor/medicalreport");
        const body = await handleResponse(res);
        medList = body.data;
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error);
    }
}

// Load khi đã tải trang xong
document.addEventListener("DOMContentLoaded", loadMedicines);

// Tìm kiếm thuốc
medSearchBox.addEventListener("keyup", function () {
    const query = this.value.toLowerCase();
    suggestionList.innerHTML = "";
    if (!medList || medList.length === 0) return;
    if (query.length === 0) return;

    // Tìm thuốc theo tên
    const matches = medList.filter(med => med.name.toLowerCase().includes(query));
    matches.forEach(med => {
        const li = document.createElement("li");
        li.textContent = `${med.name} (${med.unit}) - ${med.function}`;
        li.addEventListener("click", () => {
            medName.value = med.name;
            dosage.disabled = false;
            usage.disabled = false;
            addMedicineBtn.disabled = false;
            suggestionList.innerHTML = "";
            medSearchBox.value = "";
        });
        suggestionList.appendChild(li);
    });
});

addMedicineBtn.addEventListener("click", () => {
    if (!medName.value.trim() || !dosage.value.trim() || !usage.value.trim()) {
        alert("Vui lòng nhập đầy đủ thông tin thuốc!");
        return;
    }
    const med =
    {
        Name: medName.value.trim(),
        Dosage: dosage.value.trim(),
        Usage: usage.value.trim()
    };
    addedMedicines.push(med);
    renderTable();
    medName.value = "";
    dosage.value = "";
    dosage.disabled = true;
    usage.value = "";
    usage.disabled = true;
    addMedicineBtn.disabled = true;
});

// Hiển thị danh sách thuốc đã thêm
function renderTable() {
    const tbody = document.querySelector("#medicineTable tbody");
    tbody.innerHTML = "";

    // Hiển thị danh sách thuốc trong đơn
    addedMedicines.forEach((med, index) => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${index + 1}</td>
            <td>${med.Name}</td>
            <td>${med.Dosage}</td>
            <td>${med.Usage}</td>
            <td><button class="deleteBtn" data-id="${index}">Xóa</button></td>
        `;
        tbody.appendChild(tr);
    });

    // Xóa thuốc khỏi danh sách
    document.querySelectorAll(".deleteBtn").forEach(btn => {
        btn.addEventListener("click", function () {
            const index = this.getAttribute("data-id");
            addedMedicines.splice(index, 1);
            renderTable();
        });
    });
}

// Gửi kết quả khám bệnh
sendMedReportBtn.addEventListener("click", async () => {
    const medReport = {
        AppointmentId: appointmentId,
        Diagnosis: diagnosis.value.trim(),
        Instructions: instructions.value.trim(),
        Medicines: addedMedicines
    };

    try {
        const res = await fetch("/api/doctor/medicalreport", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(medReport)
        });
        const body = await handleResponse(res);
        alert(body.message);
        setTimeout(() => window.location.href = "/Doctor/Home/Index", 1000);
    } catch (error) {
        console.error("Lỗi:", error);
        alert(error);
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