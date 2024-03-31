import TelemetryChart from './TelemetryChart'
import TelemetryRef from './TelemetryRef'

// export class ChartRef extends TelemetryRef.TelemetryRef { }
export import ChartRef = TelemetryRef.TelemetryRef;
export { TelemetryChart as Chart, };
