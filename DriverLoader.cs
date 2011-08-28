using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace GTGHelper
{
    static class DriverLoader
    {
        // Source list of names
        private static string[] fullDrivers = new string[] 
        {
            // retired drivers
//            "Karun Chandhok",
//            "Bruno Senna",
//            "Pedro de la Rosa",
//            "Lucas di Grassi",
//            "Nico Hulkenberg",
//            "Robert Kubica",
            "Jenson Button",
            "Lewis Hamilton",
            "Michael Schumacher",
            "Nico Rosberg",
            "Sebastian Vettel",
            "Mark Webber",
            "Felipe Massa",
            "Fernando Alonso",
            "Rubens Barrichello",
            "Vitaly Petrov",
            "Adrian Sutil",
            "Vitantonio Liuzzi",
            "Sebastien Buemi",
            "Jaime Alguersuari",
            "Jarno Trulli",
            "Heikki Kovalainen",
            "Kamui Kobayashi",
            "Timo Glock",
            "Nick Heidfeld",

            // new drivers
            "Pastor Maldonado",
            "Sergio Perez",
            "Narain Karthikeyan",
            "Jerome d'Ambrosio",
            "Paul di Resta"
        };

        private static List<Racer> InitDriverList()
        {
            List<Racer> Racers = new List<Racer>();
            // Put racers in the racers list.
            for (int i = 0; i < fullDrivers.Length; i++)
            {
                string driver = fullDrivers[i];
                Racer r = new Racer();
                r.SetName(driver);

                // Handle obvious cases of alternative spellings
                switch (r.Name)
                {
                    case "Felipe Massa":
                        r.Alternatives.Add("MAS");
                        break;
                    //case "Robert Kubica":
                    //    r.Alternatives.Add("KUB");
                    //    break;
                    case "Fernando Alonso":
                        r.Alternatives.Add("ALO");
                        break;
                    case "Michael Schumacher":
                        r.Alternatives.Add("Schumi");
                        r.Alternatives.Add("Shumi");
                        r.Alternatives.Add("Shumacher");
                        r.Alternatives.Add("MSC");
                        r.Alternatives.Add("SCH");
                        r.Alternatives.Add("Schu");
                        break;
                    case "Jenson Button":
                        r.Alternatives.Add("Buttons");
                        r.Alternatives.Add("BUT");
                        break;
                    case "Mark Webber":
                        r.Alternatives.Add("Weber");
                        r.Alternatives.Add("WEB");
                        r.Alternatives.Add("Webbaarrrrrr");
                        break;
                    case "Nico Rosberg":
                        r.Alternatives.Add("Roseberg");
                        r.Alternatives.Add("Rosbeg");
                        r.Alternatives.Add("ROS");
                        r.Alternatives.Add("Rosburg");
                        r.Alternatives.Add("Rosbery");
                        r.Alternatives.Add("Rosberd");
                        r.Alternatives.Add("Rossberg");
                        break;
                    case "Rubens Barrichello":
                        r.Alternatives.Add("Barichello");
                        r.Alternatives.Add("Barrichelo");
                        r.Alternatives.Add("BAR");
                        r.Alternatives.Add("Barachello");
                        r.Alternatives.Add("Barrachello");
                        r.Alternatives.Add("Barricello");
                        break;
                    case "Lewis Hamilton":
                        r.Alternatives.Add("Hammilton");
                        r.Alternatives.Add("HAM");
                        r.Alternatives.Add("hammi");
                        break;
                    case "Kamui Kobayashi":
                        r.Alternatives.Add("Kobyashi");
                        r.Alternatives.Add("Koboyashi");
                        r.Alternatives.Add("Kobiyashi");
                        r.Alternatives.Add("Kobash!");
                        r.Alternatives.Add("Kobashi");

                        break;
                    case "Sebastian Vettel":
                        r.Alternatives.Add("Vettle");
                        r.Alternatives.Add("VET");
                        r.Alternatives.Add("Vettl");
                        break;
                    //case "Nico Hulkenberg":
                    //    r.Alternatives.Add("hulkenburg");
                    //    r.Alternatives.Add("hülkenberg");
                    //    r.Alternatives.Add("Hulk");
                    //    break;
                    case "Adrian Sutil":
                        r.Alternatives.Add("SUT");
                        break;
                    case "Jaime Alguersuari":
                        r.Alternatives.Add("Algesuari");
                        r.Alternatives.Add("alguseari");
                        break;
                    case "Vitantonio Liuzzi":
                        r.Alternatives.Add("Luizzi");
                        break;
                    case "Nick Heidfeld":
                        r.Alternatives.Add("Hiedfeld");
                        break;
                }

                Racers.Add(r);
            }
            return Racers;
        }

        public static List<Racer> GetRacers()
        {
            // Try to load from config
            List<Racer> racers = LoadDriverList();
            if (racers == null)
            {
                // if config path failed, create a new default list
                racers = InitDriverList();
            }

            return racers;
        }

        
        // try to load from config file
        private static List<Racer> LoadDriverList()
        {
            try
            {
                if (Properties.Settings.Default.drivers.Length > 0)
                {
                    List<Racer> racers = new List<Racer>();
                    // parse serialized racer list
                    string driverxml = Properties.Settings.Default.drivers;
                    XmlSerializer serializer = new XmlSerializer(racers.GetType());
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(driverxml));

                    using (XmlReader reader = XmlReader.Create(ms))
                    {
                        reader.Read();
                        reader.ReadStartElement("drivers");
                        racers = (List<Racer>)serializer.Deserialize(reader);
                        reader.ReadEndElement();
                    }

                    if (racers.Count > 0)
                    {
                        Hook.WriteLine("Loaded driverlist");
                        return racers;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return null;
        }

        public static void SaveDriverList(List<Racer> Racers)
        {
            XmlSerializer serializer = new XmlSerializer(Racers.GetType());
            StringBuilder strBuilder = new StringBuilder();
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
            xmlSettings.OmitXmlDeclaration = true;

            using (XmlWriter writer = XmlWriter.Create(strBuilder, xmlSettings))
            {
                writer.WriteStartElement("drivers");
                serializer.Serialize(writer, Racers);
                writer.WriteEndElement();
                writer.Flush();
            }

            Properties.Settings.Default.drivers = strBuilder.ToString();
            Properties.Settings.Default.Save();
            Console.WriteLine("XML size: " + strBuilder.Length);
        }
    }
}
