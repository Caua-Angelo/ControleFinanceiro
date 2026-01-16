import { Link, Outlet } from "react-router-dom";
import bg from "../assets/Equilibrio.jpg";
import icon from "../assets/icone.png";

export default function Layout() {
  return (
    <div
      className="min-h-screen w-full flex bg-cover bg-center"
      style={{ backgroundImage: `url(${bg})` }}  
    >
      {/* Menu lateral */}
    <aside className="w-[200px] bg-black/50 text-white p-5 flex flex-col backdrop-blur-md border-r border-white/20">
    <div className='mb-8 flex flex-col items-center'>
      <img src={icon} alt="Logo" className="w-12 h-12" />
        <h2 className="text-xl font-semibold mb-4">Menu</h2>
</div>
        <Link to="/" className={menuLink}>Inicial</Link>
        <Link to="/usuario" className={menuLink}>Usuário</Link>
        <Link to="/categoria" className={menuLink}>Categoria</Link>
        <Link to="/transacao" className={menuLink}>Transação</Link>
      </aside>

      {/* Área principal */}
      <div className="flex flex-col flex-1">
        
        {/* Header */}
        <header className="h-[60px]  text-white flex items-center justify-center backdrop-blur-sm">
          <h1 className="text-lg font-semibold">Controle Financeiro</h1>
        </header>

        {/* Conteúdo */}
        <main className="flex-1 overflow-auto p-6  backdrop-blur-sm">
          <Outlet />
        </main>

        {/* Footer */}
        <footer className="h-[40px] text-white flex items-center justify-center text-sm backdrop-blur-sm">
         Controle Financeiro
        </footer>
      </div>
    </div>
  );
}

const menuLink =
  "mb-2 px-2 py-1 rounded hover:bg-gray-600 transition-colors text-xl"; 