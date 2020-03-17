using System;
using System.IO;
using System.Text;
using System.Xml;

namespace cfgConfig.Core.old.Serialization
{
    internal static class XmlSerializer
    {
        public static T Deserialize<T>(string content)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                return (T)serializer.Deserialize(ms);
        }

        public static object Deserialize(string content, Type type)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(type);

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                return serializer.Deserialize(ms);
        }

        public static string Serialize(object obj)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(object));

            using(var sw = new StringWriter())
            using(XmlWriter xml = XmlWriter.Create(sw))
            {
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }
    }
}
