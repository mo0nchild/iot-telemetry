// Импортируем React библиотеку
import React from 'react';

// Импортируем тип данных ChartInfo из основного модуля
import { ChartInfo } from './../main';

// Импортируем компонент GaugeComponent для отображения датчика
import GaugeComponent from 'react-gauge-component';

// Импортируем типы меток для компонента GaugeComponent
import { Labels } from 'react-gauge-component/dist/lib/GaugeComponent/types/Labels';

// Интерфейс для состояния компонента TelemetryChart
interface ChartState { value: number; }

// Класс компонента TelemetryChart, наследуемого от React.Component
class TelemetryChart extends React.Component<{info: ChartInfo}, ChartState> {
    // Стиль для компонента диаграммы
    private chartStyle: React.CSSProperties = {
        borderRadius: '20px',
        backgroundColor: '#444',
        border: '2px solid #FFF',
        padding: '10px',
        width: '100%',
        height: '100%'
    }

    // Конструктор класса, принимающий пропсы
    constructor(prop: {info: ChartInfo}) {
        super(prop);
        // Инициализируем состояние значением 0
        this.state = { value: 0 };
    }

    // Метод для обновления значения диаграммы
    public updateChartValue(value: number): void {
        // Обновляем состояние компонента с округленным значением
        this.setState({ value: Math.round(value) });
    }

    // Метод для рендеринга компонента
    public override render(): React.ReactNode {
        // Деструктуризация пропсов и состояния
        const { min, max, label, name } = this.props.info;
        const { value } = this.state;

        // Определяем метки для компонента GaugeComponent
        const chartLabels: Labels = {
            valueLabel: { 
                formatTextValue: (value: number) => `${(Math.round(value * 10) / 10).toFixed(1)}\n${label}`, // Форматирование значения
                style: { fontSize: 32 } // Стиль текста значения
            },
        };

        // Определяем значение диаграммы, ограниченное максимумом
        const chartValue = value > max ? max : value;

        // Возвращаем JSX для рендеринга компонента
        return (
            <div style={this.chartStyle}>
                <p style={{
                    fontSize: 22,
                    margin: '0px 0px 0px 10px',
                    color: '#FFF',
                    textDecoration: 'underline'
                }}>{name}</p>
                <GaugeComponent 
                    value={chartValue} // Значение датчика
                    labels={chartLabels} // Метки для датчика
                    maxValue={max} // Максимальное значение датчика
                    minValue={min} // Минимальное значение датчика
                    type='semicircle' // Тип датчика
                    arc={{
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
                    }} 
                    pointer={{
                        type: "blob", 
                        elastic: true, 
                        color: '#888'
                    }}
                />
            </div>
        );
    }
}

// Экспортируем компонент TelemetryChart по умолчанию
export default TelemetryChart;