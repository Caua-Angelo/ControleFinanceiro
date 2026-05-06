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
    <div>
      <p>{label}</p>
    </div>
  );
}
