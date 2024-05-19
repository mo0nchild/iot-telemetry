// Импортируем функцию defineConfig из Vite
import { defineConfig } from 'vite';
// Импортируем плагин для поддержки React в Vite
import react from '@vitejs/plugin-react';

// Экспортируем конфигурацию по умолчанию для Vite
export default defineConfig({
  // Конфигурация сервера разработки
  server: {
    // Указываем хост для сервера разработки
    host: '0.0.0.0',
    // Указываем порт для сервера разработки
    port: 3000,
  },
  // Подключаем плагины, в данном случае плагин для поддержки React
  plugins: [react()],
});
