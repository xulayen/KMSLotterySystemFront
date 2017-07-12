using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Collections;

namespace KMSLotterySystemFront.DBUtility
{
    /// <summary>
    /// 该类用于在缓存中读取或存入SQL语句参数对象的实例
    /// </summary>
    public sealed class ParameterCache
    {
        //定义HashTable对象，来保存SQL语句参数对象的实例
        private static Hashtable CacheParms = new Hashtable();

        /// <summary>
        /// 根据键值取出从缓存中的HashTable中的SQL语句参数对象的实例
        /// </summary>
        /// <param name="key">唯一的键值</param>
        /// <returns>SQL语句参数对象的实例</returns>
        public static IDataParameter[] GetParams(string key)
        {
            try
            {
                return (IDataParameter[])CacheParms[key];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 把SQL语句参数对象的实例存入缓存中的HashTable中
        /// </summary>
        /// <param name="key">唯一键值</param>
        /// <param name="parms">唯一键值对应的值</param>
        public static void PushCache(string key, IDataParameter[] parms)
        {
            ////防止重复的键加入
            //if (!CacheParms.ContainsKey(key))
            //{
            //    CacheParms.Add(key, parms);
            //}
        }
    }
}
