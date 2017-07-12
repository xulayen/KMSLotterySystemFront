app.controller('regCtrl', ['$scope', 'instance', 'windowService', '$location', 'httpService', '$http', 'formInitialize', 'SendMobileCodeService', '$interval', 'CreateTOKENService', function ($scope, instance, windowService, $location, httpService, $http, formInitialize, SendMobileCodeService, $interval, CreateTOKENService) {
    var wechatManage = $scope.config.WeChat.InitWeChat();

    var R = {
        paracont: "获取验证码",
        paraevent: true,
        openid: localStorage.getItem('openid'),
        Join: true,
        Tag: "zhongduan",
        Sex: "0",
        Area: ""
    };



    var mylayer = $scope.config.layer;

    //表单验证
    function validItems(submit) {
        console.log(R);
        if (submit) {
            if (!R.UserName) {
                mylayer.open('请填写姓名！');
                return false;
            }

            R.VerifyCode = ($("[type=text].lInput_msg").val());
            if (!R.VerifyCode) {
                mylayer.open('请输入验证码');
                return false;
            }

            if (!(/^\d+$/.test(R.VerifyCode))) {
                mylayer.open('请输入正确的验证码');
                return false;
            }
        }

        if (!R.Mobile) {
            mylayer.open('请填写手机号码');
            return false;
        }

        if (!(/^1[\d]{10}$/.test(R.Mobile))) {
            mylayer.open('手机号码格式不正确');
            return false;
        }




        return true;
    }; //表单验证结束


    /**
    **发送验证码
    **/
    R.btnSend = function () {
        if (validItems()) {
            var second = 60, timePromise = undefined;
            if (R.paraevent) {
                mylayer.shade();
                SendMobileCodeService.send(R.Mobile).then(function (data, status, headers, config) {
                    try {
                        CreateTOKENService.Create($scope.config.facid); //重新给定Token
                        layer.closeAll('loading');
                        if (data.state == '1') {
                            timePromise = $interval(function () {
                                if (second <= 0) {
                                    $interval.cancel(timePromise);
                                    timePromise = undefined;
                                    second = 60;
                                    R.paracont = "重发验证码";
                                    R.paraevent = true;
                                } else {
                                    R.paraevent = false;
                                    R.paracont = second + "秒后重发";
                                    second--;
                                }
                            }, 1000, 100);
                        } else {

                            console.log(data);
                            mylayer.open($scope.config.getResult(data.sysCode, 'SendMessages').show);
                        }
                    } catch (e) {
                        CreateTOKENService.Create($scope.config.facid); //重新给定Token
                        $scope.config.clientLog.src = "../server/KMS.ashx?action=2&type=sendMessages&e=" + JSON.stringify(e);
                    } finally {

                    }
                }, function (reason) {
                    CreateTOKENService.Create($scope.config.facid); //重新给定Token
                    $scope.config.clientLog.src = "../server/KMS.ashx?action=2&type=sendMessages&e=" + reason;
                    mylayer.open($scope.config.getResult('000', 'SendMessages').show);
                });
            }
        }
    }; //发送验证码函数结束


    //提交表单
    R.Submit = function () {
        if (validItems(true)) {
            mylayer.shade();
            var d = {
                "mobile": (R.Mobile || ''),
                "verifycode": (R.VerifyCode || ''),

                "Tag": R.Tag,
                "Area": (R.Area || ''),
                "province": (R.Province || ''),
                "cityName": (R.CityName || ''),
                "Sex": (R.Sex || ''),
                "Address": (R.Address || ''),

                "userName": (R.UserName || ''),
                "action": "1",
                "openid": R.openid,
                "token": $("#TOKEN_Hidden").val(),
                "channel": $scope.config.storage.getItem("channel"),
                "join": R.Join ? "1" : "0"
            };

            httpService.setting('../server/KMS.ashx', d).then(function (data, status, headers, config) {
                //////
                try {
                    $scope.config.storage.setItem("channel", "");
                    layer.closeAll('loading');
                    if (data.state == "1") {
                        window.location.href = 'scan.html?uri=' +
                        $.base64.encode(JSON.stringify(windowService.encode(d)));
                    } else {
                        CreateTOKENService.Create($scope.config.facid); //重新给定Token
                        var lt = $scope.config.getResult(data.sysCode, 'SubmitForm');
                        mylayer.open(lt.show);

                        //                        if (data.sysCode == '304') {
                        //                            
                        //                        } else {
                        //                            $(".result_error_title").html(lt.show);
                        //                            $(".result_error_cnt").html(lt.tip);
                        //                            //mylayer.open();
                        //                            setTimeout(function () {
                        //                                $('.mask').show().find('.error_box_common').addClass('zoomIn');
                        //                            }, 300);
                        //                        }

                    }
                } catch (e) {
                    CreateTOKENService.Create($scope.config.facid); //重新给定Token
                    $scope.config.clientLog.src = "../server/KMS.ashx?action=2&type=sendMessages&e=" + JSON.stringify(e);
                } finally {

                }
            }, function (reason) {
                CreateTOKENService.Create($scope.config.facid); //重新给定Token
                $scope.config.clientLog.src = "../server/KMS.ashx?action=2&type=sendMessages&e=" + JSON.stringify(reason);
            });
        }
    }; //提交表单结束



    R.noticeHide = function () {
        noticeHide();
    };

    $scope.R = R;
} ]);