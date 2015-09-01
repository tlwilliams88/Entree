    'use strict';

    angular.module('bekApp')
      .controller('ApplicationSettingsController', ['$scope', 'SessionService', '$state', '$filter', 'AccessService', 'ApplicationSettingsService', 'LocalStorage',
        function ($scope, SessionService, $state, $filter, AccessService, ApplicationSettingsService, LocalStorage) {


     $scope.pages =  {
      availablePages: [
        { id: 'lists', name: "Lists" },
        { id: 'addtoorder', name: "Add To Order" }
      ],
      selectedPage: { id: 'lists', name: "Lists" }
      };
       
      var init = function(){ 
        if (AccessService.isOrderEntryCustomer()){
          ApplicationSettingsService.getNotificationPreferencesAndFilterByCustomerNumber(null).then(function (preferences) {
            $scope.defaultPreferences = preferences;
          });
        } else {
          $scope.hideNotificationPreferences = true;
        }

    $scope.listFields =  [
        { "value": 'position', "text": "Position", "order": '', "sortDesc": "n", "isSelected": false, "code": 0 },
        { "value": 'itemnumber', "text": "Item #", "order": '', "sortDesc": "n", "isSelected": false, "code":  1 },
        { "value": 'name', "text": "Name", "order": '', "sortDesc": "n", "isSelected": false, "code": 2  },
        { "value": 'brandextendeddescription', "text": "Brand", "order": '', "sortDesc": "n", "isSelected": false, "code":  3 },
        { "value": 'itemclass', "text": "Category", "order": '', "sortDesc": "n", "isSelected": false, "code": 4  },
        { "value": 'notes', "text": "Notes", "order": '', "sortDesc": "n", "isSelected": false, "code":  5 },
        { "value": 'label', "text": "Label", "order": '', "sortDesc": "n", "isSelected": false, "code": 6 },
        { "value": 'parlevel', "text": "Par Level", "order": '', "sortDesc": "n", "isSelected": false, "code": 7 }];


    $scope.addToOrderFields =  angular.copy($scope.listFields);
    $scope.addToOrderFields = $scope.addToOrderFields.slice(0,7);

    $scope.pageLoadSize = LocalStorage.getPageSize();
    var sortSettings = LocalStorage.getDefaultSort();
    var userid = SessionService.userProfile.userid;

    if(sortSettings && sortSettings.length > 6){
      //parsing out list and ato sections from stored sort preferences
      var atoSettings = sortSettings.slice(sortSettings.indexOf('a') + 3, sortSettings.length);
      var listSettings = sortSettings.slice(3, sortSettings.indexOf('a'));
      //loop through each section to apply saved settings to view. Numeric characters represent the index of the field in the listFields/atoFields arrays
      //y/n represent whether or not the field will be sorted descending. Each numeric character is followed by the y/n that corresponds to it
        for (var i = 0;  i < atoSettings.length; i++) {
          if(atoSettings[i] !== 'y' && atoSettings[i] !== 'n'){
            var orderAssigner = (i === 0) ? 1 : 2 ;                
            $scope.addToOrderFields[atoSettings[i]].order = orderAssigner;
            $scope.addToOrderFields[atoSettings[i]].sortDesc = atoSettings[i + 1];
            $scope.addToOrderFields[atoSettings[i]].isSelected = true;
          }
        }

        for (var i = 0;  i < listSettings.length; i++) {
          if(listSettings[i] !== 'y' && listSettings[i] !== 'n'){
            var orderAssigner = (i === 0) ? 1 : 2 ;                
            $scope.listFields[listSettings[i]].order = orderAssigner;
            $scope.listFields[listSettings[i]].sortDesc = listSettings[i + 1];
            $scope.listFields[listSettings[i]].isSelected = true;
          }
        }
    }

      if(!$scope.pageLoadSize){
        $scope.pageLoadSize = 30;
      }

      };

      $scope.setDesc = function(field){
        field.sortDesc = (field.sortDesc === 'n') ? 'y' : 'n' ;
      }
      
      $scope.goBack = function(){
        $state.go('menu.home');
      };

      $scope.setOrder = function(field, fields, isSelected){
        if(!field.isSelected){
          field.isSelected = false;
          field.order='';
          field.sortDesc = 'n';
         
           fields.forEach(function(sortField){
          if(sortField.order == 2){
            sortField.order = 1;
          }
        })
        }
        else{
        var selectedCount = 0;    
            fields.forEach(function(sortField){
             if(sortField.order === 2){
                sortField.isSelected = false;
                field.sortDesc = 'n';
                sortField.order='';           
                selectedCount++;
             }
             if(sortField.order === 1){
                selectedCount++;
             }        
            })
            field.order = (selectedCount > 0) ? 2 : 1;
            field.isSelected = true;        
        }   
      }

       $scope.restoreDefaults = function(){
         var pageSizeSettings = {key: 'pageLoadSize'};
          ApplicationSettingsService.resetApplicationSettings(pageSizeSettings).then(function(resp) { 
            LocalStorage.setPageSize(30);
            var sortOrderSettings = {key: 'sortPreferences'};
            ApplicationSettingsService.resetApplicationSettings(sortOrderSettings).then(function(resp) {
             LocalStorage.setDefaultSort([]);
             init();
            });
          });
       }

       $scope.savePreferences = function () {
        ApplicationSettingsService.updateNotificationPreferences($scope.defaultPreferences, null).then(function(data) {
          $scope.canEditNotifications = false;
          $scope.notificationPreferencesForm.$setPristine();
          var pageSizeSettings = {userid: '', key: 'pageLoadSize', value: $scope.pageLoadSize};
          LocalStorage.setPageSize($scope.pageLoadSize);
          ApplicationSettingsService.saveApplicationSettings(pageSizeSettings).then(function(resp) {

            $scope.pageSizeForm.$setPristine();
            //generate string that represents the sort order settings
            //format is one string with two sections
            //each section contains a three letter identifier, and four characters representing the field and desc/asc order
            var sortOrder = 'lis';

            var firstSort =  $filter('filter')($scope.listFields, {order: 1})[0];
            if(firstSort){
              sortOrder = sortOrder + firstSort.code + firstSort.sortDesc;
            }

            var secondSort =  $filter('filter')($scope.listFields, {order: 2})[0];
            if(secondSort){
              sortOrder = sortOrder + secondSort.code + secondSort.sortDesc;
            }    

             
            sortOrder = sortOrder +'ato';

            firstSort =  $filter('filter')($scope.addToOrderFields, {order: 1})[0];
            if(firstSort){
              sortOrder = sortOrder + firstSort.code + firstSort.sortDesc;
            }

            secondSort =  $filter('filter')($scope.addToOrderFields, {order: 2})[0];
            if(secondSort){
              sortOrder = sortOrder + secondSort.code + secondSort.sortDesc;
            }

            var sortOrderSettings = {userid: '', key: 'sortPreferences', value: sortOrder};


             LocalStorage.setDefaultSort(sortOrderSettings.value);
            ApplicationSettingsService.saveApplicationSettings(sortOrderSettings).then(function(resp) {
     
             });
          });
        });
      };

      init();

    }]);
