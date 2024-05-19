// Отключаем правило линтера для TypeScript, чтобы разрешить использование типа 'any'
/* eslint-disable @typescript-eslint/no-explicit-any */

// Импортируем React библиотеку
import React from 'react';

// Импортируем компоненты из библиотеки React Bootstrap
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

// Импортируем компонент Combobox из библиотеки react-widgets
import Combobox from "react-widgets/Combobox";

// Импортируем компоненты Chart и ChartRef из локального модуля
import { Chart, ChartRef } from './components/Imports';

// Импортируем сервис для работы с API
import ApiAccesor from './services/ApiAccessor';

// Импортируем информацию о графиках из основного модуля
import { chartInfo } from './main';

// Импортируем типы запросов и сам запрос из модуля конфигурации API
import { requestsTypes, RequestType } from './services/ApiConfig';

// Импортируем стили для приложения
import 'bootstrap/dist/css/bootstrap.min.css';
import "react-widgets/styles.css";
import './App.css';

// Интерфейс состояния телеметрии
export interface ITelemetryState {
	temperature: number;
	humidity: number;
	impurity: number;
}

// Тип для информации о ресурсе бекенда
type BackendInfo = ApiAccesor.ResourceInfo;

// Тип для сервиса бекенда
type BackendService = ApiAccesor.IApiAccessor;

// Класс компонента приложения, наследуемого от React.Component
class App extends React.Component<{backendInfo: BackendInfo}, ITelemetryState> {
	// Ссылка на компонент ChartRef
	private telemetryRef: ChartRef = new ChartRef();

	// Сервис телеметрии, инициализируемый информацией о бекенде из пропсов
	private telemetryService: BackendService = new ApiAccesor.ApiAccessor(this.props.backendInfo);

	// Тип запроса, который может быть null
	private requestType: ApiAccesor.RequestInfo | null = null;

	// Идентификатор интервала для обновления данных телеметрии
	private intervalId: number | null = null;

	// Начальное состояние телеметрии
	public state: ITelemetryState = {
		temperature: 0,
		humidity: 0,
		impurity: 0
	};

	// Конструктор класса, принимающий пропсы
	public constructor(prop: {backendInfo: BackendInfo}) {
		super(prop);
	}

	// Метод, который вызывается после монтирования компонента
	public componentDidMount(): void {
		this.intervalId = setInterval(() => this.updateTelemetry(), 1000);
	}

	// Асинхронный метод для получения данных телеметрии с сервера
	private getTelemetryFromServer = async (): Promise<ITelemetryState> => 
		await this.telemetryService.getDataFromResource<ITelemetryState>(this.requestType);

	// Метод для обновления данных телеметрии
	private updateTelemetry = async (): Promise<void> => {
		try {
			const result = await this.getTelemetryFromServer();
			console.log(result);
			this.setState({
				temperature: result.temperature,
				humidity: result.humidity,
				impurity: result.impurity
			});
			this.telemetryRef.updateTelemetry(result);
		}
		catch(error: any) { 
			// Обрабатываем ошибку, если это ошибка API
			if(error instanceof ApiAccesor.ApiAccessorError) console.log(error.message);
		}
	}

	// Метод для рендеринга компонента
	public render(): React.ReactNode {
		// Получаем ссылки на компоненты графиков
		const references = this.telemetryRef.references;

		// Создаем элементы для каждого графика
		const chart = Object.entries(references).map(([key, value]) => {
			return (
				<Col key={`div-${key}`} xl={'4'} md={'6'} sm={'12'}>
					<Chart info={chartInfo[key]} key={key} ref={value} />
				</Col>
			);
		});

		// Стили для компонента Combobox
		const comboBoxStyle: React.CSSProperties = { color: '#FFF' };

		// Возвращаем JSX для рендеринга компонента
		return (
			<div className='mx-0 mx-md-4'>
				<Container className='d-flex flex-column'>
					<Row className='justify-content-start'>
						<Col xs={'12'} md={'6'} xl={'4'}>
							<p style={{color: '#FFF', textAlign: 'start', margin: '0px 0px 5px'}}>
								Выберите период:
							</p>
							<Combobox defaultValue={requestsTypes[0].name} style={comboBoxStyle}
								data={requestsTypes} dataKey='name' textField='name'
								onChange={(value) => {
									const getData = (value as RequestType).value;
									this.requestType = getData == null ? null : getData();
									
									if(this.intervalId != null) clearInterval(this.intervalId);
									this.componentDidMount();
								}}
							/>
						</Col>
					</Row>
					<hr className='main-divider'/>
					<Row className='g-4'>{chart}</Row>
				</Container>
			</div>
		);
	}
}

// Экспортируем компонент App по умолчанию
export default App;