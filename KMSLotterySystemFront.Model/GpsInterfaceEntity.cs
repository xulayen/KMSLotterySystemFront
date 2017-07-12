using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    [Serializable]
    public class GpsInterfaceEntity
    {
        public GpsInterfaceEntity()
        {

            IsSuccess = string.Empty;
            ErrCode = string.Empty;
            ErrMessage = string.Empty;
            AcitivityId = string.Empty;
            ResponseData ResponseData = new ResponseData();
        }

        /// <summary>
        /// 返回状态
        /// </summary>
        public string IsSuccess { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 消息结果
        /// </summary>
        public string ErrMessage { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        public string AcitivityId { get; set; }

        /// <summary>
        /// GPS返回信息实体
        /// </summary>
        public ResponseData ResponseData { get; set; }
    }

    [Serializable]
    public class ResponseData
    {
        #region 构造函数
        public ResponseData()
        {
            CountryCode = string.Empty;
            CountryName = string.Empty;
            ProvinceCode = string.Empty;
            ProvinceName = string.Empty;
            CityCode = string.Empty;
            CityName = string.Empty;
            DistrictCode = string.Empty;
            DistrictName = string.Empty;
            Address = string.Empty;
            Longitude = string.Empty;
            Latitude = string.Empty;
        }
        #endregion

        /// <summary>
        /// 国家代码
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// 省份代码
        /// </summary>
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市代码
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 区/县代码
        /// </summary>
        public string DistrictCode { get; set; }

        /// <summary>
        /// 区/县名称
        /// </summary>
        public string DistrictName { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }


    }
}
