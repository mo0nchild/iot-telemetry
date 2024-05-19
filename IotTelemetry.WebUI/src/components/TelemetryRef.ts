// Отключаем правило линтера для использования пространства имен
/* eslint-disable @typescript-eslint/no-namespace */

// Импортируем компонент TelemetryChart
import TelemetryChart from "./TelemetryChart";

// Импортируем тип данных ITelemetryState из компонента App
import { ITelemetryState } from "../App";

// Импортируем библиотеку React
import React from "react";

// Определяем пространство имен TelemetryReferences
namespace TelemetryReferences {

    // Интерфейс для ссылки на телеметрию
    export interface ITelemetryRef {
        updateTelemetry(state: ITelemetryState): void; // Метод для обновления телеметрии
        references: {
            temperature: React.RefObject<TelemetryChart>;
            humidity: React.RefObject<TelemetryChart>;
            impurity: React.RefObject<TelemetryChart>;
        }
    }

    // Класс, реализующий интерфейс ITelemetryRef
    export class TelemetryRef implements ITelemetryRef {
        // Создаем ссылки на компоненты TelemetryChart для температуры, влажности и загрязнения
        public references = { 
            temperature: React.createRef<TelemetryChart>(),
            humidity: React.createRef<TelemetryChart>(),
            impurity: React.createRef<TelemetryChart>(),
        };

        // Приватный метод для установки значения диаграммы через ссылку
        private setReferenceValue(refence: React.RefObject<TelemetryChart>, value: number): void {
            refence.current?.updateChartValue(value); // Обновляем значение диаграммы, если ссылка не пустая
        }

        // Метод для обновления всех телеметрических данных
        updateTelemetry(state: ITelemetryState): void {
            const { humidity, impurity, temperature } = this.references; // Деструктуризация ссылок
            this.setReferenceValue(temperature, state.temperature); // Обновляем значение температуры
            this.setReferenceValue(humidity, state.humidity); // Обновляем значение влажности
            this.setReferenceValue(impurity, state.impurity); // Обновляем значение загрязнения
        }
    }
}

// Экспортируем пространство имен TelemetryReferences по умолчанию
export default TelemetryReferences;