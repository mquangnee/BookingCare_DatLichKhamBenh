let chuyenKhoaData = [];
let bacSiData = [];
let roomData = [];
let bookingData = [];

fetch('/data/dataBooking.json')
    .then(res => res.json())
    .then(data => {
        chuyenKhoaData = data.specialties.map(khoa => ({
            id: khoa.id,
            ten: khoa.name
        }));

        bacSiData = data.doctors.map(bs => ({
            id: bs.id,
            ten: bs.name, 
            chuyenKhoaId: bs.specialtyId,
            roomId: bs.roomId
        }));

        roomData = data.rooms;
        bookingData = data.appointments.map(a => ({
            doctorId: a.doctorId, 
            roomId: a.roomId,   
            date: a.Appointment_date,
            time: a.Appointment_time
        }));
        loadChuyenKhoa();
    })
    .catch(err => console.error("Lỗi load JSON:", err));

function loadChuyenKhoa() {
    const select = document.getElementById("chuyenKhoa");
    chuyenKhoaData.forEach(khoa => {
        const opt = document.createElement("option");
        opt.value = khoa.id;
        opt.textContent = khoa.ten;
        select.appendChild(opt);
    });
}

document.getElementById("chuyenKhoa").addEventListener("change", (e) => {
    const khoaId = parseInt(e.target.value);
    const bsSelect = document.getElementById("bacSi");
    bsSelect.innerHTML = '<option value="">-- Chọn bác sĩ --</option>';

    if (!khoaId) {
        bsSelect.disabled = true;
        return;
    }

    const dsBacSi = bacSiData.filter(bs => bs.chuyenKhoaId === khoaId);
    dsBacSi.forEach(bs => {
        const opt = document.createElement("option");
        opt.value = bs.id;
        opt.textContent = bs.ten;
        bsSelect.appendChild(opt);
    });

    bsSelect.disabled = dsBacSi.length === 0;
});

const khungGioContainer = document.getElementById("khungGioContainer");
const khungGio = document.getElementById("khungGio");

document.querySelectorAll('input[name="buoiKham"]').forEach(radio => {
    radio.addEventListener("change", e => {
        khungGioContainer.style.display = "block";
        khungGio.innerHTML = '<option value="">-- Chọn khung giờ --</option>';

        if (e.target.value === "Sáng") {
            taoKhungGio(7, 11);
        } else {
            taoKhungGio(13, 17);
        }
    });
});

function taoKhungGio(start, end) {
    for (let h = start; h < end; h += 0.5) {
        const startTime = formatTime(h);
        const endTime = formatTime(h + 0.5);
        const opt = document.createElement("option");
        opt.value = `${startTime} - ${endTime}`;
        opt.textContent = `${startTime} - ${endTime}`;
        khungGio.appendChild(opt);
    }
}
function formatTime(hour) {
    const h = Math.floor(hour);
    const m = hour % 1 === 0.5 ? "30" : "00";
    return `${h.toString().padStart(2, "0")}:${m}`;
}
document.getElementById("bookingForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const doctorId = parseInt(document.getElementById("bacSi").value);
    const date = document.getElementById("ngayKham").value;
    const khungGioValue = document.getElementById("khungGio").value;
    const reason = document.getElementById("lyDo").value;

 
    const room = roomData.find(r => r.id === bacSiData.find(bs => bs.id === doctorId)?.roomId);
    if (!room) {
        alert(" Không tìm thấy phòng cho bác sĩ này!");
        return;
    }

    const booked = bookingData.filter(b =>
        b.doctorId === doctorId &&
        b.roomId === room.id &&
        b.date === date &&
        b.time === khungGioValue
    );

    if (booked.length >= room.capacity) {
        alert("Số người chờ khám trong khung giờ này đã đầy, vui lòng chọn khung giờ khác");
        return;
    }

    const appointmentData = {
        PatientId:"",
        DoctorId: doctorId,
        AppointmentDate: date,
        AppointmentTime: khungGioValue,
        ReasonForVisit: reason,
        Status: "Pending",
        CreatedAt: new Date().toISOString()
    };

    try {
        const res = await fetch('/Patients/PatientBooking/BookAppointment', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(appointmentData)
        });

        const result = await res.json();

        if (res.ok) {
            alert(" Đặt lịch thành công!");
            console.log(" Dữ liệu gửi lên server:", result);
            e.target.reset();
            document.getElementById("bacSi").disabled = true;
            document.getElementById("khungGioContainer").style.display = "none";
        } else {
            alert(" Lỗi khi đặt lịch: " + (result.message || "Không xác định"));
        }
    } catch (err) {
        console.error(err);
        alert(" Lỗi kết nối server");
    }
});
