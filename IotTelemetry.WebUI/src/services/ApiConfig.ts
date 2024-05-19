/* eslint-disable no-mixed-spaces-and-tabs */
// Импортируем ApiAccessor из локального модуля
import ApiAccesor from './ApiAccessor';

// Создаем объект с информацией о ресурсе бэкенда
export const backendRequest: ApiAccesor.ResourceInfo = {
	accesskey: 'a81a0933-0456-4883-aec8-b9a9198b25e4', // Ключ доступа к API
	timeout: 2000, // Таймаут для запросов в миллисекундах
	baseUrl: 'http://192.168.0.11:8000/telemetry' // Базовый URL для API
};

// Описываем тип данных для типа запроса
export type RequestType = {
	readonly name: string; // Название типа запроса
	readonly value: (() => ApiAccesor.RequestInfo) | null // Функция, возвращающая информацию о запросе, либо null
};
// Функция, возвращающая информацию о запросе за прошедшее время в минутах
const getPassTime = function(minutes: number): ApiAccesor.RequestInfo {
	const currentTime = new Date(Date.now()), // Текущее время
	      pastTime = new Date(Date.now()); // Время в прошлом
	pastTime.setMinutes(pastTime.getMinutes() - minutes); // Вычитаем указанное количество минут
	return { 
		fromDate: pastTime.toJSON(), // Время начала запроса в формате JSON
		toDate: currentTime.toJSON() // Время окончания запроса в формате JSON
	};
}
// Создаем массив типов запросов с их названиями и соответствующими функциями
export const requestsTypes: RequestType[] = [
	{ name: 'Текущее значение', value: null }, // Текущие значения, без параметров времени
	{ name: 'Среднее за минуту', value: () => getPassTime(1) }, // Среднее значение за последнюю минуту
	{ name: 'Среднее за 5 минут', value: () => getPassTime(5) }, // Среднее значение за последние 5 минут
	{ name: 'Среднее за 10 минут', value: () => getPassTime(10) }, // Среднее значение за последние 10 минут
];