// /***********************************************************************
// * Copyright (C) 2013 中商网络(CCN)有限公司 版权所有
//
// *架构层次：KMSLotterySystemFront.Common 
// *文件名称：DataCache
// *文件说明：
// *创建人员：jinzhixin
// *联系方式：jinzhixin@yesno.com.cn
// *创建时间：2013-7-29 9:53:34  
//
// *创建标识：
// *创建描述：缓存数据
//
// *修改信息：
// *修改备注：
// ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;
using KMSLotterySystemFront.Model;
using System.Data;

namespace KMSLotterySystemFront.Common
{
    public class DataCache
    {
        #region 1) 获取当前应用程序指定CacheKey的Cache值
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }
        #endregion

        #region 2) 设置当前应用程序指定CacheKey的Cache值
        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }
        #endregion

        #region 3) 设置当前应用程序指定CacheKey的Cache值

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }
        #endregion

        #region 4) 清除所有的缓存
        /// <summary>
        /// 清除所有的缓存
        /// </summary>
        public static void ClearCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;

            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            ArrayList al = new ArrayList();
            while (CacheEnum.MoveNext())
            {
                al.Add(CacheEnum.Key);
            }
            foreach (string key in al)
            {
                _cache.Remove(key);
            }

        }
        #endregion
    }

    public class BaseDataCache
    {
        //所有有效缓存Table数据   key:facid+table+key
        private static Dictionary<string, DataTable> AllTable = new Dictionary<string, DataTable>();
        //所有有效缓存string数据       key:facid+key
        private static Dictionary<string, string> AllString = new Dictionary<string, string>();

        private static Dictionary<string, ShakeBaseData> AllBaseData = new Dictionary<string, ShakeBaseData>();

        /// <summary>
        /// 设置Table缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DataTable GetTable(string key)
        {
            if (AllTable != null)
            {
                if (AllTable.Count > 0)
                {
                    if (AllTable.Keys.Contains(key))
                    {
                        return AllTable[key];
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// 设置String缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            if (AllString != null)
            {
                if (AllString.Count > 0)
                {
                    if (AllString.Keys.Contains(key))
                    {
                        return AllString[key];
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 设置BaseData缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ShakeBaseData GetBaseData(string key)
        {
            if (AllBaseData != null)
            {
                if (AllBaseData.Count > 0)
                {
                    if (AllBaseData.Keys.Contains(key))
                    {
                        return AllBaseData[key];
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取Table缓存
        /// </summary>
        /// <param name="procutData"></param>
        /// <param name="key"></param>
        public static void SetTable(DataTable procutData,string key)
        {
            if (!AllTable.ContainsKey(key))
            {
                AllTable.Add(key, procutData);
            }
        }
        /// <summary>
        /// 获取String缓存
        /// </summary>
        /// <param name="strval"></param>
        /// <param name="key"></param>
        public static void SetString(string strval, string key)
        {
            if (!AllString.Keys.Contains(key))
            {
                AllString.Add(key, strval);
            }
        }

        /// <summary>
        /// 获取BaseData缓存
        /// </summary>
        /// <param name="strval"></param>
        /// <param name="key"></param>
        public static void SetString(ShakeBaseData basedata, string key)
        {
            if (!AllBaseData.Keys.Contains(key))
            {
                AllBaseData.Add(key, basedata);
            }
        }


         /// <summary>
         /// 删除Table缓存
         /// </summary>
         /// <param name="key"></param>
        public static void DelTable(string key)
        {
            if (AllTable.ContainsKey(key))
            {
                AllTable.Remove(key);
            }
        }
       /// <summary>
       /// 删除String缓存
       /// </summary>
       /// <param name="key"></param>
        public static void DelString(string key)
        {
            if (AllString.ContainsKey(key))
            {
                AllString.Remove(key);
            }
        }

        /// <summary>
        /// 删除BaseData缓存
        /// </summary>
        /// <param name="key"></param>
        public static void DelBaseData(string key)
        {
            if (AllBaseData.ContainsKey(key))
            {
                AllBaseData.Remove(key);
            }
        }
    }
}
