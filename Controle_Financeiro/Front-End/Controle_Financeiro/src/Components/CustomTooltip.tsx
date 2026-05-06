import { formatarValor } from "../Utils/formatarValor";
type TooltipPayloadItem = {
  dataKey: string;
  value: number;
  name: string;
};

type CustomTooltipProps = {
  active?: boolean;
  payload?: TooltipPayloadItem[];
  label?: string;
};

export function CustomTooltip({ active, payload, label }: CustomTooltipProps) {
  if (!active || !payload || payload.length === 0) {
    return null;
  }
  const receita = payload.find((item) => item.dataKey === "receita");
  const despesa = payload.find((item) => item.dataKey === "despesa");
  const saldo = payload.find((item) => item.dataKey === "saldoAcumulado");

  return (
    <div className="bg-white border border-gray-200 rounded-lg shadow-lg px-4 py-3 min-w-[220px]">
      <p className="text-sm font-semibold text-gray-700 mb-3">{label}</p>

      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <span className="text-sm text-green-700">Receitas</span>
          <span className="text-sm font-medium">{formatarValor(receita?.value ?? 0)}</span>
        </div>

        <div className="flex items-center justify-between">
          <span className="text-sm text-red-700">Despesas</span>
          <span className="text-sm font-medium">{formatarValor(despesa?.value ?? 0)}</span>
        </div>

        <div className="border-t pt-2 flex items-center justify-between">
          <span className="text-sm font-semibold text-blue-700">Saldo acumulado</span>

          <span className="text-sm font-bold text-blue-700">{formatarValor(saldo?.value ?? 0)}</span>
        </div>
      </div>
    </div>
  );
}
