// Caminhos das imagens
const lightLogo = '/img/LogoAFEE.png';
const darkLogo = '/img/LogoAFEE_darkmode.png';

// Função para alternar o tema
function toggleTheme() {
    const body = document.body;
    const modeText = document.querySelector(".mode-text");
    const themeIcon = document.getElementById('logo');

    // Verificar se os elementos existem antes de continuar
    if (!modeText || !themeIcon) return;

    // Alternar entre modo claro e escuro
    const isDarkMode = body.classList.contains("dark");
    if (isDarkMode) {
        body.classList.remove("dark");
        body.classList.add("light");
        modeText.innerText = "Modo Escuro";  // Mostrar a opção de Modo Escuro
        themeIcon.src = lightLogo;  // Logo para Modo Claro
    } else {
        body.classList.remove("light");
        body.classList.add("dark");
        modeText.innerText = "Modo Claro";  // Mostrar a opção de Modo Claro
        themeIcon.src = darkLogo;  // Logo para Modo Escuro
    }

    // Salvar preferências no localStorage
    const currentTheme = body.classList.contains("dark") ? "dark" : "light";
    localStorage.setItem("theme", currentTheme);
}

// Função para carregar o tema salvo
function loadTheme() {
    const body = document.body;
    const modeText = document.querySelector(".mode-text");
    const themeIcon = document.getElementById('logo');

    // Verificar se os elementos existem antes de continuar
    if (!modeText || !themeIcon) return;

    // Carregar o tema salvo no localStorage
    const savedTheme = localStorage.getItem("theme");

    // Definir o tema com base no que está salvo no localStorage
    if (savedTheme === "dark") {
        body.classList.add("dark");
        body.classList.remove("light");  // Garantir que a classe light seja removida
        themeIcon.src = darkLogo;
        modeText.innerText = "Modo Claro";
    } else {
        body.classList.add("light");
        body.classList.remove("dark");  // Garantir que a classe dark seja removida
        themeIcon.src = lightLogo;
        modeText.innerText = "Modo Escuro";
    }
}

// Inicializar o comportamento da página
function initializePage() {
    const modeSwitch = document.querySelector(".toggle-switch");
    const toggle = document.querySelector(".toggle");
    const sidebar = document.querySelector(".menu-vertical");

    // Verificar se os elementos existem antes de continuar
    if (modeSwitch) {
        modeSwitch.addEventListener("click", toggleTheme);
    }

    if (toggle && sidebar) {
        toggle.addEventListener("click", () => {
            sidebar.classList.toggle("close");
        });
    }

    // Carregar o tema salvo
    loadTheme();


    // Animação de elementos com a classe .fade-in
    const elements = document.querySelectorAll(".fade-in");
    elements.forEach(element => {
        element.classList.add("visible");
    });
}

// Garantir que tudo esteja carregado
window.onload = initializePage;
