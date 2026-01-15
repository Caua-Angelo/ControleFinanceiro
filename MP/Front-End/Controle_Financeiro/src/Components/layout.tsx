import React from "react";
import { Link, Outlet } from "react-router-dom";

export default function Layout() {
    return (
        <div className="flex h-screen w-full">

            {/* Menu lateral */}
            <aside className="w-[200px] bg-sky-400 text-white p-5 flex flex-col">
                <h2 className="text-xl font-semibold mb-4">Menu</h2>

                <Link to="/" className={menuLink}>Inicial</Link>
                <Link to="/categoria" className={menuLink}>Categoria</Link>
                <Link to="/transacao" className={menuLink}>Transação</Link>
                <Link to="/usuario" className={menuLink}>Usuário</Link>
            </aside>

            {/* Conteúdo principal */}
            <div className="flex flex-col flex-1">

                {/* Header */}
                <header className="h-[60px] bg-blue-600 text-white flex items-center justify-center px-5">
                    <h1 className="text-lg font-semibold">Controle Financeiro</h1>
                </header>

                {/* Conteúdo dinâmico */}
                <main className="flex-1 overflow-auto bg-blue-400 p-5">
                    <Outlet />
                </main>

                {/* Footer */}
                <footer className="h-[40px] bg-gray-800 text-white flex items-center justify-center text-sm">
                    © 2026 Controle Financeiro
                </footer>
            </div>
        </div>
    );
}

const menuLink =
    "mb-2 text-white hover:bg-sky-500 rounded px-2 py-1 transition-colors";
