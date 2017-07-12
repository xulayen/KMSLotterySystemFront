app.controller('successCtrl', ['$scope', 'instance', 'windowService', '$location', 'httpService', '$http', 'CreateTOKENService', function ($scope, instance, windowService, $location, httpService, $http, CreateTOKENService) {
    var wechatManage = $scope.config.WeChat.InitWeChat();

    var succ = {};

    succ.Go = function () {
        location.href = 'index.html';
    };

    $scope.succ = succ;
} ]);