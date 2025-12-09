let appointmentData = [];


function parseVNDate(dateStr, timeStr) {
    try {
        const [day, month, year] = dateStr.split("/");
        return new Date(`${year}-${month}-${day}T${timeStr}`);
    } catch {
        return new Date(0);
    }
}


async function loadAppointments() {
    try {
        const res = await fetch('/Doctors/api/AppoimentsMain/list');

        if (!res.ok) {
            throw new Error("Không thể gọi API, mã lỗi: " + res.status);
        }

        const data = await res.json();

        if (!data.success) {
            alert("Không thể tải dữ liệu!");
            return;
        }

        appointmentData = data.appointments.sort((a, b) => {
            const dateA = parseVNDate(a.date, a.time);
            const dateB = parseVNDate(b.date, b.time);
            return dateB - dateA;
        });

        displayAppointments('All');

    } catch (err) {
        console.error('Lỗi khi loadAppointments():', err);
        alert('Không thể загруз dữ liệu từ server!');
    }
}


function displayAppointments(status) {
    const tbody = document.querySelector("#appointmentsTable tbody");

    if (!tbody) {
        console.error("❌ Không tìm thấy bảng #appointmentsTable");
        return;
    }

    tbody.innerHTML = '';

    let filtered = appointmentData;
    if (status !== 'All') {
        filtered = appointmentData.filter(a => a.status === status);
    }

    filtered.forEach((a, index) => {

        let statusColor = "black";
        if (a.status === "Pending" || a.status === "Chờ khám") statusColor = "green";
        if (a.status === "Confirmed" || a.status === "Đã xác nhận") statusColor = "blue";
        if (a.status === "Canceled" || a.status === "Đã hủy") statusColor = "red";

        const tr = document.createElement("tr");
        tr.innerHTML = `
            <td>${a.id}</td>
            <td>${a.date}</td>
            <td>${a.time}</td>
            <td>${a.patientName}</td>
            <td>${a.reason}</td>
            <td style="color:${statusColor}; font-weight:600">
                ${a.status}
            </td>
            <td>
                <a href="/Doctor/Doctors/Detail/${a.id}" 
                   style="color:#2563eb; font-weight:600; text-decoration:none">
                    Chi tiết
                </a>
            </td>
        `;
        tbody.appendChild(tr);
    });
}


function onStatusFilterChange(select) {
    const status = select.value;
    displayAppointments(status);
}

document.addEventListener("DOMContentLoaded", loadAppointments);
