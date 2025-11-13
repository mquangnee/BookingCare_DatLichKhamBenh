let appointmentsData = [];

async function loadAppointments() {
    try {
        const res = await fetch('/data/dataAppointment.json');
        const data = await res.json();

        appointmentsData = data.appointments.sort((a, b) => {
            const dateA = new Date(`${a.AppointmentDate} ${a.AppoinmentTime}`);
            const dateB = new Date(`${b.AppointmentDate} ${b.AppoinmentTime}`);
            return dateB - dateA;
        });

        displayAppointments('All');
    } catch (err) {
        alert('Không thể kết nối dữ liệu. Vui lòng thử lại.');
        console.error(err);
    }
}

function displayAppointments(status) {
    const tbody = document.querySelector('#appointmentsTable tbody');
    tbody.innerHTML = '';

    let filtered = appointmentsData;
    if (status !== 'All') {
        filtered = appointmentsData.filter(a => a.Status === status);
    }

    if (filtered.length === 0) {
        document.getElementById('emptyMessage').classList.remove('d-none');
    } else {
        document.getElementById('emptyMessage').classList.add('d-none');
    }

    filtered.forEach(a => {
        const tr = document.createElement('tr');
        let actionCell = '';

        if (a.Status === 'Pending') {
            actionCell = `<button class="btn btn-sm btn-danger cancel-btn" data-id="${a.id}">Hủy</button>`;
        }

        tr.innerHTML = `
            <td>${a.id}</td>
            <td>${a.AppointmentDate}</td>
            <td>${a.AppoinmentTime}</td>
            <td>${a.DoctorName}</td>
            <td>${a.RoomNumber}</td>
            <td>${a.ReasonForVisit}</td>
            <td class="status-${a.Status}">${a.Status}</td>
            <td>${actionCell}</td>
        `;

        tbody.appendChild(tr);
    });

    // Gắn sự kiện hủy lịch
    document.querySelectorAll('.cancel-btn').forEach(btn => {
        btn.addEventListener('click', e => {
            const id = e.target.getAttribute('data-id');
            cancelAppointment(id);
        });
    });
}

function cancelAppointment(id) {
    const appointment = appointmentsData.find(a => a.id == id);
    if (!appointment) return;

    if (confirm(`Bạn có chắc muốn hủy lịch hẹn ID ${id}?`)) {
        appointment.Status = 'Canceled';
        displayAppointments(document.getElementById('statusFilter').value);

        fetch('/Appointments/Cancel', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id: id })
        })
            .then(async res => {
                if (!res.ok) {
                    const text = await res.text();
                    throw new Error(text || 'Hủy lịch thất bại!');
                }
                return res.json();
            })
            .then(result => {
                alert(result.message || `Đã hủy lịch hẹn ID ${id} thành công!`);
                console.log("Kết quả server trả về:", result);
            })
            .catch(error => {
                alert("Lỗi khi hủy lịch: " + error.message);
                console.error("Chi tiết lỗi:", error);
            });
    }
}

document.getElementById('statusFilter').addEventListener('change', e => {
    displayAppointments(e.target.value);
});

loadAppointments();
    