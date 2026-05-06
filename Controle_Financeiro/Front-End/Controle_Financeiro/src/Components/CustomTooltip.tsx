type CustomTooltipProps = {
  active?: boolean;
  payload?: unknown[];
  label?: string;
};

export function CustomTooltip({ active, payload, label }: CustomTooltipProps) {
  if (!active || !payload || payload.length === 0) {
    return null;
  }

  return (
    <div>
      <p>{label}</p>
    </div>
  );
}
