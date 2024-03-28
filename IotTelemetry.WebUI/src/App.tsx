import React from 'react';
import TelemetryChart from './components/TelemetryChart';
import Reference from './components/TelemetryRef';
import ApiAccesor from './services/ApiAccessor'
import { chartInfo } from './main'
import './App.css';

export interface ITelemetryState {
	temperature: number;
	humidity: number;
	impurity: number;
}
type BackendInfo = ApiAccesor.ResourceInfo;
type BackendService = ApiAccesor.IApiAccessor;

class App extends React.Component<{backendInfo: BackendInfo}, ITelemetryState> {
	private telemetryRef: Reference.ITelemetryRef = new Reference.TelemetryRef();
	private telemetryService: BackendService = new ApiAccesor.ApiAccessor();
	public state: ITelemetryState = {
		temperature: 0,
		humidity: 0,
		impurity: 0
	};
	public constructor(prop: {backendInfo: BackendInfo}) {
		super(prop);
	}
	public componentDidMount(): void {
		setInterval(() => this.updateTelemetry(), 2000);
	}
	private getTelemetryFromServer = async (): Promise<ITelemetryState> => 
	{
		const backendInfo: BackendInfo = this.props.backendInfo;
		return await this.telemetryService
			.getDataFromResource<ITelemetryState>(backendInfo);
	}
	private updateTelemetry = async (): Promise<void> => {
		try {
			const result = await this.getTelemetryFromServer();
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
				<div key={`div-${key}`} className='col-12 col-xl-5 col-md-6'>
					<TelemetryChart info={chartInfo[key]} key={key} ref={value} />
				</div>
			);
		});
		const rowClass = 'row g-4 justify-content-center justify-content-md-start';
		return (
			<div className="container text-center">
				<div className={rowClass}>{chart}</div>
			</div>
		);
	}
}
export default App;
