import TelemetryChart from "./TelemetryChart";
import { ITelemetryState } from "../App";
import React from "react";

namespace TelemetryReferences {
    export interface ITelemetryRef {
        updateTelemetry(state: ITelemetryState): void;
        references: {
            temperature: React.RefObject<TelemetryChart>;
            humidity: React.RefObject<TelemetryChart>;
            impurity: React.RefObject<TelemetryChart>;
        }
    }
    export class TelemetryRef implements ITelemetryRef {
        public references = { 
            temperature: React.createRef<TelemetryChart>(),
            humidity: React.createRef<TelemetryChart>(),
            impurity: React.createRef<TelemetryChart>(),
        };
        private setReferenceValue(refence: React.RefObject<TelemetryChart>,
            value: number): void {
            refence.current?.updateChartValue(value);
        }
        updateTelemetry(state: ITelemetryState): void {
            const { humidity, impurity, temperature } = this.references;
            this.setReferenceValue(temperature, state.temperature);
            this.setReferenceValue(humidity, state.humidity);
            this.setReferenceValue(impurity, state.impurity);
        }
    }
}
export default TelemetryReferences;