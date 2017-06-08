using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;


namespace PWPlanner
{
    public class UpdateChecker
    {
        public static string latest;
        public static string changelog;
        public static readonly string URL = "https://raw.githubusercontent.com/Nenkai/PixelPlanner/master/PWPlanner/config.xml";

        public static string current
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PWPlanner.config.xml";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    return GetVersionFromXml(stream);
                }
            }
        }

        public static bool CheckForUpdates()
        {
            try
            {

                using (WebClient client = new WebClient())
                {
                    Stream downloaded = client.OpenRead(URL);
                    latest = GetVersionFromXml(downloaded);
                    changelog = GetChangelogFromXml(downloaded);
                    downloaded.Dispose();

                    var v1 = Version.Parse(latest);
                    var v2 = Version.Parse(current);
                    var result = v1.CompareTo(v2);
                    
                    if (result > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }


        private static string GetVersionFromXml(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(reader);
                return xml.SelectSingleNode("data/version").InnerText;
            }
        }

        private static string GetChangelogFromXml(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(reader);
                return xml.SelectSingleNode("data/changelog").InnerText;
            }
        }
    }
}
