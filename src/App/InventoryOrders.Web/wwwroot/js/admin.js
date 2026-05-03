
$(function () {
    // Sidebar toggle
    $("#toggleSidebar").on("click", function () {
        if ($(window).width() <= 768) {
            $("#sidebar").toggleClass("active");
            $("#overlay").toggleClass("show");
        } else {
            $("#sidebar").toggleClass("active");
            $("#mainContent").toggleClass("full");
        }
    });

    // Overlay click (mobile)
    $("#overlay").on("click", function () {
        $("#sidebar").removeClass("active");
        $("#overlay").removeClass("show");
    });

    // Dark mode toggle
    $("#toggleTheme").on("click", function () {
        let isDark = $("body").hasClass("dashboard-bg-dark");
        $("body").toggleClass("dashboard-bg dashboard-bg-dark");
        $("#topbar").toggleClass("dark");
        $("#themeIcon")
            .toggleClass("ri-moon-line", isDark)
            .toggleClass("ri-sun-line", !isDark);
        localStorage.setItem("theme", isDark ? "light" : "dark");
    });

    // Load saved theme
    let savedTheme = localStorage.getItem("theme") || "light";
    if (savedTheme === "dark") {
        $("body").removeClass("dashboard-bg").addClass("dashboard-bg-dark");
        $("#topbar").addClass("dark");
        $("#themeIcon").removeClass("ri-moon-line").addClass("ri-sun-line");
    }

    // Resize fix: hide sidebar on mobile resize
    $(window).on("resize", function () {
        if ($(window).width() > 768) {
            $("#sidebar").removeClass("active");
            $("#overlay").removeClass("show");
        }
    });
});
