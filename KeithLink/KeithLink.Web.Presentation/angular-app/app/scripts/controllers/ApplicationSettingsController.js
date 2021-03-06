    'use strict';

    angular.module('bekApp')
      .controller('ApplicationSettingsController', ['$scope', 'SessionService', '$state', '$filter', 'AccessService', 'ApplicationSettingsService', 'LocalStorage',
        function ($scope, SessionService, $state, $filter, AccessService, ApplicationSettingsService, LocalStorage) {



       
      var init = function(){ 
        if (AccessService.isOrderEntryCustomer()){
          ApplicationSettingsService.getNotificationPreferencesAndFilterByCustomerNumber(null).then(function (preferences) {
            $scope.defaultPreferences = preferences;
          });
        } else {
          $scope.hideNotificationPreferences = true;
        }

        $scope.resetPage();

        $scope.addToOrderFields =  angular.copy($scope.listFields);
        $scope.addToOrderFields.splice(7,1); //drop out the parlevel field from the copy
        // to create the addtoorderfields from the hardcoded listfields

        $scope.pageSizes.selectedSize = LocalStorage.getPageSize();

        var sortSettings = LocalStorage.getDefaultSort(),
            userid = SessionService.userProfile.userid;

        if(sortSettings && sortSettings.length > 6){
          //parsing out list and ato sections from stored sort preferences
          var atoSettings = sortSettings.slice(sortSettings.indexOf('a') + 3, sortSettings.length),
              listSettings = sortSettings.slice(3, sortSettings.indexOf('a')),
              i,
              orderAssigner;
          //loop through each section to apply saved settings to view. Numeric characters represent the index of the field in the listFields/atoFields arrays
          //y/n represent whether or not the field will be sorted descending. Each numeric character is followed by the y/n that corresponds to it
          //these were just using the index of the presentation array to set the selected and such, but in
          //the case of addtoorder we want to skip on of the fields, so we need to actually map
          //by the code 
            for (i = 0;  i < atoSettings.length; i++) {
              var visualIndex = 0;
              for (var y = 0; y < $scope.addToOrderFields.length; y++){
                if($scope.addToOrderFields[y].code == atoSettings[i]){
                  visualIndex = y;
                  break;
                }
              }
              if(atoSettings[i] !== 'y' && atoSettings[i] !== 'n'){
                orderAssigner = (i === 0) ? 1 : 2 ;                
                $scope.addToOrderFields[visualIndex].order = orderAssigner;
                $scope.addToOrderFields[visualIndex].sortDesc = atoSettings[i + 1];
                $scope.addToOrderFields[visualIndex].isSelected = true;
              }
            }

            for (i = 0;  i < listSettings.length; i++) {
              var visualIndex = 0;
              for (var y = 0; y < $scope.listFields.length; y++){
                if($scope.listFields[y].code == listSettings[i]){
                  visualIndex = y;
                  break;
                }
              }
              if(listSettings[i] !== 'y' && listSettings[i] !== 'n'){
                orderAssigner = (i === 0) ? 1 : 2 ;                
                $scope.listFields[visualIndex].order = orderAssigner;
                $scope.listFields[visualIndex].sortDesc = listSettings[i + 1];
                $scope.listFields[visualIndex].isSelected = true;
              }
            }
        }
      };

      $scope.resetPage = function(){        
        $scope.pages =  {
          availablePages: [
            { id: 'lists', name: 'Lists' },
            { id: 'addtoorder', name: 'Add To Order' }
          ],
          selectedPage: { id: 'lists', name: 'Lists' }
        };

        $scope.defaultPageSize = 50;
        $scope.pageSizes = {
          availableSizes: [25, 50, 100, 200],
          selectedSize: $scope.defaultPageSize
        };

        
        $scope.listFields =  [
            { 'value': 'position', 'text': 'Line #', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code': 0 },
            { 'value': 'itemnumber', 'text': 'Item #', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code':  1 },
            { 'value': 'name', 'text': 'Name', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code': 2  },
            { 'value': 'brandextendeddescription', 'text': 'Brand', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code':  3 },
            { 'value': 'itemclass', 'text': 'Category', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code': 4  },
            { 'value': 'notes', 'text': 'Notes', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code':  5 },
            { 'value': 'label', 'text': 'Label', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code': 6 },
            { 'value': 'parlevel', 'text': 'Par Level', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code': 7 },
            { 'value': 'category', 'text': 'Contract Category', 'order': '', 'sortDesc': 'n', 'isSelected': false, 'code': 8 }];
      };

      $scope.setDesc = function(field){
        $scope.sortPreferencesForm.$setDirty();
        field.sortDesc = (field.sortDesc === 'n') ? 'y' : 'n' ;
      };

      
      $scope.goBack = function(){
        $state.go('menu.home');
      };

      $scope.setOrder = function(field, fields, isSelected){
        if(!field.isSelected){
          field.isSelected = false;
          field.order='';
          field.sortDesc = 'n';
         
           fields.forEach(function(sortField){
          if(sortField.order === 2){
            sortField.order = 1;
          }
        });
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
            });
            field.order = (selectedCount > 0) ? 2 : 1;
            field.isSelected = true;        
        }   
      };

       $scope.restoreDefaults = function(){
         var pageSizeSettings = {key: 'pageLoadSize'};
          ApplicationSettingsService.resetApplicationSettings(pageSizeSettings).then(function(resp) { 
            LocalStorage.setPageSize($scope.defaultPageSize);
            $scope.resetPage();
            var sortOrderSettings = {key: 'sortPreferences'};
            ApplicationSettingsService.resetApplicationSettings(sortOrderSettings).then(function(resp) {
             LocalStorage.setDefaultSort([]);
             init();
            });
          });
       };

       $scope.savePreferences = function () {
        ApplicationSettingsService.updateNotificationPreferences($scope.defaultPreferences, null).then(function(data) {
          $scope.canEditNotifications = false;
          $scope.notificationPreferencesForm.$setPristine();
          var pageSizeSettings = {userid: '', key: 'pageLoadSize', value: $scope.pageSizes.selectedSize};
          LocalStorage.setPageSize($scope.pageSizes.selectedSize);
          ApplicationSettingsService.saveApplicationSettings(pageSizeSettings).then(function(resp) {

            $scope.pageSizeForm.$setPristine();
            //generate string that represents the sort order settings
            //format is one string with two sections
            //each section contains a three letter identifier, and four characters representing the field and desc/asc order
            //A better description of this exists on the BEK ecommerce wiki under the title: Default Sort String: Explaination
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
            ApplicationSettingsService.saveApplicationSettings(sortOrderSettings);
          });
        });
      };

      init();

    }]);
