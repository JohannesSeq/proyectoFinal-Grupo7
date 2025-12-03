const mode2 = document.getElementById("mode2");
const mode4 = document.getElementById("mode4");

function switchToMode2() {
    mode2.style.display = "block";
    mode4.style.display = "none";

    mode2.querySelectorAll("input").forEach(i => {
        i.required = true;
        i.disabled = false;
    });

    mode4.querySelectorAll("input").forEach(i => {
        i.required = false;
        i.disabled = true;
    });
}

function switchToMode4() {
    mode2.style.display = "none";
    mode4.style.display = "block";

    mode2.querySelectorAll("input").forEach(i => {
        i.required = false;
        i.disabled = true;
    });

    mode4.querySelectorAll("input").forEach(i => {
        i.required = true;
        i.disabled = false;
    });
}

document.querySelectorAll("input[name='mode']").forEach(r => {
    r.addEventListener("change", e => {
        if (e.target.value == "1") switchToMode2();
        else switchToMode4();
    });
});

// Aplicar configuración inicial:
switchToMode2();