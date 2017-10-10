using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Niam.XRM.ECGenerator.Core
{
    public static class JsonSerializer
    {
        public static T Deserialize<T>(string json)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var reader = new MemoryStream(bytes))
            {
                return (T) serializer.ReadObject(reader);
            }
        }

        public static string Serialize<T>(T graph)
        {
            using (var memStream = new MemoryStream())
            using (var reader = new StreamReader(memStream))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memStream, graph);
                memStream.Position = 0;
                return reader.ReadToEnd();
            }
        }
    }
}
