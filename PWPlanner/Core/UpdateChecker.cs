using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Net;
using System.Xml;
using System.IO;


namespace PWPlanner
{
    public class UpdateChecker
    {
        public static string current;
        public static string latest;

        public static readonly string URL = "https://raw.githubusercontent.com/Nenkai/PixelPlanner/master/PWPlanner/config.xml";

        public static bool CheckForUpdates()
        {

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PWPlanner.config.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                current = GetVersionFromXml(stream);
            }

            WebClient client = new WebClient();
            Stream downloaded = client.OpenRead(URL);
            latest = GetVersionFromXml(downloaded);

            var v1 = Version.Parse(latest);
            var v2 = Version.Parse(current);

            var result = v1.CompareTo(v2);

            if (result > 0)
            {
                return true;
            }
            return false;
        }

        private static string GetVersionFromXml(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(reader);
                XmlNodeList nodelist = xml.SelectNodes("/data");
                return nodelist[0].InnerText;
            }
        }
    }
}
