using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotTelemetry.Data.Entities;
/// <summary>
/// Сущность получаемых данных с датчиков
/// </summary>
public partial class Sensor
{
    public int Id { get; set; }
    public float Temperature { get; set; } = default!; //Температура
    public float Humidity { get; set; } = default!; //Влажность
    public float Impurity { get; set; } = default!; //Загрязнение
    public DateTime DateFetch { get; set; } //Время получения данных с брокера
}
