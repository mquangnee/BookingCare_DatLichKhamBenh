document.addEventListener("DOMContentLoaded", async function () {
    try {
        const res = await fetch('/data/dataCurrentUser.json');
        if (!res.ok) throw new Error("Không thể tải dữ liệu JSON");

        const json = await res.json();
        const patient = json.patient;

        console.log("Dữ liệu bệnh nhân:", patient);

        document.getElementById("hoTen").value = patient.FullName || "";
        document.getElementById("ngaySinh").value = patient.BirthOfDate || "";
        document.getElementById("diaChi").value = patient.Address || "";
        document.getElementById("soDienThoai").value = patient.PhoneNumber || "";
        document.getElementById("email").value = patient.Email || "";

        if (patient.Gender === "Nam") {
            document.getElementById("nam").checked = true;
        } else {
            document.getElementById("nu").checked = true;
        }

    } catch (err) {
        alert("❌ Không thể kết nối cơ sở dữ liệu, vui lòng thử lại!");
        console.error("Lỗi load JSON:", err);
    }
});

document.getElementById("updateinfor").addEventListener("submit", async (e) => {
    e.preventDefault();
    const PatientUpdate = {
        "FullName": document.getElementById("hoTen").value,
        "BirthOfDate": document.getElementById("ngaySinh").value,
        "Address": document.getElementById("diaChi").value,
        "PhoneNumber": document.getElementById("soDienThoai").value,
        "Email": document.getElementById("email").value,
        "Gender": document.getElementById("nam").checked ? "Nam" : "Nữ"
    };

    console.log(" Dữ liệu gửi cập nhật:", PatientUpdate);

    try {
        const res = await fetch('/Patients/Update', { 
            method: "PATCH",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(PatientUpdate)
        });

        if (!res.ok) {
            alert(" Cập nhật dữ liệu không thành công!");
            console.error("Lỗi cập nhật:", await res.text());
        } else {
            alert(" Cập nhật dữ liệu thành công!");
        }
    } catch (error) {
        alert("⚠️ Lỗi khi gửi yêu cầu cập nhật!");
        console.error("Lỗi fetch PATCH:", error);
    }
});
document.getElementById("buttonReset").addEventListener("onClick", (e) => {
    e.preventDefault();
    document.getElementById("hoTen").value = "";
    document.getElementById("ngaySinh").value =  "";
    document.getElementById("diaChi").value = "";
    document.getElementById("soDienThoai").value =  "";
    document.getElementById("email").value =  "";
})
