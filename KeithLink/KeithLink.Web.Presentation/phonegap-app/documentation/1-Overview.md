# Phonegap project overview

The Phonegap project is located in the ```/KeithLink/KeithLink.Web.Presentation/phonegap-app/BEKPhoneGap``` directory.

## Copying files from the Angular app

All project files are located in ```www/```. These files are copied from the angular-app directory using the command ```grunt update```. 

## Extending the Angular app with phonegap services

The ```scripts/services/phonegapServices/``` directory is the only directory that is not copied from the angular app. We use these services to extend existing angular services and add additional functionality (like offline storage) without touching the angular files.

Only angular **services** can be extended. If changes need to be made to controllers/directives/views, you will have to do these in the angular app and copy them into the phonegap app. Otherwise, they will be overwritten when you do a ```grunt update```.

### Making changes in controllers/directives/views

If you do need to add something to a controller/direvice/view in the angular app, you can inject the ```ENV``` service. This contains all the environment variables for the app. Use ```ENV.mobileApp``` to determine if you are using the mobile app or web app.

See this example from the MenuController

~~~javascript
angular.module('bekApp')
  .controller('MenuController', ['$scope', 'ENV', ($scope, ENV) {

  // PHONEGAP Feature
  $scope.scanBarcode = function() {
    cordova.plugins.barcodeScanner.scan(
      function (result) {
        var scannedText = result.text;
        console.log(scannedText);
    });
  };
}]);
~~~