import ApiAccesor from './ApiAccessor'

export const backendRequest: ApiAccesor.ResourceInfo = {
	accesskey: 'a81a0933-0456-4883-aec8-b9a9198b25e4',
	timeout: 2000,
	baseUrl: 'http://192.168.0.10:8000/telemetry'
};
export type RequestType = {
	readonly name: string;
	readonly value: (() => ApiAccesor.RequestInfo) | null
}
const getPassTime = function(minutes: number): ApiAccesor.RequestInfo {
	const currentTime = new Date(Date.now()), pastTime = new Date(Date.now());
	pastTime.setMinutes(pastTime.getMinutes() - minutes);
	return { 
		fromDate: pastTime.toJSON(), 
		toDate: currentTime.toJSON() 
	}
}
export const requestsTypes: RequestType[] = [
	{ name: 'Текущее значение', value: null },
	{ name: 'Среднее за минуту', value: () => getPassTime(1) },
	{ name: 'Среднее за 5 минут', value: () => getPassTime(5) },
	{ name: 'Среднее за 10 минут', value: () => getPassTime(10) },
]; 