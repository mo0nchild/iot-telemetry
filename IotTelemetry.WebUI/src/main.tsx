// Импортируем React библиотеку
import React from 'react';

// Импортируем методы для работы с DOM из React
import ReactDOM from 'react-dom/client';

// Импортируем основной компонент приложения
import App from './App.tsx';

// Импортируем функцию для запросов к бэкенду
import { backendRequest } from './services/ApiConfig.ts';

// Импортируем стили для приложения
import './index.css';

// Описываем тип данных для информации о графике
export type ChartInfo = { 
	max: number, // Максимальное значение графика
	min: number, // Минимальное значение графика
	label: string, // Единица измерения
	name: string // Имя параметра
};

// Описываем тип данных для информации о телеметрии, где ключ - строка, а значение - ChartInfo
export type TelemetryChartInfo = { [key: string]: ChartInfo; };

// Создаем объект с информацией о разных типах графиков телеметрии
export const chartInfo: TelemetryChartInfo = {
	'temperature': { 
		max: 100, // Максимальная температура
		min: -100, // Минимальная температура
		label: '[C]', // Единица измерения температуры
		name: 'Температура' // Название параметра температуры
	},
	'humidity': { 
		max: 100, // Максимальная влажность
		min: 0, // Минимальная влажность
		label: '[%]', // Единица измерения влажности
		name: 'Влажность' // Название параметра влажности
	},
	'impurity': { 
		max: 10000, // Максимальное загрязнение
		min: 0, // Минимальное загрязнение
		label: '[ppm]', // Единица измерения загрязнения
		name: 'Загрязнение' // Название параметра загрязнения
	},
};

// Рендерим приложение в корневой элемент DOM
ReactDOM.createRoot(document.getElementById('root')!).render(
	<React.StrictMode>
		<App backendInfo={backendRequest} /> {/* Передаем информацию о бэкенде в компонент App */}
	</React.StrictMode>,
);