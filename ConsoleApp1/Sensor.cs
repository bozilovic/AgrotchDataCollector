
namespace ArduinoController
{
    public class Sensor
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public Sensor() { }

        public Sensor(string sensorName)
        {
            Name = sensorName;
        }

        public Sensor(string sensorName, object sensorValue)
        {
            Name = sensorName;
            Value = sensorValue;
        }
    }
}
