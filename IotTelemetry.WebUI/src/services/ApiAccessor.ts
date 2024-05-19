// Отключаем правила линтера для использования типа 'any' и пространства имен
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-namespace */

// Определяем пространство имен ApiAccessors
export namespace ApiAccessors {

    // Определяем тип данных для информации о ресурсе
    export type ResourceInfo = {
        readonly baseUrl: string; // Базовый URL API
        readonly timeout: number; // Таймаут для запросов в миллисекундах
        readonly accesskey: string; // Ключ доступа к API
    };

    // Определяем тип данных для информации о запросе
    export type RequestInfo = {
        readonly fromDate: string; // Начало периода запроса в формате ISO
        readonly toDate: string; // Конец периода запроса в формате ISO
    };

    // Определяем интерфейс для работы с API
    export interface IApiAccessor {
        getDataFromResource<TResult>(request: RequestInfo | null): Promise<TResult>; // Метод для получения данных из ресурса
    }

    // Определяем класс для ошибок доступа к API
    export class ApiAccessorError extends Error {
        public constructor(message: string) {
            super(message); // Вызываем конструктор родительского класса
            this.name = 'ApiAccessorError'; // Устанавливаем имя ошибки
        }
    }

    // Определяем класс для доступа к API, реализующий интерфейс IApiAccessor
    export class ApiAccessor extends Object implements IApiAccessor {
        private readonly apiInfo: ResourceInfo; // Приватное поле для хранения информации о ресурсе

        public constructor(info: ResourceInfo) {
            super(); // Вызываем конструктор родительского класса
            this.apiInfo = info; // Инициализируем поле apiInfo
        }

        // Реализуем метод для получения данных из ресурса
        public async getDataFromResource<TResult = any>(request: RequestInfo | null): Promise<TResult> {
            const controller = new AbortController(); // Создаем контроллер для отмены запроса
            const timeoutId = setTimeout(() => controller.abort(), this.apiInfo.timeout); // Устанавливаем таймаут для запроса

            const requestInfo: RequestInit = {
                method: 'GET', // Метод запроса
                signal: controller.signal, // Сигнал для отмены запроса
                headers: {
                    'AccessKey': this.apiInfo.accesskey, // Заголовок с ключом доступа
                    'Content-Type': 'application/json', // Заголовок с типом содержимого
                }
            };

            // Формируем путь запроса в зависимости от наличия информации о запросе
            const path = request == null ? 'current' : 'average?' + 
                new URLSearchParams({
                    'fromDate': request.fromDate, // Параметр начала периода
                    'toDate': request.toDate, // Параметр конца периода
                }).toString();

            let result: Response;
            try {
                // Выполняем запрос к API
                result = await fetch(`${this.apiInfo.baseUrl}/${path}`, requestInfo);
            }
            catch(error: any) { 
                // Обрабатываем ошибку при выполнении запроса
                throw new ApiAccessor(error.message); 
            }

            if(result.status != 200) {
                // Если статус ответа не 200, выбрасываем ошибку
                throw new ApiAccessorError('Не удалось получить данные');
            }

            clearTimeout(timeoutId); // Очищаем таймаут
            return await result.json(); // Возвращаем результат в формате JSON
        }
    }
}

// Экспортируем пространство имен ApiAccessors по умолчанию
export default ApiAccessors;