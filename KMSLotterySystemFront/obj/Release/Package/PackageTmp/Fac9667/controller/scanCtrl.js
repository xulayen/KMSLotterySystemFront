app.controller('scanCtrl', ['$scope', 'instance', 'windowService', '$location', 'httpService', '$http', 'CreateTOKENService', function ($scope, instance, windowService, $location, httpService, $http, CreateTOKENService) {


    var wechatManage = $scope.config.WeChat.InitWeChat();
    var mylayer = $scope.config.layer;
    //    {
    //        分享到朋友圈: true,
    //        发送给朋友: true
    //    }

    //    //分享到朋友圈
    //    wechatManage.Forword({
    //        forword_title: $scope.config.forword_title,
    //        forword_desc: $scope.config.forword_desc,
    //        forword_link: $scope.config.forword_link,
    //        forword_imgUrl: $scope.config.forword_imgUrl
    //    });

    //    //分享给朋友
    //    wechatManage.ForwordToFriend({
    //        forword_title: $scope.config.forword_title,
    //        forword_desc: $scope.config.forword_desc,
    //        forword_link: $scope.config.forword_link,
    //        forword_imgUrl: $scope.config.forword_imgUrl
    //    });


    var scan = {
        sudo: function () {
            localStorage.setItem('uri', windowService.queryString('uri'));
        } (),
        formData: function () {
            var c = {};
            if (localStorage.getItem('uri')) {
                c = windowService.decode(JSON.parse($.base64.decode(localStorage.getItem('uri'))));
            } else {
                c = {
                    "mobile": localStorage.getItem('c_mobile'),
                    "openid": localStorage.getItem('c_mobile'),
                    "channel": $scope.config.storage.getItem("channel")
                };
            }
            return c;
        } (),
        lottery_dom: $(".sResult_get_wrap"),
        notice_dom: $(".sResult_error"),
        mask_dom: $(".mask"),
        lt: {},
        lonlat: '',
        time: 0,
        system: function () {
            var currentSystem = [];
            var system = BroswerUtil.CurrentSystem.system;
            for (var k in system) {
                if (system[k] && system[k] != '') {
                    currentSystem.push(k);
                    if (Object.prototype.toString.call(system[k]) === "[object Number]") {
                        currentSystem.push(system[k]);
                    }
                }
            };
            return currentSystem;
        } (),
        browser: function () {
            return BroswerUtil.getBrowserVersion();
        } (),
        TB: {},
        code: $scope.config.storage.getItem("code"),
        openid: localStorage.getItem('openid')
    };




    //alert('1:' + wechatManage)
    var begin = new Date().getTime();
    setTimeout(function () {
        wechatManage.GetLocation('wgs84', function (res) {
            scan.lonlat = (res.longitude + "|" + res.latitude);
            var end = new Date().getTime();
            scan.time = parseInt(end) - parseInt(begin);
            //            alert("2:" + res.longitude);
            //            alert("4:" + scan.lonlat)
        });
    }, 600);





    scan.Scan = function () {
        wechatManage.Scan(function (c) {
            doLottery(c);
        });
    };

    scan.Submit = function () {
        scan.code = $("[type=text].sIput_input").val();
        if (!scan.code) {
            mylayer.open('请输入数码');
            return;
        }

        if (!/^\d{16}$/.test(scan.code)) {
            mylayer.open('请输入16位数字');
            return;
        }


        if (scan.code) {
            doLottery(scan.code);
        }
    };

    function doLottery(q) {
        mylayer.shade();
        scan.mask_dom.show();
        //解密
        var d = scan.formData;
        d.action = "3";
        d.q = q;
        d.useropenid = $scope.config.useropenid;
        d.openid = scan.openid;
        d.lonlat = scan.lonlat; //经纬度
        d.time = scan.time; //gps耗时
        d.browser = scan.browser.join(' '); //浏览器
        d.system = scan.system.join(' '); //系统
        d.token = $('#TOKEN_Hidden').val();

        //////

        httpService.setting('../server/kms.ashx', d).then(function (data, status, headers, config) {
            layer.closeAll('loading');
            CreateTOKENService.Create($scope.config.facid); //重新给定Token
            console.log(data);
            var lt = $scope.config.getResult(data.sysCode, 'lottery');
            scan.lt = lt;

            localStorage.setItem('c_digitcode', data.data.digitcode);
            localStorage.setItem('c_lid', data.data.lid);
            localStorage.setItem('c_lotterylevel', data.data.lotteryLevel);
            localStorage.setItem('c_mobile', data.data.mobile);
            localStorage.setItem('c_result', data.data.result);

            localStorage.setItem("code", "");
            if (data.sysCode == '001003') {
                location.href = 'lottery.html';
                return;
            } else {
                if (data.sysCode == '002028') {
                    location.href = 'addr.html';
                    return;
                }
                //mylayer.open(lt.show);
                showCommon_box();
            }
        }, function (reason) {
            $scope.config.clientLog.src = "../server/kms.ashx?action=2&type=getResult&e=" + reason;
            layer.closeAll('loading');
        });
    };


    $scope.scan = scan;
} ]);