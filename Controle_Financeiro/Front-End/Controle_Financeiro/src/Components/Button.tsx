import trashCan from "../assets/trash.png";
import pencil from "../assets/pencil.png";
interface ButtonProps {
  label: string;
  onClick: () => void;
  className?: string;
  variant?: "primary" | "edit" | "delete" | "saveModal" | "cancelModal" | "paginacao";
  // Props específicas para paginação
  totalPaginas?: number;
  paginaAtual?: number;
  setPaginaAtual?: (pagina: number) => void;
}

export function Button({ label, onClick, className = "", variant = "primary", totalPaginas, paginaAtual, setPaginaAtual }: ButtonProps) {
  const variantClass =
    {
      primary: "bg-[#7A9D8F] hover:bg-[#5A7067] w-full h-11",
      edit: "bg-[#7A9D8F] hover:bg-[#5A7067] px-3 py-2",
      delete: "bg-[#AD675C] hover:bg-[#6d2a21] px-3 py-2",
      saveModal: "px-4 py-2 bg-[#7A9D8F] text-white rounded hover:bg-[#5A7067]",
      cancelModal: "px-4 py-2 bg-[#C8D6D1] text-[#2F4F4F] rounded hover:bg-[#9DB4AB]",
      paginacao: "w-fit mx-auto",
    }[variant] ?? "";

  const variantIcon =
    {
      primary: null,
      edit: pencil,
      delete: trashCan,
      cancelModal: null,
      saveModal: null,
      paginacao: null,
    }[variant] ?? null;

  // Renderização especial para paginação
  if (variant === "paginacao" && totalPaginas && setPaginaAtual) {
    return (
      <div className={`flex justify-center  gap-2 mt-6 w-fit mx-auto  ${className}`}>
        {Array.from({ length: totalPaginas }).map((_, index) => {
          const pagina = index + 1;
          return (
            <button
              key={pagina}
              onClick={() => setPaginaAtual(pagina)}
              className={`px-3 py-1 rounded ${paginaAtual === pagina ? "bg-[#2F4F4F] text-white" : "bg-[#D4E2DC] hover:bg-[#C8D6D1] text-[#2F4F4F]"}`}
            >
              {pagina}
            </button>
          );
        })}
      </div>
    );
  }
  //retorno normal para os outros tipos de botão
  return (
    <button onClick={onClick} className={`flex items-center justify-center gap-1 text-white rounded transition ${variantClass} ${className}`}>
      {variantIcon && <img src={variantIcon} alt={label} className="w-5 h-5" />}
      {label}
    </button>
  );
}
