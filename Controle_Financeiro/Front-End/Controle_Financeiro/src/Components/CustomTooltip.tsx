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
  const saldoValor = saldo?.value ?? 0;

  return (
    <div className="bg-white border border-gray-200 rounded-lg shadow-lg px-4 py-3 min-w-[220px]">
      <p className="text-sm font-semibold text-gray-700 mb-3">{label}</p>

      <div className="space-y-2">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="w-2 h-2 rounded-full bg-green-600" />
            <span className="text-sm text-green-700">Receitas</span>
          </div>
          <span className="text-sm font-medium">{formatarValor(receita?.value ?? 0)}</span>
        </div>

        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="w-2 h-2 rounded-full bg-red-600" />
            <span className="text-sm text-red-700">Despesas</span>
          </div>
          <span className="text-sm font-medium">{formatarValor(despesa?.value ?? 0)}</span>
        </div>

        <div className="border-t pt-2  flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className={`w-2 h-2 mr-2 rounded-full ${saldoValor >= 0 ? "bg-green-600" : "bg-red-600"}`} />

            <span className={`text-sm mr-2  font-semibold ${saldoValor >= 0 ? "text-green-700" : "text-red-700"}`}>Saldo acumulado</span>
          </div>

          <span className={`text-sm font-bold ${saldoValor >= 0 ? "text-green-700" : "text-red-700"}`}>{formatarValor(saldoValor)}</span>
        </div>
      </div>
    </div>
  );
}
