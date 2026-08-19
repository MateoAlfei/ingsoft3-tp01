interface PieSlice {
  label: string;
  value: number;
  color: string;
}

interface PieChartProps {
  data: PieSlice[];
  size?: number;
  strokeWidth?: number;
  centerValue?: string;
  centerLabel?: string;
}

// Separación visual entre porciones (unidades de stroke-dasharray, no px de pantalla).
const SEGMENT_GAP = 3;

export function PieChart({ data, size = 200, strokeWidth = 32, centerValue, centerLabel }: PieChartProps) {
  const total = data.reduce((sum, d) => sum + d.value, 0);
  const radius = (size - strokeWidth) / 2;
  const circumference = 2 * Math.PI * radius;
  const center = size / 2;

  let cumulativeFraction = 0;
  const segments = data.map((d) => {
    const fraction = total > 0 ? d.value / total : 0;
    const offset = cumulativeFraction;
    cumulativeFraction += fraction;
    return { ...d, fraction, offset };
  });

  return (
    <div className="pie-chart">
      <svg
        width={size}
        height={size}
        viewBox={`0 0 ${size} ${size}`}
        role="img"
        aria-label="Distribución de gastos por categoría"
      >
        {/* rotate -90: el primer segmento arranca arriba (12hs) en vez de a la derecha (3hs) */}
        <g transform={`rotate(-90 ${center} ${center})`}>
          {segments.map((s) => {
            const segmentLength = Math.max(s.fraction * circumference - SEGMENT_GAP, 0);
            return (
              <circle
                key={s.label}
                cx={center}
                cy={center}
                r={radius}
                fill="none"
                stroke={s.color}
                strokeWidth={strokeWidth}
                strokeDasharray={`${segmentLength} ${circumference - segmentLength}`}
                strokeDashoffset={-s.offset * circumference}
              >
                <title>
                  {s.label}: ${s.value.toFixed(2)} ({(s.fraction * 100).toFixed(0)}%)
                </title>
              </circle>
            );
          })}
        </g>

        {centerValue && (
          <text x={center} y={center - 4} textAnchor="middle" className="pie-chart-center-value">
            {centerValue}
          </text>
        )}
        {centerLabel && (
          <text x={center} y={center + 18} textAnchor="middle" className="pie-chart-center-label">
            {centerLabel}
          </text>
        )}
      </svg>

      <ul className="pie-chart-legend">
        {segments.map((s) => (
          <li key={s.label}>
            <span className="swatch" style={{ backgroundColor: s.color }} />
            <span className="legend-label">{s.label}</span>
            <span className="legend-value">
              ${s.value.toFixed(2)} · {(s.fraction * 100).toFixed(0)}%
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
}
