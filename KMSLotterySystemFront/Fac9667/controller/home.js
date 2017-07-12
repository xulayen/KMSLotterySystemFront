app.controller('homeCtrl', ['$scope', 'instance', 'windowService', '$location', 'httpService', '$http', 'formInitialize', 'SendMobileCodeService', '$interval', 'CreateTOKENService', function ($scope, instance, windowService, $location, httpService, $http, formInitialize, SendMobileCodeService, $interval, CreateTOKENService) {
    var channel = (windowService.queryString('c'));
    var code = (windowService.queryString('code'));
    if (channel) {
        $scope.config.storage.setItem("channel", channel);
    }

    if (code) {
        $scope.config.storage.setItem("code", code);
    }

    var wechatManage = $scope.config.WeChat.InitWeChat();
    if (!windowService.queryString('openid')) {
        //http://wechat.cummins.com.cn/open/weixin/authbase/getuserinfo.do?libraryid=1&linkurl=http://152l8u0817.51mypc.cn/lotterywb/fac9667/po/index.html&oauth2=snsapi_userinfo
        //http://dm.zhsh.co/userauth/default/UserAuth.aspx?returnUrl=http://152l8u0817.51mypc.cn/lotterywb/fac9667/po/index.html&c=y&facid=9667&type=2&typenum=2
        location.href = 'http://wechat.cummins.com.cn/open/weixin/authbase/getuserinfo.do?libraryid=1&linkurl=http://dm.zhsh.co/dmwb/fac9667/po/index.html&oauth2=snsapi_userinfo';
        return;
    }

    $("body").show();

    var mylayer = $scope.config.layer;
    var home = {
        sudo: function () {
            localStorage.setItem('opneid', windowService.queryString('openid'));
            setTimeout(function () {
                new Image().src = "../server/KMS.ashx?action=6&data=" + (windowService.queryString('openid'));
            }, 300);


        } ()
    };

    home.gotoLottery = function () {
        mylayer.shade();
        var d = {
            openid: windowService.queryString('openid'),
            action: "8",
            token: $('#TOKEN_Hidden').val()
        };
        httpService.setting('../server/KMS.ashx', d).then(function (data, status, headers, config) {
            try {

                console.log(data);
                CreateTOKENService.Create($scope.config.facid); //重新给定Token
                if (data.sysCode == '001004') { //已存在
                    var openid = data.data.openid;
                    var mobile = data.data.mobile;
                    localStorage.setItem('c_mobile', mobile);
                    localStorage.setItem('c_openid', openid);
                    location.href = 'scan.html';
                } else {
                    location.href = 'reg.html';
                }
                layer.closeAll('loading');
            } catch (e) {
                CreateTOKENService.Create($scope.config.facid); //重新给定Token
                $scope.config.clientLog.src = "../server/KMS.ashx?action=2&type=sendMessages&e=" + JSON.stringify(e);
            } finally {

            }
        }, function (reason) {
            CreateTOKENService.Create($scope.config.facid); //重新给定Token
            $scope.config.clientLog.src = "../server/KMS.ashx?action=2&type=sendMessages&e=" + JSON.stringify(reason);
        });

    };






    $scope.home = home;
} ]);