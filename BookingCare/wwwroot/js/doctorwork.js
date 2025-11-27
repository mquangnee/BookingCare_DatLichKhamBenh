let appointmentData = [];

async function loadAppointments() {
    try {
        const res = await fetch('/Doctors/api/AppoimentsMain/list');
        const data = await res.json();

        if (!data.success) {
            alert("Không thể tải dữ liệu!");
            return;
        }

        // Sắp xếp theo ngày + giờ (mới nhất lên trước)
        appointmentData = data.appointments.sort((a, b) => {
            const dateA = new Date(`${a.date} ${a.time}`);
            const dateB = new Date(`${b.date} ${b.time}`);
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

    // Lọc theo status
    let filtered = appointmentData;
    if (status !== 'All') {
        filtered = appointmentData.filter(a => a.status === sta
