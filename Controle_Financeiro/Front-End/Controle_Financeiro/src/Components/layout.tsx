import { Link, Outlet } from "react-router-dom";
import { useState } from "react";

import category from "../assets/category.png";
import Finance from "../assets/finance.png";
import transaction from "../assets/transaction.png";
import logo from "../assets/logo.png";

export default function Layout() {
  const [menuOpen, setMenuOpen] = useState(false);

  function logout() {
    localStorage.removeItem("token");
    window.location.href = "/login";
  }

  function closeMenu() {
    setMenuOpen(false);
  }

  return (
    <div className="min-h-screen w-full flex flex-col">
      {/* Navbar */}
      <header className="sticky top-0 z-50 bg-[#3F6464] shadow-md">
        <nav className="px-4 md:px-8 py-3">
          <div className="flex items-center justify-between">
            {/* Logo / Nome */}
            <Link to="/" onClick={closeMenu} className="flex items-center text-white">
              <img src={logo} alt="Controle Financeiro" className="w-10 h-10 mr-2" />

              <span className="font-semibold text-xl">Controle Financeiro</span>
            </Link>

            {/* Menu desktop */}
            <div className="hidden md:flex items-center gap-6">
              <Link to="/" className={menuLink}>
                <img src={Finance} alt="" className="w-6 h-6" />
                Resumo
              </Link>

              <Link to="/categoria" className={menuLink}>
                <img src={category} alt="" className="w-6 h-6" />
                Categorias
              </Link>

              <Link to="/transacao" className={menuLink}>
                <img src={transaction} alt="" className="w-6 h-6" />
                Transações
              </Link>

              <button onClick={logout} className="px-3 py-2 rounded text-white hover:bg-[#5A7067] transition-colors">
                Sair
              </button>
            </div>

            {/* Botão mobile */}
            <button onClick={() => setMenuOpen(!menuOpen)} className="md:hidden text-white text-2xl px-2" aria-label="Abrir menu">
              ☰
            </button>
          </div>

          {/* Menu mobile */}
          {menuOpen && (
            <div className="md:hidden mt-4 flex flex-col gap-2 border-t border-white/20 pt-3">
              <Link to="/" onClick={closeMenu} className={mobileMenuLink}>
                <img src={Finance} alt="" className="w-6 h-6" />
                Resumo
              </Link>

              <Link to="/categoria" onClick={closeMenu} className={mobileMenuLink}>
                <img src={category} alt="" className="w-6 h-6" />
                Categorias
              </Link>

              <Link to="/transacao" onClick={closeMenu} className={mobileMenuLink}>
                <img src={transaction} alt="" className="w-6 h-6" />
                Transações
              </Link>

              <button onClick={logout} className="text-left px-3 py-2 rounded text-white hover:bg-[#5A7067] transition-colors">
                Sair
              </button>
            </div>
          )}
        </nav>
      </header>

      {/* Conteúdo */}
      <main className="flex-1 p-4 md:p-6 backdrop-blur-sm">
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="h-10 text-white flex items-center justify-center text-sm backdrop-blur-sm">Controle Financeiro</footer>
    </div>
  );
}

const menuLink = "flex items-center gap-2 px-3 py-2 rounded text-white hover:bg-[#5A7067] transition-colors";

const mobileMenuLink = "flex items-center gap-2 px-3 py-2 rounded text-white hover:bg-[#5A7067] transition-colors";
