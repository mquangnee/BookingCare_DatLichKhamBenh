let appointmentData = [];

async function loadAppointments() {
    try {
        const res = await fetch('/data/index.json'); 
        const data = await res.json();
        appointmentData = data.appointments.sort((a, b) => {
            const dateA = new Date(`${a.AppointmentDate} ${a.AppointmentTime}`);
            const dateB = new Date(`${b.AppointmentDate} ${b.AppointmentTime}`);
            return dateB - dateA;
        });
        displayAppointments('All');
    } catch (err) {
        console.error('Chi tiết lỗi khi loadAppointments():', err);
        alert(err.message);
    }
}

    function displayAppointments(status) {  
        const tbody = document.querySelector("#appointmentsTable tbody");
    tbody.innerHTML = '';

    let filtered = appointmentData;
    if (status !== 'All') {
        filtered = appointmentData.filter(a => a.Status === status);
        }

    if (filtered.length === 0) {
        document.getElementById('emptyMessage').classList.remove('d-none');
        } else {
        document.getElementById('emptyMessage').classList.add('d-none');
        }

        filtered.forEach(a => {
            const tr = document.createElement('tr');
    tr.innerHTML = `
 <tr>
    <td>${a.id}</td>
    <td>${a.AppointmentDate}</td>
    <td>${a.AppointmentTime}</td>
    <td>${a.PatientName}</td>
    <td>${a.ReasonForVisit}</td>
    <td class="status-${a.Status} fw-bold">${a.Status}</td>
    <td>
        <a class="nav-link text-red fw-bold" href="/Doctors/Doctors/Detail/${a.id}">Chi tiết</a>
    </td>
</tr>

    `;
    tbody.appendChild(tr);
        });
    }

    document.addEventListener('DOMContentLoaded', () => {
   
        loadAppointments();

    const statusFilter = document.getElementById('statusFilter');
        statusFilter.addEventListener('change', (e) => {
        displayAppointments(e.target.value);
        });
    });
