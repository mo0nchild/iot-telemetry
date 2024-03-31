/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-namespace */
export namespace ApiAccessors {
    export type ResourceInfo = {
        readonly baseUrl: string;
        readonly timeout: number;
        readonly method: string;
        readonly accesskey: string;
    };
    export type RequestInfo = {
        readonly fromDate: string;
        readonly toDate: string;
    }
    export interface IApiAccessor {
        getDataFromResource<TResult>(request: RequestInfo | null): Promise<TResult>;
    }
    export class ApiAccessorError extends Error {
        public constructor(message: string) {
            super(message);
            this.name = 'ApiAccessorError';
        }
    }
    export class ApiAccessor extends Object implements IApiAccessor {
        private readonly apiInfo: ResourceInfo;
        public constructor(info: ResourceInfo) {
            super();
            this.apiInfo = info;
        }
        public async getDataFromResource<TResult = any>(request: RequestInfo | null)
        : Promise<TResult> {
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), this.apiInfo.timeout);
            const requestInfo: RequestInit = {
                method: this.apiInfo.method,
                signal: controller.signal,
                headers: {
                    'AccessKey': this.apiInfo.accesskey,
                    'Content-Type': 'application/json'
                },
                body: request == null ? null : JSON.stringify(request)
            };
            const path = request == null ? 'current' : 'average'
            let result: Response;
            try {
                result = await fetch(`${this.apiInfo.baseUrl}/${path}`, requestInfo);
            }
            catch(error: any) { throw new ApiAccessor(error.message) }
            if(result.status != 200) {
                throw new ApiAccessorError('Неудалось получить данные');
            }
            clearTimeout(timeoutId);
            return await result.json();
        }
    }
}
export default ApiAccessors;