const step1 = document.querySelector('.step1');
const step2 = document.querySelector('.step2');
const step3 = document.querySelector('.step3');
const stepTitle = document.querySelector('.step-title');

document.getElementById('btnNext').addEventListener('click', async () => {
        const email = document.getElementById('email').value.trim();
        const oldPassword = document.getElementById('oldPassword').value.trim();
        const newPassword = document.getElementById('newPassword').value.trim();
        const confirmPassword = document.getElementById('confirmPassword').value.trim();


        if (!email || !oldPassword || !newPassword || !confirmPassword) {
            alert("Vui lòng điền đầy đủ thông tin!");
            return;
        }
        if (newPassword !== confirmPassword) {
            alert("Mật khẩu xác nhận không khớp!");
            return;
        }

        try {
            const res = await fetch('/Account/RequestOtp', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, oldPassword, newPassword })
            });

            const result = await res.json();

            if (res.ok && result.success) {
                alert("✅ Mã OTP đã được gửi tới email của bạn!");
                step1.classList.remove('active');
                step2.classList.add('active');
                stepTitle.textContent = "BƯỚC 2: XÁC NHẬN OTP";
            } else {
                alert("❌ " + (result.message || "Không thể gửi OTP!"));
            }
        } catch (error) {
            console.error("Lỗi gửi OTP:", error);
            alert("Có lỗi khi gửi yêu cầu đến server!");
        }
    });


document.getElementById('btnConfirmOtp').addEventListener('click', async () => {
        const email = document.getElementById('email').value.trim();
        const otpCode = document.getElementById('otpCode').value.trim();
        const newPassword = document.getElementById('newPassword').value.trim();

        if (!otpCode) {
            alert("Vui lòng nhập mã OTP!");
            return;
        }

        try {
            const res = await fetch('/Account/ConfirmOtp', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, otpCode, newPassword })
            });

            const result = await res.json();

            if (res.ok && result.success) {
                step2.classList.remove('active');
                step3.classList.add('active');
                stepTitle.textContent = "BƯỚC 3: HOÀN TẤT";
            } else {
                alert("❌ " + (result.message || "Mã OTP không hợp lệ!"));
            }
        } catch (error) {
            console.error("Lỗi xác nhận OTP:", error);
            alert("Có lỗi khi xác nhận OTP!");
        }
    });

document.getElementById('btnBack').addEventListener('click', () => {
        step2.classList.remove('active');
        step1.classList.add('active');
        stepTitle.textContent = "BƯỚC 1: TÀI KHOẢN";
    });
