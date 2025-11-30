//Lấy các phần tử từ DOM
const formUpdateInfo = document.getElementById("updateInfor");
const fullName = document.getElementById("fullName");
const dateOfBirth = document.getElementById("dateOfBirth");
const address = document.getElementById("address");
const phoneNumber = document.getElementById("phoneNumber");
const medicalHistory = document.getElementById("medicalHistory");
const maleRadio = document.getElementById("male");
const femaleRadio = document.getElementById("female");

//Load thông tin bệnh nhân vào form
document.addEventListener("DOMContentLoaded", async function () {
    try {
        //Gọi API để lấy thông tin bệnh nhân
        const res = await fetch('/Patient/api/AccountManagementApi/accountManagement');
        if (!res.ok) {
            alert("Lỗi khi tải dữ liệu bệnh nhân!");
            return;
        }

        //Xử lý dữ liệu trả về
        const data = await res.json();

        //Hiển thị thông tin lên form
        fullName.value = data.fullName;
        dateOfBirth.value = data.dateOfBirth;
        address.value = data.address;
        phoneNumber.value = data.phoneNumber;
        medicalHistory.value = data.medicalHistory ? data.medicalHistory : "Chưa có tiền sử bệnh án."; 
        if (data.gender == "Nam") {
            maleRadio.checked = true;
        } else {
            femaleRadio.checked = true;
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.");
    }
});

//Cập nhật thông tin bệnh nhân
formUpdateInfo.addEventListener("submit", async function (e) {
    e.preventDefault();

    //Tạo đối tượng chứa dữ liệu cập nhật
    var patientUpdate = {
        fullName: fullName.value,
        gender: maleRadio.checked ? "Nam" : "Nữ",
        dateOfBirth: dateOfBirth.value,
        address: address.value,
        phoneNumber: phoneNumber.value,
        medicalHistory: medicalHistory.value
    };
    try {
        //Gửi yêu cầu về server để cập nhật thông tin bệnh nhân
        const res = await fetch('/Patient/api/AccountManagementApi/updateInfor', { 
            method: "PUT",
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(patientUpdate)
        });

        //Xử lý phản hồi từ server và hiển thị thông báo
        const result = await res.json();
        alert(result.message);
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Lỗi kết nối với máy chủ! Vui lòng thử lại sau.");
    }
});
//document.getElementById("buttonReset").addEventListener("onClick", (e) => {
//    e.preventDefault();
//    document.getElementById("hoTen").value = "";
//    document.getElementById("ngaySinh").value =  "";
//    document.getElementById("diaChi").value = "";
//    document.getElementById("soDienThoai").value =  "";
//    document.getElementById("email").value =  "";
//})
