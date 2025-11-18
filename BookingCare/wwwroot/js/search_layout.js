const searchInput = document.getElementById("searchInput");
let timer;

if (searchInput) {
    searchInput.addEventListener("keyup", function () {
        clearTimeout(timer);
        const keyword = this.value.trim();
        const pageType = document.getElementById("pageType")?.dataset.page;

        if (!pageType) return;

        timer = setTimeout(() => {
            switch (pageType) {
                case "doctor":
                    searchDoctors(keyword);
                    break;
                case "patient":
                    searchPatients(keyword);
                    break;
                case "medicine":
                    searchMedicines(keyword);
                    break;
            }
        }, 300);
    });
}
