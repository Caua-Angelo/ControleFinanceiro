import { Link, Outlet } from "react-router-dom";
import category from "../assets/category.png";
import Finance from "../assets/finance.png";
import transaction from "../assets/transaction.png";
import logo from "../assets/logo.png";

export default function Layout() {
  function logout() {
    localStorage.removeItem("token");
    window.location.href = "/login";
  }
  return (
    <div className="h-screen w-full flex bg-[#9DB4AB]">
      {/* Menu lateral */}
      <aside className="w-72 bg-[#2F4F4F]/90 backdrop-blur-sm text-white p-5 flex flex-col shadow-[inset_-5px_0_10px_rgba(0,0,0,0.05)]">
        <div className="mb-8 flex flex-col items-center">
          <img src={logo} alt="Logo" className="w-12 h-12" />
          <h2 className="text-xl font-semibold mb-4">Controle Financeiro</h2>
        </div>
        <div className="ml-4">
          <Link to="/" className={` ${menuLink} flex items-center`}>
            <img src={Finance} alt="Logo" className="w-6 h-6 mr-2" /> Resumo
          </Link>
        </div>

        <div className="ml-4">
          <Link to="/categoria" className={`${menuLink} flex items-center`}>
            <img src={category} alt="Logo" className="w-6 h-6 mr-2" /> Categorias
          </Link>
        </div>
        <div className="ml-4">
          <Link to="/transacao" className={`${menuLink} flex items-center`}>
            <img src={transaction} alt="Logo" className="w-6 h-6 mr-2" /> Transações
          </Link>
        </div>
        <button
          onClick={logout}
          className=" mt-auto fundo [#7A9D8F] hover:bg-[#5A7067] px-3 py-2 rounded text-white text-2xl flex items-center justify-center"
        >
          Sair
        </button>
      </aside>

      {/* Área principal */}
      <div className="flex flex-col flex-1">
        {/* Conteúdo */}
        <main className="flex-1 overflow-y-auto p-6 backdrop-blur-sm">
          <Outlet />
        </main>

        {/* Footer */}
        <footer className="h-10 text-white flex items-center justify-center text-sm backdrop-blur-sm">Controle Financeiro</footer>
      </div>
    </div>
  );
}

const menuLink = "mb-2 px-2 py-1 rounded hover:bg-gray-600 transition-colors text-2xl";
