using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotData.Entities
{
    public class Indicator
    {
        public float Temperature { get; set; } = default!; //Температура
        public float Humidity { get; set; } = default!; //Влажность
        public float Impurity { get; set; } = default!; //Загрязнение
    }
}
