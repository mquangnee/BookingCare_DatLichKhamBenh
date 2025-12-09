async function loadData() {
    const id = document.getElementById("appointmentId")?.value;

    if (!id) {
        alert("Không tìm thấy ID lịch khám!");
        return;
    }

    try {
        const res = await fetch(`/Doctors/api/AppoimentDetail/detail/${id}`, {
            credentials: "include"
        });

        if (!res.ok) {
            throw new Error("Không tìm thấy dữ liệu!");
        }

        const data = await res.json();
        console.log("DATA API:", data);

        if (!data.success) {
            alert("Không có dữ liệu!");
            return;
        }

        const p = data.data;

        document.getElementById("tenBenhNhan").textContent = p.patientName;
        document.getElementById("ngaySinh").textContent = p.dob;
        document.getElementById("gender").textContent = p.gender;
        document.getElementById("appointmentDate").textContent = p.date;
        document.getElementById("appointmentTime").textContent = p.time;
        document.getElementById("phoneNumber").textContent = p.phone;
        document.getElementById("address").textContent = p.address;
        document.getElementById("reason").value = p.reason;

    } catch (err) {
        alert("Không thể kết nối với server!");
        console.error("Lỗi load chi tiết:", err);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    loadData();

    // ✅ Quay lại danh sách
    const btnBack = document.getElementById("quayLai");
    if (btnBack) {
        btnBack.addEventListener("click", () => {
            window.location.href = "/Doctor/Doctors/Index";
        });
    }
});
document.getElementById("traKqKhamBenh").addEventListener("click", function () {
    const url = this.getAttribute("data-url");

    if (!url) {
        alert("Không tìm thấy URL điều hướng!");
        return;
    }

    window.location.href = url;
});
