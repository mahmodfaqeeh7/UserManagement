var app = angular.module('userManagementApp', []);

app.controller('UsersController', function ($scope, $http) {
    $scope.users = [];

    // Fetch users from the backend
    $scope.loadUsers = function () {
        $http.get('/Users/Index').then(function (response) {
            $scope.users = response.data;
        });
    };

    // Add user function
    $scope.addUser = function () {
        $http.post('/Users/Create', $scope.newUser).then(function (response) {
            $scope.loadUsers();
        });
    };

    $scope.loadUsers(); // Load users on page load
});


