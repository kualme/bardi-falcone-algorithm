using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BardiFalcone
{
    public static class Helpers
    {
        public static void Serialize(string folder, DataToSerialize data, int iter)
        {
            FileStream fileStream = new FileStream(Path.Combine(folder, String.Format("data_{0}.dat", iter)), FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fileStream, data);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fileStream.Close();
            }
        }

        public static DataToSerialize Deserialize(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            DataToSerialize result = new DataToSerialize();
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                result = (DataToSerialize)formatter.Deserialize(fileStream);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fileStream.Close();
            }
            return result;
        }
    }
}
