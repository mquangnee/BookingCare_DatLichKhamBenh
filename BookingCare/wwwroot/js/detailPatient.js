async function loadData() {
    try {
        const res = await fetch("/data/detailPatient.json");
        const data = await res.json();
        console.log(data);

        const p = data.detailPatient;
        document.getElementById("tenBenhNhan").textContent = p.FullName;
        document.getElementById("ngaySinh").textContent = p.BirthOfDate;
        document.getElementById("gender").textContent = p.Gender;
        document.getElementById("appointmentDate").textContent = p.AppointmentDate;
        document.getElementById("appointmentTime").textContent = p.AppointmentTime;
        document.getElementById("phoneNumber").textContent = p.PhoneNumber;
        document.getElementById("address").textContent = p.Address;
        document.querySelector("textarea").value = p.ReasonForVisit;
    } catch (err) {
        alert("Không thể kết nối với cơ sở dữ liệu");
        console.error(err);
    }
}
document.addEventListener('DOMContentLoaded', () => {
    loadData();

    document.getElementById("quayLai").addEventListener("click", () => {
        window.history.back();
    });
    const btn = document.getElementById("traKqKhamBenh");
    if (btn) {
        btn.addEventListener("click", function () {
            const url = btn.getAttribute("data-url");
            if (url) {
                window.location.href = url;
            } else {
                alert("Không tìm thấy URL điều hướng!");
            }
        });
    }

});

