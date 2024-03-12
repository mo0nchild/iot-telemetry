import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import { backendRequest } from './services/ApiConfig.ts'
import './index.css'

export type ChartInfo = { 
	max: number, 
	min: number, 
	label: string,
	name: string
}
export type TelemetryChartInfo = { [key: string]: ChartInfo; };
export const chartInfo: TelemetryChartInfo = {
	'temperature': { 
		max: 100,
		min: -100, 
		label: '[C]', 
		name: 'Температура'
	},
	'humidity': { 
		max: 100, 
		min: 0, 
		label: '[%]',
		name: 'Влажность'
	},
	'impurity': { 
		max: 10000, 
		min: 0, 
		label: '[ppm]',
		name: 'Загрязнение'
	},
};
// export const backendRequest: ApiAccesor.ResourceInfo = {
// 	accesskey: 'a81a0933-0456-4883-aec8-b9a9198b25e4',
// 	method: 'GET',
// 	timeout: 2000,
// 	url: 'your-url-to-api'
// };
ReactDOM.createRoot(document.getElementById('root')!).render(
	<React.StrictMode>
		<App backendInfo={backendRequest} />
	</React.StrictMode>,
)
