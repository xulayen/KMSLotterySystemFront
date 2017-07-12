﻿/**
* Created by xulayen on 2016/9/14.
*/
;
(function ($, WX) {
    var WXAPIConfig = {
        debug: false,
        baseapi_checkJsApi: true,
        baseapi_onMenuShareTimeline: true,
        baseapi_onMenuShareAppMessage: true,
        baseapi_onMenuShareQQ: true,
        baseapi_onMenuShareWeibo: true,
        baseapi_hideMenuItems: true,
        baseapi_showMenuItems: true,
        baseapi_hideAllNonBaseMenuItem: true,
        baseapi_showAllNonBaseMenuItem: true,
        baseapi_hideOptionMenu: true,
        baseapi_showOptionMenu: true,
        baseapi_closeWindow: true,
        baseapi_scanQRCode: true,
        baseapi_startRecord: true,
        baseapi_stopRecord: true,
        baseapi_onVoiceRecordEnd: true,
        baseapi_playVoice: true,
        baseapi_pauseVoice: true,
        baseapi_stopVoice: true,
        baseapi_onVoicePlayEnd: true,
        baseapi_uploadVoice: true,
        baseapi_downloadVoice: true,
        baseapi_chooseImage: true,
        baseapi_previewImage: true,
        baseapi_uploadImage: true,
        baseapi_downloadImage: true,
        baseapi_translateVoice: true,
        baseapi_getNetworkType: true,
        baseapi_openLocation: true,
        baseapi_getLocation: true,
        baseapi_chooseWXPay: true,
        baseapi_openProductSpecificView: true,
        baseapi_addCard: true,
        baseapi_chooseCard: true,
        baseapi_openCard: true,
        appId: null,
        timestamp: null,
        nonceStr: null,
        signature: null,
        access_token: null,
        分享到朋友圈: false, //'menuItem:share:timeline'
        发送给朋友: false, //, 'menuItem:share:appMessage',
        收藏: false, //, 'menuItem:favorite',
        在Safari中打开: false, //,'menuItem:openWithSafari',
        邮件: false, //,'menuItem:share:email',
        分享到QQ: false, //,'menuItem:share:qq',
        分享到QQ空间: false, //, 'menuItem:share:QZone',
        分享到Weibo: false, //, 'menuItem:share:weiboApp',
        复制链接: false, //, 'menuItem:copyUrl',
        调整字体: false, //, 'menuItem:setFont',
        阅读模式: false, //,'menuItem:readMode',
        刷新: false, //,'menuItem:refresh',
        api: '',
        facid: '00446',
        typenum: '1',
        scanAuthUrl: location.href,
        hideOptionMenu: true,
        async: true, //是否异步
        type: 'POST',
        ContentType: 'application/x-www-form-urlencoded',
        cache: true
    }, opts = {};
    $.fn.WeChart = $.WeChart = WeChart;
    /**
    * 帮助类
    * @param a
    * @param b
    * @returns {d.init}
    */
    var d = function () {
        return new d.prototype.init();
    };
    d.prototype = {
        init: function () {
            return this;
        },
        error: function (a) {
            throw a;
        },
        lookDebug: function (a) {
            if (opts.debug) {
                alert(a);
                console.log(a);
            }
        },
        type: function (a) {
            return Object.prototype.toString.call(a);
        },
        SetWeChat: function (result) {
            var _r = JSON.stringify(result);
            d.lookDebug('Ajax success:' + _r);
            opts.appId = result.APPID;
            opts.timestamp = result.TIMESTAMP;
            opts.nonceStr = result.NONCESTR;
            opts.signature = result.SIGNATURE;
            opts.access_token = result.ACCESS_TOKEN;
            localStorage.setItem('WeChatData', _r);
        },
        GetWeChat: function (success, error) {
            $.ajax({
                type: opts.type,
                url: opts.api,
                async: opts.async,
                ContentType: opts.ContentType,
                cache: opts.cache,
                data: {'url': opts.scanAuthUrl, 'typenum': opts.typenum, 'facid': opts.facid},
                success: function (result) {
                    d.SetWeChat(result);
                    success && success.call(opts, result);
                },
                error: function (e) {
                    d.error('NetWork is busy!');
                    d.lookDebug('Ajax error:' + JSON.stringify(e));
                    error && error.call(opts, e);
                }
            });
        }
    };
    d.prototype.init.prototype = d.prototype;
    d = d();

    function WeChart(options) {
        var _self = this;
        opts = $.extend({}, WXAPIConfig, options);
        return _self;
    };

    /**
    * 微信初始化
    * @type {Function}
    */
    $.fn.InitWeChat = $.InitWeChat = function (config, success, error) {
        var _self = this;
        opts = $.extend({}, opts, config);
        try {
            if (opts.api) {
                var _r_local = localStorage.getItem('WeChatData');
                if (_r_local) {
                    var result = JSON.parse(_r_local);
                    d.SetWeChat(result);
                    opts.async = true;//异步
                    d.GetWeChat(success, error);
                } else {
                    opts.async = false;//同步
                    d.GetWeChat(success, error);
                }
            } else {
                d.error('请先初始化微信api');
            }
            ;

            var jsApi = [], menuList = [];
            if (opts.baseapi_checkJsApi) {
                jsApi.push('checkJsApi');
            }
            if (opts.baseapi_onMenuShareTimeline) {
                jsApi.push('onMenuShareTimeline');
            }
            if (opts.baseapi_onMenuShareAppMessage) {
                jsApi.push('onMenuShareAppMessage');
            }
            if (opts.baseapi_onMenuShareQQ) {
                jsApi.push('onMenuShareQQ');
            }
            if (opts.baseapi_onMenuShareWeibo) {
                jsApi.push('onMenuShareWeibo');
            }
            if (opts.baseapi_hideMenuItems) {
                jsApi.push('hideMenuItems');
            }
            if (opts.baseapi_showMenuItems) {
                jsApi.push('showMenuItems');
            }
            if (opts.baseapi_hideAllNonBaseMenuItem) {
                jsApi.push('hideAllNonBaseMenuItem');
            }
            if (opts.baseapi_showAllNonBaseMenuItem) {
                jsApi.push('showAllNonBaseMenuItem');
            }
            if (opts.baseapi_hideOptionMenu) {
                jsApi.push('hideOptionMenu');
            }
            if (opts.baseapi_showOptionMenu) {
                jsApi.push('showOptionMenu');
            }
            if (opts.baseapi_closeWindow) {
                jsApi.push('closeWindow');
            }
            if (opts.baseapi_startRecord) {
                jsApi.push('startRecord');
            }
            if (opts.baseapi_stopRecord) {
                jsApi.push('stopRecord');
            }
            if (opts.baseapi_onVoiceRecordEnd) {
                jsApi.push('onVoiceRecordEnd');
            }
            if (opts.baseapi_playVoice) {
                jsApi.push('playVoice');
            }
            if (opts.baseapi_pauseVoice) {
                jsApi.push('pauseVoice');
            }
            if (opts.baseapi_stopVoice) {
                jsApi.push('stopVoice');
            }
            if (opts.baseapi_onVoicePlayEnd) {
                jsApi.push('onVoicePlayEnd');
            }
            if (opts.baseapi_uploadVoice) {
                jsApi.push('uploadVoice');
            }
            if (opts.baseapi_downloadVoice) {
                jsApi.push('downloadVoice');
            }
            if (opts.baseapi_chooseImage) {
                jsApi.push('chooseImage');
            }
            if (opts.baseapi_previewImage) {
                jsApi.push('previewImage');
            }
            if (opts.baseapi_uploadImage) {
                jsApi.push('uploadImage');
            }
            if (opts.baseapi_downloadImage) {
                jsApi.push('downloadImage');
            }
            if (opts.baseapi_translateVoice) {
                jsApi.push('translateVoice');
            }
            if (opts.baseapi_getNetworkType) {
                jsApi.push('getNetworkType');
            }
            if (opts.baseapi_openLocation) {
                jsApi.push('openLocation');
            }
            if (opts.baseapi_getLocation) {
                jsApi.push('getLocation');
            }
            if (opts.baseapi_chooseWXPay) {
                jsApi.push('chooseWXPay');
            }
            if (opts.baseapi_openProductSpecificView) {
                jsApi.push('openProductSpecificView');
            }
            if (opts.baseapi_addCard) {
                jsApi.push('addCard');
            }
            if (opts.baseapi_chooseCard) {
                jsApi.push('chooseCard');
            }
            if (opts.baseapi_openCard) {
                jsApi.push('openCard');
            }
            if (opts.baseapi_scanQRCode) {
                jsApi.push('scanQRCode');
            }
            if (opts.分享到朋友圈) {
                menuList.push('menuItem:share:timeline');
            }
            if (opts.发送给朋友) {
                menuList.push('menuItem:share:appMessage');
            }
            if (opts.收藏) {
                menuList.push('menuItem:favorite');
            }
            if (opts.在Safari中打开) {
                menuList.push('menuItem:openWithSafari');
            }
            if (opts.邮件) {
                menuList.push('menuItem:share:email');
            }
            if (opts.分享到QQ) {
                menuList.push('menuItem:share:qq');
            }
            if (opts.分享到QQ空间) {
                menuList.push('menuItem:share:QZone');
            }
            if (opts.分享到Weibo) {
                menuList.push('menuItem:share:weiboApp');
            }
            if (opts.复制链接) {
                menuList.push('menuItem:copyUrl');
            }
            if (opts.调整字体) {
                menuList.push('menuItem:setFont');
            }
            if (opts.阅读模式) {
                menuList.push('menuItem:readMode');
            }
            if (opts.刷新) {
                menuList.push('menuItem:refresh');
            }
            WX.config({
                debug: opts.debug,
                appId: opts.appId,
                timestamp: opts.timestamp,
                nonceStr: opts.nonceStr,
                signature: opts.signature,
                jsApiList: jsApi
            });
            WX.ready(function () {
                if (opts.hideOptionMenu) {
                    WX.hideOptionMenu();
                }
                WX.showMenuItems({ menuList: menuList });
            });
            return _self;
        } catch (e) {
            d.error('config.wxInit error' + e.message);
            d.lookDebug('wxInit error:' + e.message);
        }
    };

    /**
    @param success 扫描成功回调函数 {Function}
    */
    $.fn.Scan = $.Scan = function (success) {
        var _self = this;
        try {
            WX.scanQRCode({
                needResult: 1, //
                success: function (res) {
                    var result = res.resultStr; //当needResult 为 1 时，扫码返回的结果
                    success && success.call(_self, result);
                }
            });
        } catch (e) {
            d.error('Scan error' + e.message);
            d.lookDebug('Scan error:' + e.message);
        }
        return _self;
    };

    /**
    * 分享到朋友圈
    @param forword 转发对象
    @param success 转发成功回调函数 {Function}
    @param cancel  取消转发回调函数 {Function}
    */
    $.fn.Forword = $.Forword = function (forword, success, cancel) {
        var _self = this;
        try {
            WX.ready(function () {
                WX.onMenuShareTimeline({
                    title: forword.forword_title, // 分享标题
                    desc: forword.forword_desc, // 分享描述
                    link: forword.forword_link, // 分享链接
                    imgUrl: forword.forword_imgUrl, // 分享图标
                    type: forword.forword_type, // 分享类型,music、video或link，不填默认为link
                    dataUrl: forword.forword_dataUrl, // 如果type是music或video，则要提供数据链接，默认为空
                    success: function (res) {
                        success && success.call(_self, res);
                    },
                    cancel: function (res) {
                        /**
                        * res 回调对象
                        * forword 转发原始参数
                        */
                        cancel && cancel.call(_self, res, forword);
                    }
                });
            });
        } catch (e) {
            d.error('Forword error' + e.message);
            d.lookDebug('Forword error:' + e.message);
        }
        return _self;
    };


    /**
    * 分享给朋友
    * @param success
    * @param cancel
    * @returns {$.fn.ShareQQ}
    * @constructor
    */
    $.fn.ForwordToFriend = $.ForwordToFriend = function (forword, success, cancel) {
        var _self = this;
        try {
            WX.ready(function () {
                WX.onMenuShareAppMessage({
                    title: forword.forword_title, // 分享标题
                    desc: forword.forword_desc, // 分享描述
                    link: forword.forword_link, // 分享链接
                    imgUrl: forword.forword_imgUrl, // 分享图标
                    type: forword.forword_type, // 分享类型,music、video或link，不填默认为link
                    dataUrl: forword.forword_dataUrl, // 如果type是music或video，则要提供数据链接，默认为空
                    success: function (res) {
                        // 用户确认分享后执行的回调函数
                        success && success.call(_self, res, forword);
                    },
                    cancel: function (res) {
                        // 用户取消分享后执行的回调函数
                        cancel && cancel.call(_self, res, forword);
                    }
                });
            });
        } catch (e) {
            d.error('ForwordToFriend error' + e.message);
            d.lookDebug('ForwordToFriend error:' + e.message);
        }
        return _self;
    };


    /**
    * 分享到QQ
    * @param success
    * @param cancel
    * @returns {$.fn.ShareQQ}
    * @constructor
    */
    $.fn.ShareQQ = $.ShareQQ = function (forword, success, cancel) {
        var _self = this;
        try {
            WX.ready(function () {
                WX.onMenuShareQQ({
                    title: forword.forword_title, // 分享标题
                    desc: forword.forword_desc, // 分享描述
                    link: forword.forword_link, // 分享链接
                    imgUrl: forword.forword_imgUrl, // 分享图标
                    success: function (res) {
                        // 用户确认分享后执行的回调函数
                        success && success.call(_self, res, forword);
                    },
                    cancel: function (res) {
                        // 用户取消分享后执行的回调函数
                        cancel && cancel.call(_self, res, forword);
                    }
                });
            });
        } catch (e) {
            d.error('ShareQQ error' + e.message);
            d.lookDebug('ShareQQ error:' + e.message);
        }
        return _self;
    };


    /**
    * 分享到微博
    * @param success
    * @param cancel
    * @returns {$.fn.ShareWeibo}
    * @constructor
    */
    $.fn.ShareWeibo = $.ShareWeibo = function (forword, success, cancel) {
        var _self = this;
        try {
            WX.ready(function () {
                WX.onMenuShareWeibo({
                    title: forword.forword_title, // 分享标题
                    desc: forword.forword_desc, // 分享描述
                    link: forword.forword_link, // 分享链接
                    imgUrl: forword.forword_imgUrl, // 分享图标
                    success: function (res) {
                        // 用户确认分享后执行的回调函数
                        success && success.call(_self, res, forword);
                    },
                    cancel: function (res) {
                        // 用户取消分享后执行的回调函数
                        cancel && cancel.call(_self, res, forword);
                    }
                });
            });
        } catch (e) {
            d.error('ShareWeibo error' + e.message);
            d.lookDebug('ShareWeibo error:' + e.message);
        }
        return _self;
    };


    /**
    * 分享到QQ空间
    * @param success
    * @param cancel
    * @returns {$.fn.ShareQZone}
    * @constructor
    */
    $.fn.ShareQZone = $.ShareQZone = function (forword, success, cancel) {
        var _self = this;
        try {
            WX.ready(function () {
                WX.onMenuShareQZone({
                    title: forword.forword_title, // 分享标题
                    desc: forword.forword_desc, // 分享描述
                    link: forword.forword_link, // 分享链接
                    imgUrl: forword.forword_imgUrl, // 分享图标
                    success: function (res) {
                        // 用户确认分享后执行的回调函数
                        success && success.call(_self, res, forword);
                    },
                    cancel: function (res) {
                        // 用户取消分享后执行的回调函数
                        cancel && cancel.call(_self, res, forword);
                    }
                });
            });
        } catch (e) {
            d.error('ShareQZone error' + e.message);
            d.lookDebug('ShareQZone error:' + e.message);
        }
        return _self;
    };


    /**
    * 选择图片
    * @param success
    * @returns {$.fn.ChooseImg}
    * @constructor
    */
    $.fn.ChooseImg = $.ChooseImg = function (success) {
        var _self = this;
        try {
            WX.ready(function () {
                WX.chooseImage({
                    count: 9, // 默认9
                    sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
                    sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
                    success: function (res) {
                        var localIds = res.localIds; // 返回选定照片的本地ID列表，localId可以作为img标签的src属性显示图片
                        opts.imgLocalIds = localIds;
                        success && success.call(_self, res, localIds);
                    }
                });
            });
        } catch (e) {
            d.error('ChooseImg error' + e.message);
            d.lookDebug('ChooseImg error:' + e.message);
        }
        return _self;
    };

    /**
    * 预览图片
    * @returns {$.fn.PreviewImage}
    * @constructor
    */
    $.fn.PreviewImage = $.PreviewImage = function (previewCurrentImg, previewUrls) {
        var _self = this;
        if (Object.prototype.toString.call(previewUrls) !== "[object Array]") {
            d.error('PreviewImage error - previewUrls必须是Array类型');
        } else {
            try {
                WX.ready(function () {
                    WX.previewImage({
                        current: previewCurrentImg, // 当前显示图片的http链接
                        urls: previewUrls // 需要预览的图片http链接列表 []
                    });
                });
            } catch (e) {
                d.error('PreviewImage error' + e.message);
                d.lookDebug('PreviewImage error:' + e.message);
            }
        }
        return _self;
    };


    /**
    * 上传图片接口
    * @returns {$.fn.UploadImage}
    * @constructor
    */
    $.fn.UploadImage = $.UploadImage = function (imgLocalIds, success) {
        var _self = this;
        try {
            WX.ready(function () {
                WX.uploadImage({
                    localId: imgLocalIds, // 需要上传的图片的本地ID，由chooseImage接口获得
                    isShowProgressTips: 1, // 默认为1，显示进度提示
                    success: function (res) {
                        var serverId = res.serverId; // 返回图片的服务器端ID
                        success && success.call(_self, res, serverId);
                    }
                });
            });
        } catch (e) {
            d.error('UploadImage error' + e.message);
            d.lookDebug('UploadImage error:' + e.message);
        }
        return _self;
    };


    /**
    * 获取网络状态
    * @type {Function}
    */
    $.fn.GetNetWorkType = $.GetNetWorkType = function (success) {
        var _self = this;
        try {
            WX.ready(function () {
                WX.getNetworkType({
                    success: function (res) {
                        var networkType = res.networkType; // 返回网络类型2g，3g，4g，wifi
                        success && success.call(_self, res, networkType);
                    }
                });
            });
        } catch (e) {
            d.error('GetNetWorkType error' + e.message);
            d.lookDebug('GetNetWorkType error:' + e.message);
        }
        return _self;
    };


    /**
    * 获取地理位置
    * @type {Function}
    */
    $.fn.GetLocation = $.GetLocation = function (type, success) {
        var _self = this;
        try {
            WX.getLocation({
                type: type || 'wgs84', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
                success: function (res) {
                    var latitude = res.latitude; // 纬度，浮点数，范围为90 ~ -90
                    var longitude = res.longitude; // 经度，浮点数，范围为180 ~ -180。
                    var speed = res.speed; // 速度，以米/每秒计
                    var accuracy = res.accuracy; // 位置精度
                    success && success.call(_self, res, latitude, longitude, speed, accuracy);
                },
                cancel: function (res) {
                    alert('用户拒绝授权获取地理位置');
                }
            });
        } catch (e) {
            d.error('GetLocation error' + e.message);
            d.lookDebug('GetLocation error:' + e.message);
        }
        return _self;
    };


    /**
    * 打开地图
    * @type {Function}
    */
    $.fn.OpenLocation = $.OpenLocation = function (res) {
        var _self = this;
        try {
            WX.openLocation({
                latitude: res.latitude, // 纬度，浮点数，范围为90 ~ -90
                longitude: res.longitude, // 经度，浮点数，范围为180 ~ -180。
                name: res.name || '当前位置', // 位置名
                address: res.address || '当前位置', // 地址详情说明
                scale: res.scale || 14, // 地图缩放级别,整形值,范围从1~28。默认为最大
                infoUrl: res.infoUrl || '000' // 在查看位置界面底部显示的超链接,可点击跳转
            });
        } catch (e) {
            d.error('OpenLocation error' + e.message);
            d.lookDebug('OpenLocation error:' + e.message);
        }
        return _self;
    };

    /**
    * 隐藏右上角菜单接口
    * @type {Function}
    */
    $.fn.HideOptionMenu = $.HideOptionMenu = function () {
        var _self = this;
        try {
            WX.ready(function () {
                WX.hideOptionMenu();
            });
        } catch (e) {
            d.error('HideOptionMenu error' + e.message);
            d.lookDebug('HideOptionMenu error:' + e.message);
        }
        return _self;
    };


    /**
    * 显示右上角按钮
    * @type {Function}
    */
    $.fn.ShowOptionMenu = $.ShowOptionMenu = function () {
        var _self = this;
        try {
            WX.ready(function () {
                WX.showOptionMenu();
            });
        } catch (e) {
            d.error('ShowOptionMenu error' + e.message);
            d.lookDebug('ShowOptionMenu error:' + e.message);
        }
        return _self;
    };

    /**
    * 发起支付请求
    * @type {Function}
    */
    $.fn.ChooseWXPay = $.ChooseWXPay = function (pay, success) {
        var _self = this;
        try {
            WX.chooseWXPay({
                timestamp: pay.timestamp, // 支付签名时间戳，注意微信jssdk中的所有使用timestamp字段均为小写。但最新版的支付后台生成签名使用的timeStamp字段名需大写其中的S字符
                nonceStr: pay.nonceStr, // 支付签名随机串，不长于 32 位
                package: pay.package, // 统一支付接口返回的prepay_id参数值，提交格式如：prepay_id=***）
                signType: pay.signType || 'SHA1', // 签名方式，默认为'SHA1'，使用新版支付需传入'MD5'
                paySign: pay.paySign, // 支付签名
                success: function (res) {
                    // 支付成功后的回调函数
                    success && success.call(_self, res, pay);
                }
            });
        } catch (e) {
            d.error('ShowOptionMenu error' + e.message);
            d.lookDebug('ShowOptionMenu error:' + e.message);
        }
        return _self;
    };


    /**
    * 创建卡券
    * @type {Function}
    */
    $.fn.CreateCard = $.CreateCard = function (CreateCard, success) {
        var _self = this;
        try {
            $.ajax({
                type: CreateCard.type || 'POST',
                url: CreateCard.api,
                data: CreateCard.data,
                success: function (result) {
                    /**
                    * result 创建的卡券对象
                    * errcode 错误码
                    * card_id 卡券
                    * Signature_api_ticket 签名
                    */
                    success && success.call(_self, result, result.errcode, result.card_id);
                },
                error: function (error) {
                    d.error('CreateCard error' + JSON.stringify(error));
                }
            });
        } catch (e) {
            d.error('CreateCard error' + e.message);
            d.lookDebug('CreateCard error:' + e.message);
        }
        return _self;
    };

    /**
    * 获取卡券签名
    * @type {Function}
    */
    $.fn.GetWxCardSignature = $.GetWxCardSignature = function (si, success) {
        var _self = this;
        try {
            $.ajax({
                type: si.type || 'get',
                url: si.api,
                async: false,
                ContentType: si.ContentType || 'application/x-www-form-urlencoded',
                cache: opts.cache || false,
                data: {
                    'url': location.href,
                    'typenum': opts.typenum,
                    'facid': opts.facid,
                    'cardid': si.card_id,
                    'openid': si.openid,
                    'code': si.code
                },
                success: function (result) {
                    success && success.call(_self, result);
                },
                error: function (error) {

                }
            });
        } catch (e) {
            d.error('GetWxCardSignature error' + e.message);
            d.lookDebug('GetWxCardSignature error:' + e.message);
        }
        return _self;
    }


    /**
    * 创建卡券二维码
    * @type {Function}
    */
    $.fn.CreateCardQR = $.CreateCardQR = function (si, success) {
        var _self = this;
        try {
            $.ajax({
                type: si.type || 'POST',
                url: si.api,
                data: si.data,
                success: function (result) {
                    /**
                    * result 卡券二维码对象
                    */
                    success && success.call(_self, result);
                },
                error: function (error) {
                    d.error('CreateCardQR error' + JSON.stringify(error));
                }
            });
        } catch (e) {
            d.error('CreateCardQR error' + e.message);
            d.lookDebug('CreateCardQR error:' + e.message);
        }
        return _self;
    };


    /**
    * 批量添加卡券
    * @type {Function}
    */
    $.fn.AddCard = $.AddCard = function (card, success) {
        var _self = this;
        try {
            WX.addCard({
                cardList: [{
                    cardId: card.cardid,
                    cardExt: '{"nonce_str":"' + card.nonceStr + '","timestamp" :"' + card.timestamp + '", "signature":"' + card.signature + '"}'
                }], // 需要添加的卡券列表
                success: function (res) {
                    var cardList = res.cardList; // 添加的卡券列表信息
                    /**
                    * res 添加的卡券对象
                    * cardList 添加的卡券列表信息
                    */
                    success && success.call(_self, res, cardList);
                }
            });
        } catch (e) {
            d.error('AddCard error' + e.message);
            d.lookDebug('AddCard error:' + e.message);
        }
        return _self;
    };

    /**
    * 关闭当前网页窗口接口
    * @type {Function}
    */
    $.fn.CloseWindow = $.CloseWindow = function () {
        var _self = this;
        try {
            WX.ready(function () {
                WX.closeWindow();
            });
        } catch (e) {
            d.error('CloseWindow error' + e.message);
            d.lookDebug('CloseWindow error:' + e.message);
        }
        return _self;
    };

    /**
    * 微信初始化失败回调函数
    * @type {Function}
    */
    $.fn.InitWxError = $.InitWxError = function (callback) {
        var _self = this;
        WX.error(function (res) {
            callback && callback.call(_self, res);
        });
        return _self;
    };

    /**
    * 是否是微信浏览器
    * @type {Function}
    */
    $.fn.IsWeChatBrower = $.IsWeChatBrower = function (callback) {
        var ua = window.navigator.userAgent.toLowerCase();
        var _self = this;
        callback && callback.call(_self, ua.match(/micromessenger/i) == 'micromessenger');
        return _self;
    };
})(jQuery, wx);
