namespace ApiAccessors {
    export type ResourceInfo = {
        readonly url: string;
        readonly timeout: number;
        readonly method: string;
        readonly accesskey: string;
    };
    export interface IApiAccessor {
        getDataFromResource<TResult>(info: ResourceInfo): Promise<TResult>;
    };
    export class ApiAccessorError extends Error {
        public constructor(message: string) {
            super(message);
            this.name = 'ApiAccessorError';
        }
    }
    export class ApiAccessor implements IApiAccessor {
        public constructor() { }
        public async getDataFromResource<TResult = any>(info: ResourceInfo)
        : Promise<TResult> {
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), info.timeout);
            const result = await fetch(info.url, {
                method: info.method,
                signal: controller.signal,
                headers: {
                    'AccessKey': info.accesskey,
                    'Content-Type': 'application/json'
                }
            });
            if(result.status != 200) {
                throw new ApiAccessorError('Неудалось получить данные');
            }
            clearTimeout(timeoutId);
            return await result.json();
        }
    }
}
export default ApiAccessors;