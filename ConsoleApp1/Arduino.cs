
namespace ArduinoController
{
    public class Arduino
    {
        public string NodeId { get; set; }
        public Sensor SensorValue { get; set; }


        public Arduino() { }

        public Arduino(string NodeId) {
            this.NodeId = NodeId;
        }

        public Arduino(string NodeId, string sensorName, object sensorValue)
        {
            this.NodeId = NodeId;
            this.SensorValue = new Sensor(sensorName, sensorValue);
        }

    }
}
