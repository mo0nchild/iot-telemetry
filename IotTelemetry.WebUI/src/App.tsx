/* eslint-disable @typescript-eslint/no-explicit-any */
import React from 'react';
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Combobox from "react-widgets/Combobox";

import { Chart, ChartRef } from './components/Imports'
import ApiAccesor from './services/ApiAccessor'
import { chartInfo } from './main'
import { requestsTypes, RequestType } from './services/ApiConfig'

import 'bootstrap/dist/css/bootstrap.min.css';
import "react-widgets/styles.css";
import './App.css';

export interface ITelemetryState {
	temperature: number;
	humidity: number;
	impurity: number;
}
type BackendInfo = ApiAccesor.ResourceInfo;
type BackendService = ApiAccesor.IApiAccessor;

class App extends React.Component<{backendInfo: BackendInfo}, ITelemetryState> {
	private telemetryRef: ChartRef = new ChartRef();
	private telemetryService: BackendService = new ApiAccesor.ApiAccessor(
		this.props.backendInfo);
	private requestType: ApiAccesor.RequestInfo | null = null;
	private intervalId: number | null = null;
	public state: ITelemetryState = {
		temperature: 0,
		humidity: 0,
		impurity: 0
	};
	public constructor(prop: {backendInfo: BackendInfo}) {
		super(prop);
	}
	public componentDidMount(): void {
		this.intervalId = setInterval(() => this.updateTelemetry(), 1000);
	}
	private getTelemetryFromServer = async (): Promise<ITelemetryState> => 
		await this.telemetryService.getDataFromResource<ITelemetryState>(this.requestType);

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
			if(error instanceof ApiAccesor.ApiAccessorError) console.log(error.message);
		}
	}
	public render(): React.ReactNode {
		const references = this.telemetryRef.references;
		const chart = Object.entries(references).map(([key, value]) => {
			return (
				<Col key={`div-${key}`} xl={'4'} md={'6'} sm={'12'}>
					<Chart info={chartInfo[key]} key={key} ref={value} />
				</Col>
			);
		});
		const comboBoxStyle: React.CSSProperties = { color: '#FFF' }
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
export default App;
