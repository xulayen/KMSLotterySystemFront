using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMSLotterySystemFront.Model
{
    [Serializable]
    public class PostAddEntity
    {
        #region 字段
        private String _postguid = String.Empty;// 邮寄GUID
        private String _userguid = String.Empty;// 用户GUID
        private String _usertype = String.Empty;// 用户类型
        private String _provinceid = String.Empty;// 省份编号（收货人所在省份）
        private String _cityid = String.Empty;// 城市编号（收货人所在城市）
        private String _countyid = String.Empty;// 区县编号（收货人所在区县）
        private String _zip = String.Empty;// 邮政编码（收货人的邮政编码）
        private String _postname = String.Empty;// 收货人姓名
        private String _mobile = String.Empty;// 收货人手机号
        private String _telphone = String.Empty;// 收货人固定电话
        private String _postaddr = String.Empty;// 街道地址
        private String _defaultstate = String.Empty;// 是否为默认（0：不是默认地址 1：默认收货地址）
        private DateTime _createdate = DateTime.Now;// 创建时间
        private String _deleteflag = String.Empty;// 数据状态(0:无效 1：正常使用)
        private String _cardid = String.Empty;
        #endregion

        #region 属性

        /// <summary>
        /// 邮寄GUID
        /// </summary>
        public String Postguid
        {
            get
            {
                return _postguid;
            }
            set
            {
                _postguid = value;
            }
        }

        /// <summary>
        /// 用户GUID
        /// </summary>
        public String Userguid
        {
            get
            {
                return _userguid;
            }
            set
            {
                _userguid = value;
            }
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        public String Usertype
        {
            get
            {
                return _usertype;
            }
            set
            {
                _usertype = value;
            }
        }

        /// <summary>
        /// 省份编号（收货人所在省份）
        /// </summary>
        public String Provinceid
        {
            get
            {
                return _provinceid;
            }
            set
            {
                _provinceid = value;
            }
        }

        /// <summary>
        /// 城市编号（收货人所在城市）
        /// </summary>
        public String Cityid
        {
            get
            {
                return _cityid;
            }
            set
            {
                _cityid = value;
            }
        }

        /// <summary>
        /// 区县编号（收货人所在区县）
        /// </summary>
        public String Countyid
        {
            get
            {
                return _countyid;
            }
            set
            {
                _countyid = value;
            }
        }

        /// <summary>
        /// 邮政编码（收货人的邮政编码）
        /// </summary>
        public String Zip
        {
            get
            {
                return _zip;
            }
            set
            {
                _zip = value;
            }
        }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public String Postname
        {
            get
            {
                return _postname;
            }
            set
            {
                _postname = value;
            }
        }

        /// <summary>
        /// 收货人手机号
        /// </summary>
        public String Mobile
        {
            get
            {
                return _mobile;
            }
            set
            {
                _mobile = value;
            }
        }

        /// <summary>
        /// 收货人固定电话
        /// </summary>
        public String Telphone
        {
            get
            {
                return _telphone;
            }
            set
            {
                _telphone = value;
            }
        }

        /// <summary>
        /// 街道地址
        /// </summary>
        public String Postaddr
        {
            get
            {
                return _postaddr;
            }
            set
            {
                _postaddr = value;
            }
        }

        /// <summary>
        /// 是否为默认（0：不是默认地址 1：默认收货地址）
        /// </summary>
        public String Defaultstate
        {
            get
            {
                return _defaultstate;
            }
            set
            {
                _defaultstate = value;
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Createdate
        {
            get
            {
                return _createdate;
            }
            set
            {
                _createdate = value;
            }
        }

        /// <summary>
        /// 数据状态(0:无效 1：正常使用)
        /// </summary>
        public String Deleteflag
        {
            get
            {
                return _deleteflag;
            }
            set
            {
                _deleteflag = value;
            }
        }
    
        #endregion
        ///<summary>
        ///无参构造函数
        ///<summary>
        public PostAddEntity() { }
        ///<summary>
        /// 全参构造
        ///<summary>
        public PostAddEntity(
          String _postguid,
          String _userguid,
          String _usertype,
          String _provinceid,
          String _cityid,
          String _countyid,
          String _zip,
          String _postname,
          String _mobile,
          String _telphone,
          String _postaddr,
          String _defaultstate,
          DateTime _createdate,
          String _deleteflag)
        {
            this.Postguid = _postguid;
            this.Userguid = _userguid;
            this.Usertype = _usertype;
            this.Provinceid = _provinceid;
            this.Cityid = _cityid;
            this.Countyid = _countyid;
            this.Zip = _zip;
            this.Postname = _postname;
            this.Mobile = _mobile;
            this.Telphone = _telphone;
            this.Postaddr = _postaddr;
            this.Defaultstate = _defaultstate;
            this.Createdate = _createdate;
            this.Deleteflag = _deleteflag;
        }
    }
}
