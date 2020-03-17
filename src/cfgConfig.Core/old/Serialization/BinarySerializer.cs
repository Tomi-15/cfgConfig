using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace cfgConfig.Core.old.Serialization
{
    internal static class BinarySerializer
    {
        public static T Deserialize<T>(string input)
        {
            byte[] b = Convert.FromBase64String(input);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static object Deserialize(string input)
        {
            byte[] b = Convert.FromBase64String(input);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }

        public static string Serialize<T>(T settings)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }
}
