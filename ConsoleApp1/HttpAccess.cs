using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArduinoController
{
    class HttpAccess
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> httpWrite(string nodeId, string sensorName, string sensorValue)
        {
            string vpip = Configuration.GetConfig()["ArgoTechIP"];
            
            var values = new Dictionary<string, string> {
                                { "Path", "/Digital Lab/ChocolateLine/" + nodeId },
                                { "Name", nodeId},
                                { "Sensor Name", sensorName },
                                { "Sensor Value", sensorValue }};

            var content = new FormUrlEncodedContent(values);
            try {

                //var restresponse = await client.PostAsync("http://" + vpip + "/SmartwatchDemoRoomService/api/VirtualPlantConnector/WriteVariable", content);
                var restresponse = await client.PostAsync("http://" + vpip + "/SmartwatchDemoRoomService/api/AgroTech/arduinoData", content);
                var responseString = await restresponse.Content.ReadAsStringAsync();

                return restresponse.IsSuccessStatusCode;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException == null ? ex.Message.ToString() : ex.InnerException.ToString());
                return false;
            }
            
        }
    }
}
