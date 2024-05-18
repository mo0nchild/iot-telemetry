#include <ESP8266WiFi.h>  // Библиотека для работы с Wi-Fi на микроконтроллере ESP8266
#include <MQ135.h>        // Библиотека для работы с датчиком качества воздуха MQ135
#include <DHT.h>          // Библиотека для работы с датчиками температуры и влажности DHT
#include <PubSubClient.h> // Библиотека для работы с MQTT протоколом

#define DHT_PIN D2        // Пин, к которому подключен датчик DHT
#define DHT_TYPE DHT22    // Тип датчика DHT (в данном случае DHT22)
#define PIN_MQ135 A0      // Пин, к которому подключен датчик MQ135
#define MSG_BUFFER_SIZE (255) // Размер буфера для сообщений

const char* ssid = "my-home";          // Имя Wi-Fi сети
const char* password = "";  // Пароль от Wi-Fi сети
const char* mqtt_server = "192.168.0.10"; // IP-адрес MQTT сервера
const unsigned int mqtt_port = 5000;   // Порт MQTT сервера
const float ppm_infelicity = 100.0;    // Коэффициент для корректировки PPM

DHT dht(DHT_PIN, DHT_TYPE);  // Создание объекта для работы с DHT датчиком
MQ135 mq135_sensor(PIN_MQ135); // Создание объекта для работы с MQ135 датчиком

WiFiClient espClient;          // Создание WiFi клиента
PubSubClient client(espClient); // Создание MQTT клиента с использованием WiFi клиента

unsigned long lastMsg = 0;     // Переменная для хранения времени последнего сообщения
char msg[MSG_BUFFER_SIZE];     // Буфер для хранения сообщений

void initializationWifi() {
  WiFi.mode(WIFI_STA);        // Установка режима работы Wi-Fi в режим станции (клиента)
  WiFi.begin(ssid, password); // Подключение к Wi-Fi сети
  uint i = 0;
  while (WiFi.status() != WL_CONNECTED) { // Ожидание подключения к Wi-Fi сети
    delay(1000);
    Serial.print(++i); Serial.print(' ');
  }
  randomSeed(micros());       // Инициализация генератора случайных чисел
  Serial.println('\n');
  Serial.println("Connection established!");  
  Serial.print("IP address:\t");
  Serial.println(WiFi.localIP()); // Вывод IP-адреса устройства
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived ["); // Вывод информации о полученном сообщении
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();
}

void reconnectToServer() {
  while (!client.connected()) { // Повторное подключение к MQTT серверу, если соединение потеряно
    Serial.print("Attempting MQTT connection...");
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX); // Генерация уникального ID клиента

    if (client.connect(clientId.c_str())) { // Попытка подключения
      Serial.println("connected");
      client.subscribe("esp8266/check"); // Подписка на топик
    } 
    else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      delay(5000); // Задержка перед повторной попыткой
    }
  }
}

void setup() {
  Serial.begin(115200);          // Инициализация последовательного соединения
  initializationWifi();          // Подключение к Wi-Fi
  client.setServer(mqtt_server, mqtt_port); // Настройка MQTT сервера
  client.setCallback(callback);  // Установка функции обратного вызова для обработки сообщений
  dht.begin();                   // Инициализация DHT датчика
}

void loop() {
  if (!client.connected()) {     // Проверка подключения к MQTT серверу
    reconnectToServer();         // Попытка повторного подключения
  }
  client.loop();                 // Обработка входящих и исходящих MQTT сообщений

  unsigned long now = millis();  // Текущее время
  if (now - lastMsg > 2000) {    // Отправка данных каждые 2 секунды
    lastMsg = now;

    float humidity = dht.readHumidity(true); // Чтение влажности с DHT датчика
    float temperature = dht.readTemperature(); // Чтение температуры с DHT датчика

    float ppm = mq135_sensor.getPPM(); // Чтение PPM с MQ135 датчика
    float correctedPPM = mq135_sensor.getCorrectedPPM(temperature, humidity) / ppm_infelicity; // Коррекция PPM

    snprintf(msg, MSG_BUFFER_SIZE, "temp: %.3f; hum: %.3f; ppm: %.3f", temperature, humidity, correctedPPM); // Форматирование сообщения
    Serial.print("Publish message: ");
    Serial.println(msg);
    client.publish("esp8266/data", msg); // Публикация сообщения в MQTT топик
  }
}