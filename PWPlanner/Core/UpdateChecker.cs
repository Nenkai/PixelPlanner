using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Windows;
using System.Threading;

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

        /// <summary>
        /// Check for updates by checking github.
        /// </summary>
        /// <param name="latest"></param>
        /// <returns>Returns true if there is a new update.</returns>
        public static bool CheckForUpdates()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    using (MemoryStream downloaded = new MemoryStream(client.DownloadData(URL)))
                    {
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
                catch (ThreadAbortException)
                {
                    return false;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not check for updates\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
                
        }

        /// <summary>
        /// Get the "version" property from an XML Stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string GetVersionFromXml(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return xml.SelectSingleNode("data/version").InnerText;
        }

        /// <summary>
        /// Get the "changelog" property from an XML Stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string GetChangelogFromXml(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            XmlDocument xml = new XmlDocument();
            xml.Load(reader);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return xml.SelectSingleNode("data/changelog").InnerText;
        }
    }
}
