using System;
using System.IO;
using System.Runtime.Serialization;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Niam.XRM.ECGenerator.Core.Tests
{
    public static class TestUtils
    {
        public static void WriteToDataContractFileZip<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (var fileStream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            using (var zipStream = new ZipOutputStream(fileStream))
            using (var memoryStream = new MemoryStream())
            {
                zipStream.SetLevel(9); //0-9, 9 being the highest level of compression

                var newEntry = new ZipEntry("item")
                {
                    DateTime = DateTime.Now
                };

                zipStream.PutNextEntry(newEntry);

                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, objectToWrite);
                memoryStream.Position = 0;

                StreamUtils.Copy(memoryStream, zipStream, new byte[4096]);
                zipStream.CloseEntry();

                zipStream.IsStreamOwner = false;
                zipStream.Close();

                memoryStream.Position = 0;
            }
        }

        public static T ReadFromDataContractFileZip<T>(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            using (var zipFile = new ZipFile(fileStream))
            {
                var index = zipFile.FindEntry("item", false);
                var itemStream = zipFile.GetInputStream(index);
                var serializer = new DataContractSerializer(typeof(T));
                var result = (T)serializer.ReadObject(itemStream);
                zipFile.IsStreamOwner = true;
                zipFile.Close();
                return result;
            }
        }
    }
}
