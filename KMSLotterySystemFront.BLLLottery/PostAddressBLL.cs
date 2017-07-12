using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMSLotterySystemFront.Model;
using KMSLotterySystemFront.DAL;

namespace KMSLotterySystemFront.BLLLottery
{
    public static class PostAddressBLL
    {
        private static PostAddressDAL postadd = new PostAddressDAL();
        #region 添加地址管理
        /// <summary>
        /// 得到用户所有的地址管理
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static bool AddPostAdd(string facid, PostAddEntity info)
        {
            return postadd.AddPostAdd(facid, info);
        }
        #endregion
    }
}
