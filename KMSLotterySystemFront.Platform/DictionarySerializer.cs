using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace KMSLotterySystemFront.Platform
{
    public sealed class DictionarySerializer
    {
        /// <summary>
        /// 将字典序列化为字节数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Serialize(Dictionary<string, object> data) { 
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, data);
            return ms.GetBuffer();
        }

        /// <summary>
        /// 将字节数组反序列化为字典
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, object> Deserialize(byte[] data) {
            Dictionary<string, object> result  = new Dictionary<string,object>();
            try
            {
                MemoryStream ms = new MemoryStream(data);
                BinaryFormatter bf = new BinaryFormatter();
                result = bf.Deserialize(ms) as Dictionary<string, object>;
            }
            catch (Exception error) {
                Debug.WriteLine(string.Format("DictionarySerializer Deserialize failed, detail is \r\n {0}", error));
            }
            return result;
        }

    }
}
