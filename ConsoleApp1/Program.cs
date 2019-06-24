using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;

namespace ArduinoController
{
    class Program
    {
        static SerialPort serialPort;
        static readonly string jobType = Configuration.GetConfig()["jobType"];


        public static void Main(string[] args)
        {
            var list = new List<Arduino>();
            DataTable dt = new DataTable();
            dt.Columns.Add("node");
            dt.Columns.Add("sensor");
            dt.Columns.Add("value");

            ReconnectAndManipulateData(list, dt);
        }


        static void ReconnectAndManipulateData(List<Arduino> list, DataTable dt)
        {   
            try { 
                serialPort = new SerialPort();
                serialPort.PortName = Configuration.GetConfig()["PortName"]; 
                serialPort.BaudRate = int.Parse(Configuration.GetConfig()["BaudRate"]);
                serialPort.Open();
                Console.WriteLine("Connection open: " + DateTime.Now);
                DataManipulation(list, dt);
            } 
            catch
            {
                Thread.Sleep(5000);
                ReconnectAndManipulateData(list, dt);
            }
        }

        private static void DataManipulation(List<Arduino> list, DataTable dt)
        {
            try
            {                 
                while (serialPort.IsOpen)
                {
                    // ValidationData:LongerLineForValidationPurpose|Arduino001{Luminosity:30,Air Temperature:26.10,Air Humidity:63.10,Soil Moisture:0}
                    string arduinoData = serialPort.ReadLine();

                    String[] elements = arduinoData.Split('|');  
                    try
                    {
                        if (elements[0].Split(":")[0] != "ValidationData")
                        {
                            continue;
                        }
                        else if (elements[0].Split(":")[1] != "LongerLineForValidationPurpose")
                        {
                            continue;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    string pattern1 = @"[{}]";
                    String[] nodeSensors = Regex.Split(elements[1], pattern1);
                    String[] sensors = nodeSensors[1].Split(",");

                    foreach (String element in sensors)
                    {
                        Arduino arduino = new Arduino(nodeSensors[0], element.Split(':')[0], element.Split(':')[1]);

                        if (list.Count < (int.Parse(Configuration.GetConfig()["numberOfObjectsInList"]) * sensors.Length - 1))
                        {
                            list.Add(arduino);
                            Thread.Sleep(1500);//for 100objects(400sensor values) = 1000ms done in 06:40, 2000ms done in 13:20
                        }

                        else
                        {
                            list.Add(arduino);
                            foreach (Arduino item in list)
                            {
                                dt.Rows.Add(item.NodeId, item.SensorValue.Name, item.SensorValue.Value);
                            }

                            switch (jobType)
                            {
                                case "db":
                                    DbAccess.ExecSqlProcedure(dt);
                                    dt.Clear();
                                    Console.WriteLine("Inserted " + list.Count + " rows in database.");
                                    break;
                                case "http":
                                    foreach (DataRow dtRow in dt.Rows)
                                    {
                                        HttpAccess.httpWrite(dtRow["node"].ToString(), dtRow["sensor"].ToString(), dtRow["value"].ToString()).Wait();
                                    }
                                    Console.WriteLine("Http request sent.");
                                    dt.Clear();
                                    break;
                                default:
                                    Console.WriteLine("Job type must be declared as db or http");
                                    break;
                            }
                            list.Clear();
                        }
                    }
                }                
            }
            catch (Exception e)
            {   
                Console.WriteLine(e.Message);
            }
            finally
            {
                dt.Clear();
                list.Clear();
                Console.WriteLine("Connection lost: " + DateTime.Now);
                ReconnectAndManipulateData(list, dt);
            }
        }
    }
}
