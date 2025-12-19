//Load dropdown chuyên khoa khi load trang xong
document.addEventListener("DOMContentLoaded", async function () {
    loadSpecialty();
});

async function loadSpecialty() {
    try {
        const specialtySelect = document.getElementById("specialty");

        // Reset options
        specialtySelect.innerHTML = `<option value="">-- Chọn chuyên khoa --</option>`;

        //Gọi Api chuyên khoa
        const res = await fetch(`/Patient/api/SpecialtyApi/getAll`);
        if (!res.ok) {
            alert("Không thể tải danh sách chuyên khoa!");
            return;
        }
        const specialtyList = await res.json();

        //Thêm dropdown chuyên khoa
        specialtyList.forEach(s => {
            const opt = document.createElement("option");
            opt.value = s.id;
            opt.textContent = s.name;
            specialtySelect.appendChild(opt);
        });
    } catch (error) {
        console.error("Lỗi loadDropdowns:", error);
        alert("Không thể tải dữ liệu chuyên khoa hoặc bác sĩ!");
    }
}

//Nếu chuyên khoa có bác sĩ thì bỏ disable bác sĩ
document.getElementById("specialty").addEventListener("change", async function () {
    try {
        const specialtyId = document.getElementById("specialty").value;
        const doctorSelect = document.getElementById("doctor");
        const bookingDate = document.getElementById("bookingDate");
        const timeShift = document.querySelectorAll("input[name='bookingShift']");
        const timeSlotContainer = document.getElementById("timeSlotContainer");

        //Reset options
        doctorSelect.innerHTML = '<option value="">-- Chọn bác sĩ --</option>';

        //Cập nhật hiển thị
        bookingDate.value = "";
        bookingDate.disabled = true;
        timeShift.forEach(r => {
            r.disabled = true;
            r.checked = false;
        });
        timeSlotContainer.style.display = "none";

        if (!specialtyId) {
            doctorSelect.disabled = true;
            return;
        }

        //Gọi Api bác sĩ
        const res = await fetch(`/Patient/Api/DoctorApi/getAll/${specialtyId}`);
        if (!res.ok) {
            alert("Không thể tải danh sách bác sĩ!");
            return;
        }
        const doctorList = await res.json();
        if (doctorList.length != 0) {
            doctorSelect.disabled = false;
        } else {
            doctorSelect.disabled = true;
            doctorSelect.innerHTML = '<option value="">Không có bác sĩ nào trong khoa này!</option>';   
        }

        //Thêm dropdown bác sĩ
        doctorList.forEach(d => {
            const otp = document.createElement("option");
            otp.value = d.id;
            otp.textContent = d.fullName;
            doctorSelect.appendChild(otp);
        });
    } catch (error) {
        console.error("Lỗi loadDropdowns:", error);
        alert("Không thể tải dữ liệu chuyên khoa hoặc bác sĩ!");
    }
});

//Bỏ disable booking date
document.getElementById("doctor").addEventListener("change", function () {
    try {
        const doctorId = document.getElementById("doctor").value;
        const bookingDate = document.getElementById("bookingDate");
        const timeShift = document.querySelectorAll("input[name='bookingShift']");
        const timeSlotContainer = document.getElementById("timeSlotContainer");

        //Cập nhật hiển thị
        bookingDate.value = "";
        timeShift.forEach(r => {
            r.disabled = true;
            r.checked = false;
        });
        timeSlotContainer.style.display = "none";

        if (!doctorId) {
            bookingDate.disabled = true;
            return;
        }
        bookingDate.disabled = false;
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.");
    }
});

//Bỏ disable buổi khám
document.getElementById("bookingDate").addEventListener("change", function () {
    try {
        const timeShift = document.querySelectorAll("input[name='bookingShift']");
        const timeSlotContainer = document.getElementById("timeSlotContainer");

        //Cập nhật hiển thị
        timeSlotContainer.style.display = "none";
        timeShift.forEach(r => {
            r.disabled = false;
            r.checked = false;        
        });
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.");
    }
});

//Hiển thị ca khám theo buổi
document.querySelectorAll("input[name='bookingShift']").forEach(radio => {
    radio.addEventListener("change", async function (e) {
        const timeSlotContainer = document.getElementById("timeSlotContainer");
        
        //Hiển thị ca khám khi đã chọn buổi khám
        timeSlotContainer.style.display = "block";

        if (e.target.value == "Sáng") {
            await getMorningTimeSlot();
        } else {
            await getEveningTimeSlot();
        }
    });
});

async function getMorningTimeSlot() {
    try {
        const doctorId = document.getElementById("doctor").value;
        const bookingDate = document.getElementById("bookingDate").value;
        const timeSlot = document.getElementById("timeSlot");

        //Reset options
        timeSlot.innerHTML = '<option value="">-- Chọn khung giờ --</option>';

        //Gọi Api ca khám sáng
        const res = await fetch(`/Patient/api/AppointmentApi/morningShift/${doctorId}/${bookingDate}`);
        if (!res.ok) {
            alert("Không thể tải danh sách ca!");
            return;
        }
        const morningTimeSlot = await res.json();

        //Thêm dropdown ca khám sáng
        morningTimeSlot.forEach(t => {
            const otp = document.createElement("option");
            otp.value = t.timeSlot;
            otp.textContent = t.timeSlot;
            if (!t.check) otp.disabled = true;
            timeSlot.appendChild(otp);
        });
        timeSlot.disabled = false;
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.");
    }
}

async function getEveningTimeSlot() {
    try {
        const doctorId = document.getElementById("doctor").value;
        const bookingDate = document.getElementById("bookingDate").value;
        const timeSlot = document.getElementById("timeSlot");

        //Reset options
        timeSlot.innerHTML = '<option value="">-- Chọn khung giờ --</option>';

        //Gọi Api ca khám sáng
        const res = await fetch(`/Patient/api/AppointmentApi/eveningShift/${doctorId}/${bookingDate}`);
        if (!res.ok) {
            alert("Không thể tải danh sách ca!");
            return;
        }
        const morningTimeSlot = await res.json();

        //Thêm dropdown ca khám sáng
        morningTimeSlot.forEach(t => {
            const otp = document.createElement("option");
            otp.value = t.timeSlot;
            otp.textContent = t.timeSlot;
            if (!t.check) otp.disabled = true;
            timeSlot.appendChild(otp);
        });
        timeSlot.disabled = false;
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.");
    }
}

//====ĐẶT LỊCH KHÁM BỆNH====//
document.getElementById("bookingForm").addEventListener("submit", async function (e) {
    try {
        e.preventDefault();
        //Lấy thông tin từ form
        const body = {
            specialtyId: document.getElementById("specialty").value,
            doctorId: document.getElementById("doctor").value,
            appointmentDate: document.getElementById("bookingDate").value,
            appointmentTime: document.getElementById("timeSlot").value,
            reasonForVisit: document.getElementById("medicalReason").value.trim()
        };

        //Gửi yêu cầu đặt lịch đến server
        const res = await fetch(`/Patient/api/PatientBookingApi/booking`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        //Xử lý phản hồi từ server
        const result = await res.json();

        //Hển thị thông báo
        alert(result.message);

        //Nếu thành công => về trang chủ
        if (res.ok) {
            setTimeout(() => window.location.href = "/Patient/Home/Index", 1000);
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.");
    }
});