#include <ESP8266WiFi.h>
#include <MQ135.h>
#include <DHT.h>
#include <PubSubClient.h>

#define DHT_PIN D4
#define DHT_TYPE DHT22
#define PIN_MQ135 A0
#define MSG_BUFFER_SIZE	(255)

const char* ssid     = "my-home";         // The SSID (name) of the Wi-Fi network 
const char* password = "Prolodgy778";     // The Password of the Wi-Fi network 
const char* mqtt_server = "192.168.0.10";
const unsigned int mqtt_port = 5000;

DHT dht(DHT_PIN, DHT_TYPE);
MQ135 mq135_sensor(PIN_MQ135);

WiFiClient espClient;
PubSubClient client(espClient);

unsigned long lastMsg = 0;
char msg[MSG_BUFFER_SIZE];

void initializationWifi() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  uint i = 0;
  while (WiFi.status() != WL_CONNECTED) { // Wait for the Wi-Fi to connect
    delay(1000);
    Serial.print(++i); Serial.print(' ');
  }
  randomSeed(micros());
  Serial.println('\n');
  Serial.println("Connection established!");  
  Serial.print("IP address:\t");
  Serial.println(WiFi.localIP());
}

void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i = 0; i < length; i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();
}

void reconnectToServer() {
  while (!client.connected()) {

    Serial.print("Attempting MQTT connection...");
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);

    if (client.connect(clientId.c_str())) {
      Serial.println("connected");
      client.subscribe("esp8266/check");
    } 
    else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");

      delay(5000);
    }
  }
}

void setup() {
  Serial.begin(115200);
  initializationWifi();
  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);
  dht.begin();
}

void loop() {
  if (!client.connected()) {
    reconnectToServer();
  }
  client.loop();

  unsigned long now = millis();
  if (now - lastMsg > 2000) {
    lastMsg = now;

    float humidity = dht.readHumidity(true);
    float temperature = dht.readTemperature();

    float ppm = mq135_sensor.getPPM();
    float correctedPPM = mq135_sensor.getCorrectedPPM(temperature, humidity);

    snprintf (msg, MSG_BUFFER_SIZE, "temp: %.3f; hum: %.3f; ppm: %.3f", temperature, humidity, correctedPPM);
    Serial.print("Publish message: ");
    Serial.println(msg);
    client.publish("esp8266/data", msg);
  }
}
