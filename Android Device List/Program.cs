using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Android_Device_List
{
    class Program
    {
        public class DeviceModel
        {
            public string brand { get; set; }
            public string name { get; set; }
            public string device { get; set; }
            public string model { get; set; }
        }

        static void Main(string[] args)
        {
            //Download google supported device list
            // On the CSV file, devices are ordered alphabetically (A-Z) by manufacturer name and listed in the following format:
            // Retail brand, marketing name, build.os.DEVICE, build.os.MODEL
            string deviceCSV = DownloadDeviceCSV();

            //Split result by new line
            string[] lines = deviceCSV.Split( new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            List<string> brandJSON = new List<string>();
            List<DeviceModel> deviceJSON = new List<DeviceModel>();
            //Frist line is format information [Retail Branding,Marketing Name,Device,Model]
            for (int i = 1; i < lines.Count(); i++)
            {
                //Last line exit loop
                if (string.IsNullOrWhiteSpace(lines[i]))
                    break;

                //split with ,(comma)
                string[] deviceData = lines[i].Split(',');

                string Brand = deviceData[0];
                string Name = deviceData[1];
                string Device = deviceData[2];
                string Model = deviceData[3];

                //Append To List
                deviceJSON.Add(new DeviceModel
                {
                    brand = Brand,
                    name = Name,
                    device = Device,
                    model = Model
                });

                //Check Brand exists
                if (!string.IsNullOrWhiteSpace(Brand) && !brandJSON.Any(a => a == Brand))
                {
                    //Does not exists Append Data
                    brandJSON.Add(Brand);
                }

                Console.WriteLine(string.Join("\t", deviceData));
            }

            //Convert json to string And write to file
            File.WriteAllText(Environment.CurrentDirectory + @"\Device.json", JsonConvert.SerializeObject(deviceJSON));
            File.WriteAllText(Environment.CurrentDirectory + @"\Brand.json", JsonConvert.SerializeObject(brandJSON));

            Console.WriteLine();
            Console.WriteLine("Complate... Press any key to exit");
            Console.ReadLine();
        }

        /// <summary>
        /// Download CSV File
        /// </summary>
        /// <returns></returns>
        static string DownloadDeviceCSV()
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString("https://storage.googleapis.com/play_public/supported_devices.csv");
            }
        }
    }
}
