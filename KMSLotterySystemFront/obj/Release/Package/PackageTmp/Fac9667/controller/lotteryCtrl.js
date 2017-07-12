app.controller('lotteryCtrl', ['$scope', 'instance', 'windowService', '$location', 'httpService', '$http', 'formInitialize', 'SendMobileCodeService', '$interval', 'CreateTOKENService', function ($scope, instance, windowService, $location, httpService, $http, formInitialize, SendMobileCodeService, $interval, CreateTOKENService) {
    var wechatManage = $scope.config.WeChat.InitWeChat();

    var lottery = {
        digitcode: localStorage.getItem('c_digitcode'),
        lid: localStorage.getItem('c_lid'),
        lotterylevel: localStorage.getItem('c_lotterylevel'),
        mobile: localStorage.getItem('c_mobile'),
        result: localStorage.getItem('c_result')
    };
    var lt = $scope.config.getResult('001003' + lottery.lotterylevel);
    lottery.lt = lt;


    if ($location.absUrl().indexOf('/addr.html') > -1) {
        if (!lottery.digitcode || !lottery.mobile) {
            location.href = "../po/index.html";
            return;

        }
    }

    if ($location.absUrl().indexOf('/lottery.html') > -1) {

        if (!lottery.lid || !lottery.digitcode || !lottery.lotterylevel || !lottery.mobile) {
            location.href = "../po/index.html";
            return;
        }
    }





    var mylayer = $scope.config.layer;

    //表单验证
    function validItems() {

        //lottery.pname = $(".pname").find("option:selected").text();
        if (!lottery.pname || lottery.pname.indexOf("请选择") > -1) {
            mylayer.open('请填写省份！');
            return false;
        }

        //lottery.cname = $(".cname").find("option:selected").text();
        if (!lottery.cname || lottery.cname.indexOf("请选择") > -1) {
            mylayer.open('请填写城市！');
            return false;
        }
        lottery.addr = $('[type=text].address').val();
        if (!lottery.addr) {
            mylayer.open('请输入详细地址');
            return false;
        }

        return true;
    }; //表单验证结束


    lottery.PostAddr = function () {
        if (validItems()) {
            mylayer.shade();
            var d = {
                mobile: lottery.mobile,
                provice: lottery.pname,
                city: lottery.cname,
                address: lottery.addr,
                lid: lottery.lid,
                digitcode: lottery.digitcode,
                action: "10",
                token: $('#TOKEN_Hidden').val()
            };
            httpService.setting('../server/KMS.ashx', d).then(function (data, status, headers, config) {
                try {
                    layer.closeAll('loading');

                    console.log(data);
                    CreateTOKENService.Create($scope.config.facid); //重新给定Token
                    if (data.sysCode == '001000') { //成功
                        localStorage.setItem('c_digitcode', "");
                        localStorage.setItem('c_lid', "");
                        localStorage.setItem('c_lotterylevel', "");
                        localStorage.setItem('c_mobile', "");
                        localStorage.setItem('c_result', "");
                        location.href = '../po/success.html';
                        return;
                    } else {
                        var lt = $scope.config.getResult(data.sysCode);
                        mylayer.open(lt.show);
                    }

                } catch (e) {
                    CreateTOKENService.Create($scope.config.facid); //重新给定Token
                    $scope.config.clientLog.src = "../server/KMS.ashx?action=2&type=PostAddr&e=" + JSON.stringify(e);
                } finally {

                }
            }, function (reason) {
                CreateTOKENService.Create($scope.config.facid); //重新给定Token
                $scope.config.clientLog.src = "../server/KMS.ashx?action=2&type=PostAddr&e=" + JSON.stringify(reason);
            });
        }
    };


    $scope.lottery = lottery;
} ]);