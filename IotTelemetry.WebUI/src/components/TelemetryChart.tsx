import React from 'react';
import { ChartInfo } from './../main'
import GaugeComponent from 'react-gauge-component'
import { Labels } from 'react-gauge-component/dist/lib/GaugeComponent/types/Labels';

interface ChartState { value: number; }
class TelemetryChart extends React.Component<{info: ChartInfo}, ChartState> {
    private chartStyle: React.CSSProperties = {
        borderRadius: '20px',
        backgroundColor: '#444',
        border: '2px solid #FFF',
        padding: '10px',
        width: '100%',
        height: '100%'
    }
    constructor(prop: {info: ChartInfo}) {
        super(prop);
        this.state = { value: 0 };
    }
    public updateChartValue(value: number): void {
        this.setState({ value: Math.round(value) });
    }
    public override render(): React.ReactNode {
        const { min, max, label, name } = this.props.info;
        const { value } = this.state;
        const chartLabels: Labels = {
            valueLabel: { 
                formatTextValue: (value: number) => `${(Math.round(value * 10) / 10).toFixed(1)}\n${label}`,
                style: { fontSize: 32 }
            },
        };
        const chartValue = value > max ? max : value;
        return (
            <div style={this.chartStyle}>
                <p style={{
                    fontSize: 22,
                    margin: '0px 0px 0px 10px',
                    color: '#FFF',
                    textDecoration: 'underline'
                }}>{name}</p>
                <GaugeComponent value={chartValue} labels={chartLabels}
                    maxValue={max} minValue={min} type='semicircle' arc={{
                        gradient: false,
                        cornerRadius: 10,
                        subArcs: [
                            {
                                limit: 0,
                                color: '#3080FF',
                                showTick: true
                            },
                            {
                                limit: Math.floor(max / 4),
                                color: '#5BE12C',
                                showTick: true
                            },
                            {
                                limit: Math.floor(max / 2),
                                color: '#F5CD19',
                                showTick: true
                            },
                            {
                                limit: Math.floor(max * 3 / 4),
                                color: '#F5CD19',
                                showTick: true
                            },
                            {
                                limit: Math.floor(max),
                                color: '#EA4228',
                                showTick: true
                            },
                        ]
                    }} pointer={
                        {type: "blob", elastic: true, color: '#888'}
                    }/>
            </div>
        );
    }
}
export default TelemetryChart;