let medicineData = [];
let addedMedicines = [];


async function loadMedicines() {
    try {
        const res = await fetch("/data/medicines.json");
        const data = await res.json();
        medicineData = data.medicines;
    } catch (error) {
        console.error("Không thể tải dữ liệu thuốc:", error);
    }
}


document.getElementById("searchMedicine").addEventListener("input", function () {
    const query = this.value.toLowerCase();
    const suggestionList = document.getElementById("suggestionList");
    suggestionList.innerHTML = "";

    if (query.length === 0) return;

    const matches = medicineData.filter(med => med.Name.toLowerCase().includes(query));

    matches.forEach(med => {
        const li = document.createElement("li");
        li.textContent = `${med.Name} (${med.Unit}) - ${med.Function}`;
        li.addEventListener("click", () => {
            document.getElementById("selectedMedicine").value = med.Name;
            suggestionList.innerHTML = "";
            document.getElementById("searchMedicine").value = "";
        });
        suggestionList.appendChild(li);
    });
});


document.getElementById("addMedicineBtn").addEventListener("click", () => {
    const name = document.getElementById("selectedMedicine").value.trim();
    const dosage = document.getElementById("dosage").value.trim();
    const usage = document.getElementById("usage").value.trim();

    if (!name || !dosage || !usage) {
        alert("Vui lòng nhập đầy đủ thông tin thuốc!");
        return;
    }

    const med = { Name: name, Dosage: dosage, Usage: usage };
    addedMedicines.push(med);
    renderTable();

    document.getElementById("selectedMedicine").value = "";
    document.getElementById("dosage").value = "";
    document.getElementById("usage").value = "";
});


function renderTable() {
    const tbody = document.querySelector("#medicineTable tbody");
    tbody.innerHTML = "";

    addedMedicines.forEach((med, index) => {
        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${med.Name}</td>
            <td>${med.Dosage}</td>
            <td>${med.Usage}</td>
            <td><button class="deleteBtn" data-index="${index}">Xóa</button></td>
        `;
        tbody.appendChild(tr);
    });

    document.querySelectorAll(".deleteBtn").forEach(btn => {
        btn.addEventListener("click", function () {
            const index = this.getAttribute("data-index");
            addedMedicines.splice(index, 1);
            renderTable();
        });
    });
}


document.getElementById("submitBtn").addEventListener("click", async () => {
    const diagnosis = document.getElementById("diagnosis").value.trim();
    const instructions = document.getElementById("instructions").value.trim();

    if (!diagnosis) {
        alert("Vui lòng nhập chẩn đoán!");
        return;
    }

    const result = {
        Diagnosis: diagnosis,
        Instructions: instructions,
        Medicines: addedMedicines
    };

    try {
        const res = await fetch("/api/patient/submit-result", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(result)
        });

        if (!res.ok) {
            throw new Error(`Server trả về lỗi: ${res.status}`);
        }

        const responseData = await res.json();
        alert("Đã lưu kết quả khám bệnh thành công!");
        console.log("Phản hồi từ server:", responseData);

        addedMedicines = [];
        renderTable();
        document.getElementById("diagnosis").value = "";
        document.getElementById("instructions").value = "";
    } catch (error) {
        console.error("Lỗi khi gửi kết quả khám:", error);
        alert("Không thể gửi dữ liệu lên server!");
    }
});


document.addEventListener("DOMContentLoaded", loadMedicines);
