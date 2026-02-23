import trashCan from "../assets/trash.png";
import pencil from "../assets/pencil.png";
interface ButtonProps {
  label: string;
  onClick: () => void;
  className?: string;
  variant?: "primary" | "edit" | "delete";
}

export function Button({
  label,
  onClick,
  className = "",
  variant = "primary",
}: ButtonProps) {
  const variantClass = {
    primary: "bg-[#7A9D8F] hover:bg-[#5A7067] w-full h-11",
    edit: "bg-[#7A9D8F] hover:bg-[#5A7067] px-3 py-2",
    delete: "bg-[#AD675C] hover:bg-[#6d2a21] px-3 py-2",
  }[variant];

  const variantIcon = {
    primary: null,
    edit: pencil,
    delete: trashCan,
  }[variant];
  return (
    <button
      onClick={onClick}
      className={`flex items-center justify-center gap-1 text-white rounded transition ${variantClass} ${className}`}
    >
      {variantIcon && <img src={variantIcon} alt={label} className="w-5 h-5" />}
      {label}
    </button>
  );
}
