var app = angular.module('KMS', ['ngRoute', 'KMS.services']);
app.controller('configCtrl', ['$scope', 'instance', 'windowService', '$location', 'httpService', '$http', 'CreateTOKENService', function ($scope, instance, windowService, $location, httpService, $http, CreateTOKENService) {
    var config = {
        //厂家编号
        facid: '9667',
        typenum: '2',
        api: 'http://dmapi.yesno.com.cn/api/wechat/GetWxInfoAndSign',
        notwechart: '../../../_public/notwechart/authorize.html',
        GoToTBUrl: 'http://msbcdsit.ecpic.com.cn/mobilemsb/product/initPrd',
        clientLog: new Image(), //方便客户端写入错误日志
        uri: windowService.queryString('uri'),
        forword_title: '',
        forword_desc: '',
        forword_link: '',
        forword_imgUrl: '',
        storage: localStorage,
        result: [

            { "syscode": '000', "message": 'NetWork Error!', "show": '网络繁忙', "tip": "请扫描正确的数码参与活动", "img": "../../static/images/face_3.png", type: 'Default', flag: "1", "des": "默认答复" },

            { "syscode": "0X0001", "message": "N非法提交", "show": "非法提交", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "000000", "message": "N系统异常", "show": "系统异常", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "001000", "message": "Y成功", "show": "成功", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "001001", "message": "Y手机验证码验证成功", "show": "手机验证码验证成功", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "001002", "message": "Y手机验证码发送成功", "show": "手机验证码发送成功", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "001003", "message": "Y中奖SW", "show": "中奖SW", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "0010031", "message": "一等奖", "show": "Iphone7（128G）亮黑色", "tip": "", "img": "phone.png", "type": "lottery", "flag": "1", "des": "memo" },
            { "syscode": "0010032", "message": "二等奖", "show": "Ipad（32G）新款 银色", "tip": "", "img": "ipad.jpg", "type": "lottery", "flag": "1", "des": "memo" },
            { "syscode": "0010033", "message": "三等奖", "show": "新秀丽背包", "tip": "", "img": "bag.jpg", "type": "lottery", "flag": "1", "des": "memo" },
            { "syscode": "0010034", "message": "四等奖", "show": "轮胎组合工具(黑色配银色）", "tip": "", "img": "tool.jpg", "type": "lottery", "flag": "1", "des": "memo" },
            { "syscode": "0010035", "message": "五等奖", "show": "洗漱包", "tip": "", "img": "bags.jpg", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002000", "message": "NN失败", "show": "失败", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002001", "message": "NN手机号码格式不正确", "show": "手机号码格式不正确", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002002", "message": "NN手机号码不能为空", "show": "手机号码不能为空", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002003", "message": "NN手机验证码格式不正确", "show": "手机验证码格式不正确", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002004", "message": "NN验证码验证失败", "show": "验证码验证失败", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002005", "message": "NN验证码发送失败", "show": "验证码发送失败", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002006", "message": "NN防伪数码不能为空", "show": "防伪数码不能为空", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002007", "message": "NNopenid为空", "show": "openid为空", "tip": "", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002008", "message": "NN解码的厂家不是本活动厂家", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002009", "message": "NN二维码解码失败", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002010", "message": "NN防伪数码格式错误", "show": "防伪数码格式错误", "tip": "", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002011", "message": "NN抽奖活动不存在", "show": "抽奖活动不存在", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002012", "message": "NN活动答复未配置", "show": "活动答复未配置", "tip": "", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002013", "message": "NNIP地址格式不正确", "show": "IP地址格式不正确", "tip": "", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002014", "message": "NN不是指定活动参与的数码厂家", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002015", "message": "NN数码所属表不存在", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002016", "message": "NN数码已经过期", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002017", "message": "NN数码所属表信息表结构异常", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002018", "message": "NN数码不存在", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002019", "message": "NN产品不参与活动", "show": "产品不参与活动", "tip": "请扫描正确的数码参与活动", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002020", "message": "NN生产时间不在许可范围", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002021", "message": "NN数码生产时间为空", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002022", "message": "NN未配置参与活动的产品", "show": "产品不参与活动", "tip": "请扫描正确的数码参与活动", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002023", "message": "NN数码未激活", "show": "数码不存在", "tip": "请扫描正确的数码参与活动", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002024", "message": "NN活动未开始", "show": "活动未开始", "tip": "请5月15日后再来吧~", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002025", "message": "NN活动已结束", "show": "活动已结束", "tip": "更多活动，请继续关注康明斯公众号~", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002026", "message": "NN活动奖池为空", "show": "奖项名额被抢光啦！下次早点来咯~", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002027", "message": "NN用户已达当日开奖上限", "show": "用户已达当日开奖上限", "tip": "请明天再来吧！", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002028", "message": "NN未填写邮寄信息", "show": "未填写邮寄信息", "tip": "请先填写邮寄信息", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002029", "message": "NN数码已经参与过活动", "show": "数码已经参与过活动", "tip": "该码已参加过本活动", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002030", "message": "NN未中奖", "show": "这次未中奖哦", "tip": "再接再厉吧！", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002031", "message": "NN未中奖_未命中奖池", "show": "这次未中奖哦", "tip": "再接再厉吧！", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002032", "message": "NNopenid不存在记录", "show": "openid不存在记录", "tip": "", "img": "ng.png", "type": "lottery", "flag": "1", "des": "memo" },

            { "syscode": "002033", "message": "NN为查询到中奖记录", "show": "为查询到中奖记录", "tip": "", "img": "wq.png", "type": "lottery", "flag": "1", "des": "memo" }
        ]
    };

    config.getResult = function (syscode, type) {
        return c = (t = config.result.filter(function (item, index, array) {
            return (item.syscode === syscode && item.flag === "1");
        })).length > 0 ? t[0] : config.result[0];
    };

    config.WeChat = $.WeChart({
        api: config.api,
        type: 'GET',
        facid: config.facid,
        typenum: config.typenum,
        async: false
    });

    config.layer = {
        open: function (msg) {
            layer.open({
                content: msg
                , skin: 'msg'
                , time: 2
            });
        },
        shade: function () {
            layer.open({ type: 2, shadeClose: false });
        }

    };



    setTimeout(function () {
        config.WeChat.IsWeChatBrower(function (y) {
            if (!y) {
                //location.href = config.notwechart;
            }
        });
    }, 500);


    CreateTOKENService.Create(config.facid);


    $scope.config = config;

} ]);