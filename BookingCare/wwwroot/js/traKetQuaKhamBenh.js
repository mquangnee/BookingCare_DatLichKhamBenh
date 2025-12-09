let medicineData = [];
let addedMedicines = [];

const appointmentId = document.getElementById("appointmentId")?.value;

// ================== LOAD DANH SÁCH THUỐC TỪ DB ==================
async function loadMedicines() {
    try {
        const res = await fetch("/Doctor/api/ReturnResults/get-medicines", {
            credentials: "include"
        });
        const data = await res.json();
        medicineData = data;
    } catch (error) {
        console.error("Không thể tải dữ liệu thuốc:", error);
    }
}

// ================== TÌM THUỐC ==================
document.getElementById("searchMedicine").addEventListener("input", function () {
    const query = this.value.toLowerCase();
    const suggestionList = document.getElementById("suggestionList");
    suggestionList.innerHTML = "";

    if (query.length === 0) return;

    const matches = medicineData.filter(med =>
        med.name.toLowerCase().includes(query)
    );

    matches.forEach(med => {
        const li = document.createElement("li");
        li.textContent = `${med.name} (${med.unit})`;
        li.addEventListener("click", () => {
            const selected = document.getElementById("selectedMedicine");
            selected.value = med.name;
            selected.dataset.id = med.id; // ✅ ID thật từ DB
            suggestionList.innerHTML = "";
            document.getElementById("searchMedicine").value = "";
        });
        suggestionList.appendChild(li);
    });
});

// ================== THÊM THUỐC ==================
document.getElementById("addMedicineBtn").addEventListener("click", () => {
    const selectedInput = document.getElementById("selectedMedicine");
    const name = selectedInput.value.trim();
    const medId = selectedInput.dataset.id;
    const quantity = document.getElementById("dosage").value.trim();
    const usage = document.getElementById("usage").value.trim();

    if (!name || !quantity || !usage || !medId) {
        alert("Vui lòng nhập đầy đủ thông tin thuốc!");
        return;
    }

    const med = {
        MedicineId: parseInt(medId),
        Quantity: parseInt(quantity),
        Dosage: quantity,
        Instructions: usage
    };

    addedMedicines.push(med);
    renderTable();

    selectedInput.value = "";
    document.getElementById("dosage").value = "";
    document.getElementById("usage").value = "";
});

// ================== RENDER BẢNG ==================
function renderTable() {
    const tbody = document.querySelector("#medicineTable tbody");
    tbody.innerHTML = "";

    addedMedicines.forEach(med => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${med.MedicineId}</td>
            <td>${med.Quantity}</td>
            <td>${med.Dosage}</td>
            <td>${med.Instructions}</td>
        `;
        tbody.appendChild(tr);
    });
}

// ================== GỬI KẾT QUẢ KHÁM ==================
document.getElementById("submitBtn").addEventListener("click", async () => {
    const diagnosis = document.getElementById("diagnosis").value.trim();
    const instructions = document.getElementById("instructions").value.trim();

    if (!diagnosis) {
        alert("Vui lòng nhập chẩn đoán!");
        return;
    }

    if (!appointmentId) {
        alert("Không tìm thấy ID lịch khám!");
        return;
    }

    const result = {
        Diagnosis: diagnosis,
        Instructions: instructions,
        Medicines: addedMedicines
    };

    try {
        const res = await fetch(`/Doctor/api/ReturnResults/submit-result/${appointmentId}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            credentials: "include",
            body: JSON.stringify(result)
        });

        const responseData = await res.json();

        if (!res.ok || !responseData.success) {
            throw new Error("Lưu thất bại!");
        }

        alert("✅ Đã lưu kết quả khám bệnh thành công!");

        addedMedicines = [];
        renderTable();
        document.getElementById("diagnosis").value = "";
        document.getElementById("instructions").value = "";

    } catch (error) {
        console.error("Lỗi khi gửi kết quả khám:", error);
        alert("❌ Không thể gửi dữ liệu lên server!");
    }
});

document.addEventListener("DOMContentLoaded", loadMedicines);
