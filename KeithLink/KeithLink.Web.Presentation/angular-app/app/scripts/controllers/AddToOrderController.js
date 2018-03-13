'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$rootScope', '$scope', '$state', '$modal', '$q', '$stateParams', '$filter', '$timeout', 'blockUI', 
   'lists', 'selectedList', 'selectedCart', 'Constants', 'CartService', 'ListService', 'OrderService', 'UtilityService', 'DateService', 'PricingService', 'ListPagingModel', 'LocalStorage', '$analytics', 'toaster', 'ENV', 'SessionService',
    function ($rootScope, $scope, $state, $modal, $q, $stateParams, $filter, $timeout, blockUI, lists, selectedList, selectedCart, Constants,
     CartService, ListService, OrderService, UtilityService, DateService, PricingService, ListPagingModel, LocalStorage, $analytics, toaster, ENV, SessionService) {

         $scope.$on('$stateChangeStart', function(event, toState, toParams, fromState, fromParams){
             var toCart = (toState.name == 'menu.cart.items' || fromState.name == 'menu.cart.items'),
                 toATOOrFromATO = (toState.name == 'menu.addtoorder.items' || fromState.name == 'menu.addtoorder.items'),
                 toATOAndFromATO = (toState.name == 'menu.addtoorder.items' && fromState.name == 'menu.addtoorder.items'),
                 toRegister = (toState.name == 'register'),
                 selectedCart = ($scope.selectedCart != null);

             if(!toCart &&
                 !toATOAndFromATO &&
                 !toRegister &&
                 !$scope.continueToCart &&
                 !$scope.orderCanceled &&
                 toATOOrFromATO &&
                 selectedCart
             ){

                 if(!$scope.tempCartName){
                   $scope.saveAndRetainQuantity();
                 } else {
                   if($scope.combinedItems){
                     $scope.selectedCart.items = $scope.combinedItems;
                   }
                   $scope.renameCart($scope.selectedCart.id, $scope.tempCartName);
                 }

             }
             guiders.hideAll();
         });

    var isMobile = UtilityService.isMobileDevice();
    var isMobileApp = ENV.mobileApp;

    $scope.recommendedItems = [
        {
          "ext_description":"",
          "caseaverage":0,
          "packageaverage":0,
          "detail":"Milk Whole Organic / 135779 / ORGANIC VALLEY / Dairy / 4 / 1 GAL",
          "productimages":[
            {
              "fileName":"135779-0.jpg",
              "url":"http://shopqa.benekeith.com/multidocsurl/ItemImage/GetFile/?ImageName=135779-0.jpg",
              "width":"724",
              "height":"483"
            }
          ],
          "isproprietary":false,
          "orderhistory":{},
          "inhistory":false,
          "itemnumber":"135779",
          "isvalid":true,
          "description":"Store 3338 Degrees",
          "nonstock":"N",
          "status1":"",
          "caseprice":"59.25",
          "caseonly":true,
          "unitprice":0.0,
          "packageprice":"0.00",
          "replacementitem":"000000",
          "replaceditem":"000000",
          "childnutrition":"N",
          "brand":"ORGANIC",
          "brand_extended_description":"ORGANIC VALLEY",
          "brand_control_label":"",
          "name":"Milk Whole Organic",
          "favorite":false,
          "sellsheet":"",
          "deviatedcost":"N",
          "temp_zone":"C",
          "categorycode":"60",
          "subcategorycode":"60",
          "categoryname":"Dairy",
          "class":"Dairy",
          "vendor_num":"047564",
          "upc":"00070744006767",
          "size":"1 GAL",
          "pack":"4",
          "packsize":"4 / 1 GAL",
          "cases":"3",
          "nutritional":{
            "diets":[],
            "brandowner":"",
            "countryoforigin":"",
            "grossweight":"",
            "handlinginstruction":"",
            "ingredients":"",
            "marketingmessage":"",
            "moreinformation":"",
            "servingsize":"",
            "servingsizeuom":"",
            "servingsperpack":"",
            "servingsuggestion":null,
            "shelf":"",
            "storagetemp":"",
            "unitmeasure":"",
            "unitspercase":"",
            "volume":"",
            "height":"",
            "length":"",
            "width":"",
            "nutrition":[],
            "diet":[],
            "allergens":{
              "freefrom":null,"maycontain":null,"contains":null
            }
          },
          "kosher":"N",
          "manufacturer_number":"61383",
          "manufacturer_name":"DEAN FOODS",
          "average_weight":34.5,
          "catalog_id":"fdf",
          "is_specialty_catalog":false,
          "specialtyitemcost":0.0,
          "marketing_name":null,
          "marketing_description":null,
          "marketing_brand":null,
          "marketing_manufacturer":null
        },
        {"ext_description":"","caseaverage":0,"packageaverage":0,"detail":"Chicken Breast Strip / 487303 / PIERCE CHICKEN / Center Of Plate / 2 / 5 LB","productimages":[{"fileName":"487303-0.PNG","url":"http://shopqa.benekeith.com/multidocsurl/ItemImage/GetFile/?ImageName=487303-0.PNG","width":"498","height":"509"}],"isproprietary":false,"orderhistory":{},"inhistory":false,"itemnumber":"487303","isvalid":true,"description":"Raw Tempura Battered","nonstock":"N","status1":"","caseprice":"67.50","caseonly":true,"unitprice":0.0,"packageprice":"0.00","replacementitem":"000000","replaceditem":"000000","childnutrition":"N","brand":"PIERCE","brand_extended_description":"PIERCE CHICKEN","brand_control_label":"","name":"Chicken Breast Strip","favorite":false,"sellsheet":"Y","deviatedcost":"N","temp_zone":"F","categorycode":"35","subcategorycode":"35","categoryname":"Poultry Value Added","class":"Center Of Plate","vendor_num":"002868","upc":"10075632113562","size":"5 LB","pack":"2","packsize":"2 / 5 LB","cases":"44","nutritional":{"diets":[],"brandowner":"PILGRIM'S PRIDE CORP","countryoforigin":"US","grossweight":"11.000","handlinginstruction":"KEEP FROZEN","ingredients":"CONTAINS UP TO 8% SOLUTION OF WATER, SALT, MODIFIED FOOD STARCH, SODIUM PHOSPHATES.  BATTERED WITH: WATER, YELLOW CORN FLOUR, BLEACHED WHEAT FLOUR, MODIFIED CORN STARCH, SALT, LEAVENING (SODIUM ALUMINUM PHOSPHATE, SODIUM BICARBONATE), DRIED WHEY, SPICES, GARLIC POWDER, ONION POWDER, DRIED WHOLE EGGS.  BREADED WITH: BLEACHED WHEAT FLOUR, LEAVENING (SODIUM BICARBONATE, SODIUM ALUMINUM PHOSPHATE, MONOCALCIUM PHOSPHATE), NONFAT DRY MILK, SALT, DRIED WHEY, FLAVOR (MALTODEXTRIN, SALT, SUGAR, SILICON DIOXIDE, GARLIC POWDER, SPICES, SOYBEAN OIL, NATURAL FLAVOR, EXTRACTIVES OF TURMERIC, HYDROLYZED CORN GLUTEN), GARLIC POWDER, ONION POWDER, TORULA YEAST.  BATTERED WITH: WATER, YELLOW CORN FLOUR, BLEACHED WHEAT FLOUR, MODIFIED CORN STARCH, SALT, LEAVENING (SODIUM ALUMINUM PHOSPHATE, SODIUM BICARBONATE), DRIED WHEY, SPICES, GARLIC POWDER, ONION POWDER, FLAVOR (MALTODEXTRIN, SALT, SUGAR, SILICON DIOXIDE, GARLIC POWDER, SPICES, SOYBEAN OIL, NATURAL FLAVOR, EXTRACTIVES OF TURMERIC, HYDROLYZED CORN GLUTEN).  BREADING SET IN VEGETABLE OIL.","marketingmessage":"READY TO COOK","moreinformation":"","servingsize":"3.000","servingsizeuom":"H87","servingsperpack":"34","servingsuggestion":null,"shelf":"365","storagetemp":"-10 FAH / 10 FAH","unitmeasure":"10.000","unitspercase":"0","volume":"0.609","height":"9.750","length":"11.750","width":"9.188","nutrition":[{"dailyvalue":"4","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"CA","nutrienttype":"Calcium"},{"dailyvalue":"7","measurementvalue":"22.000","measurementtypeid":"GRM","nutrienttypecode":"CHO-","nutrienttype":"Carbohydrate Total"},{"dailyvalue":"4","measurementvalue":"1.000","measurementtypeid":"GRM","nutrienttypecode":"FIBTSW","nutrienttype":"Dietary Fiber"},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"VITC-","nutrienttype":"Vitamin C"},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"VITA-","nutrienttype":"Vitamin A"},{"dailyvalue":"10","measurementvalue":"2.000","measurementtypeid":"GRM","nutrienttypecode":"FASAT","nutrienttype":"Saturated Fat"},{"dailyvalue":"18","measurementvalue":"55.000","measurementtypeid":"MGM","nutrienttypecode":"CHOL-","nutrienttype":"Cholesterol"},{"dailyvalue":"2","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"FE","nutrienttype":"Iron"},{"dailyvalue":"17","measurementvalue":"11.000","measurementtypeid":"GRM","nutrienttypecode":"FAT","nutrienttype":"Fat, total"},{"dailyvalue":"26","measurementvalue":"620.000","measurementtypeid":"MGM","nutrienttypecode":"NA","nutrienttype":"Sodium"}],"diet":[],"allergens":{"freefrom":["nuts","crustaceans","soybeans","fish","peanuts","sesame seeds"],"maycontain":[],"contains":["Milk","Eggs","Wheat"]}},"kosher":"N","manufacturer_number":"111356","manufacturer_name":"PILGRIMS PRIDE","average_weight":10.0,"catalog_id":"fsa","is_specialty_catalog":false,"specialtyitemcost":0.0,"marketing_name":null,"marketing_description":null,"marketing_brand":null,"marketing_manufacturer":null},
        {"ext_description":"","caseaverage":0,"packageaverage":0,"detail":"Bacon Slab 18/22 / 530581 / SMITHFIELD / Center Of Plate / 1 / 15 LB","productimages":[{"fileName":"530581-0.PNG","url":"http://shopqa.benekeith.com/multidocsurl/ItemImage/GetFile/?ImageName=530581-0.PNG","width":"691","height":"340"}],"isproprietary":false,"orderhistory":{"01/05/2018":1},"inhistory":false,"itemnumber":"530581","isvalid":true,"description":"300 Ct Avg Silver","nonstock":"N","status1":"G","caseprice":"82.25","caseonly":true,"unitprice":0.1811674008810572687224669604,"packageprice":"0.00","replacementitem":"000000","replaceditem":"000000","childnutrition":"N","brand":"SMITHFLD","brand_extended_description":"SMITHFIELD","brand_control_label":"","name":"Bacon Slab 18/22","favorite":false,"sellsheet":"Y","deviatedcost":"N","temp_zone":"F","categorycode":"34","subcategorycode":"34","categoryname":"Beef & Pork Value Added","class":"Center Of Plate","vendor_num":"010012","upc":"10070800039927","size":"15 LB","pack":"1","packsize":"1 / 15 LB","cases":"57","nutritional":{"diets":[],"brandowner":"Farmland Foods, Inc.","countryoforigin":"US","grossweight":"16.100","handlinginstruction":"Store and use per package instruction. Keep frozen","ingredients":"See package for additional ingrediants","marketingmessage":"Protein","moreinformation":"","servingsize":"0.000","servingsizeuom":"","servingsperpack":"454","servingsuggestion":null,"shelf":"90","storagetemp":"-10 FAH / 0 FAH","unitmeasure":"15.000","unitspercase":"0","volume":"0.469","height":"5.750","length":"13.750","width":"10.250","nutrition":[],"diet":[],"allergens":{"freefrom":null,"maycontain":null,"contains":null}},"kosher":"N","manufacturer_number":"7080003992","manufacturer_name":"SMITHFIELD FARMLAND","average_weight":15.0,"catalog_id":"fdf","is_specialty_catalog":false,"specialtyitemcost":0.0,"marketing_name":null,"marketing_description":null,"marketing_brand":null,"marketing_manufacturer":null},          
        {"ext_description":"","caseaverage":0,"packageaverage":0,"detail":"Ketchup Bottle Classic Squeeze / 661019 / HEINZ / Grocery / 16 / 14 OZ","productimages":[{"fileName":"661019-5.jpg","url":"http://shopqa.benekeith.com/multidocsurl/ItemImage/GetFile/?ImageName=661019-5.jpg","width":"300","height":"300"}],"isproprietary":false,"orderhistory":{},"inhistory":false,"itemnumber":"661019","isvalid":true,"description":"Red Plastic  Table Top","nonstock":"N","status1":"G","caseprice":"37.75","caseonly":true,"unitprice":0.0,"packageprice":"0.00","replacementitem":"000000","replaceditem":"000000","childnutrition":"N","brand":"HEINZ","brand_extended_description":"HEINZ","brand_control_label":"","name":"Ketchup Bottle Classic Squeeze","favorite":false,"sellsheet":"Y","deviatedcost":"N","temp_zone":"D","categorycode":"41","subcategorycode":"41","categoryname":"Condiments & Dressings","class":"Grocery","vendor_num":"052159","upc":"10013000514504","size":"14 OZ","pack":"16","packsize":"16 / 14 OZ","cases":"90","nutritional":{"diets":["kosher"],"brandowner":"","countryoforigin":"","grossweight":"","handlinginstruction":"","ingredients":"","marketingmessage":"","moreinformation":"","servingsize":"","servingsizeuom":"","servingsperpack":"","servingsuggestion":null,"shelf":"","storagetemp":"","unitmeasure":"","unitspercase":"","volume":"","height":"","length":"","width":"","nutrition":[],"diet":[{"diettype":"kosher","value":null}],"allergens":{"freefrom":[],"maycontain":[],"contains":[]}},"kosher":"N","manufacturer_number":"1300051450","manufacturer_name":"THE KRAFT HEINZ COMPANY","average_weight":14.01,"catalog_id":"fdf","is_specialty_catalog":false,"specialtyitemcost":0.0,"marketing_name":null,"marketing_description":null,"marketing_brand":null,"marketing_manufacturer":null},
        {"ext_description":"","caseaverage":0,"packageaverage":0,"detail":"Lettuce Green Leaf 12 Ct  K / 124015 / MARKON FIRST CROP / Produce / 1 / 12 CT","category":"Produce","productimages":[{"fileName":"124015-0.jpg","url":"https://shop.benekeith.com/multidocsurl/ItemImage/GetFile/?ImageName=124015-0.jpg","width":"300","height":"300"}],"isproprietary":false,"orderhistory":{},"inhistory":false,"itemnumber":"124015","isvalid":true,"description":"Us#1 Stor @34 F Salad & Garn","nonstock":"N","status1":"","caseprice":"16.63","caseonly":true,"unitprice":0.1014024390243902439024390244,"packageprice":"0.00","replacementitem":"000000","replaceditem":"000000","childnutrition":"N","brand":"MRKN/FC","brand_extended_description":"MARKON FIRST CROP","brand_control_label":"MC","name":"Lettuce Green Leaf 12 Ct  K","favorite":false,"sellsheet":"Y","deviatedcost":"N","temp_zone":"C","categorycode":"14","subcategorycode":"14","categoryname":"Lettuce","class":"Produce","vendor_num":"007115","upc":"00611628918105","size":"12 CT","pack":"1","packsize":"1 / 12 CT","cases":"54","nutritional":{"diets":["vegan"],"brandowner":"Markon Cooperative, Inc.","countryoforigin":"US","grossweight":"14.801","handlinginstruction":"Optimum storage is 34°-36° F/1°-2° C at high humidity with adequate circulation; refrigerate immediately; store in original packaging.","ingredients":"Green leaf lettuce","marketingmessage":"Available year-round from USA growers. Dependable yields and reliable cost per serving. Shipped in recyclable, wax- and staple-free cartons. Backed by Markon’s comprehensive 5-Star Food Safety® Program.","moreinformation":"www.markon.com","servingsize":"36.000","servingsizeuom":"GRM","servingsperpack":"164","servingsuggestion":null,"shelf":"14","storagetemp":"34 FAH / 36 FAH","unitmeasure":"12.000","unitspercase":"0","volume":"1.406","height":"9.000","length":"20.000","width":"13.500","nutrition":[{"dailyvalue":"2","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"CA","nutrienttype":"Calcium"},{"dailyvalue":"0","measurementvalue":"1.000","measurementtypeid":"GRM","nutrienttypecode":"CHO-","nutrienttype":"Carbohydrate Total"},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"MGM","nutrienttypecode":"CHOL-","nutrienttype":"Cholesterol"},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"GRM","nutrienttypecode":"FASAT","nutrienttype":"Saturated Fat"},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"GRM","nutrienttypecode":"FAT","nutrienttype":"Fat, total"},{"dailyvalue":"2","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"FE","nutrienttype":"Iron"},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"GRM","nutrienttypecode":"FIBTSW","nutrienttype":"Dietary Fiber"},{"dailyvalue":"0","measurementvalue":"10.000","measurementtypeid":"MGM","nutrienttypecode":"NA","nutrienttype":"Sodium"},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"GRM","nutrienttypecode":"SUGAD","nutrienttype":""},{"dailyvalue":"50","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"VITA-","nutrienttype":"Vitamin A"},{"dailyvalue":"6","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"VITC-","nutrienttype":"Vitamin C"}],"diet":[{"diettype":"vegan","value":null}],"allergens":{"freefrom":["crustaceans","eggs","fish","milk","nuts","peanuts","sesame seeds","sulphur dioxide","gluten(other)","soybeans","celery","mustard","shellfish","wheat"],"maycontain":[],"contains":[]}},"kosher":"N","manufacturer_number":"91810","manufacturer_name":"MARKON","average_weight":13.0,"catalog_id":"fdf","is_specialty_catalog":false,"specialtyitemcost":0.0,"marketing_name":"Lettuce Green Leaf 12 Ct  K","marketing_description":"Markon First Crop (MFC) Premium Green Leaf Lettuce is harvested only after inspection for maturity, flavor, texture, color, and weight to offer the highest possible yield; shipped in recyclable, wax-free cartons.\n\nMix MFC Green Leaf with grilled shrimp, RSS Pineapple Sections, and roasted cashews; dress with sesame-ginger vinaigrette. Sauté boneless chicken until browned; add RSS Proprietary Blend Orange Juice; simmer until fully reduced; chop chicken and toss with MFC Green Leaf Lettuce, sliced MFC Yellow Bell Peppers, and slivered almonds. Roast acorn squash with butter and brown sugar; slice and toss with MFC Green Leaf and shredded duck confit; dress with cranberry vinaigrette.\n\nAvailable year-round from USA growers. Dependable yields and reliable cost per serving. Shipped in recyclable, wax- and staple-free cartons. Backed by Markon’s comprehensive 5-Star Food Safety® Program.\n\nLearn more at www.markon.com.","marketing_brand":"MARKON FIRST CROP","marketing_manufacturer":"MARKON"},          
        {"ext_description":"","caseaverage":0,"packageaverage":0,"detail":"Chip Potato Reg Lays Lss / 699274 / LAYS / Grocery / 64 / 1.5 OZ","productimages":[{"fileName":"699274-1.jpg","url":"http://shopqa.benekeith.com/multidocsurl/ItemImage/GetFile/?ImageName=699274-1.jpg","width":"300","height":"300"},{"fileName":"699274-5.jpg","url":"http://shopqa.benekeith.com/multidocsurl/ItemImage/GetFile/?ImageName=699274-5.jpg","width":"300","height":"300"}],"isproprietary":false,"orderhistory":{},"inhistory":false,"itemnumber":"699274","isvalid":true,"description":"Gluten Free","nonstock":"N","status1":"G","caseprice":"53.70","caseonly":true,"unitprice":0.0,"packageprice":"0.00","replacementitem":"000000","replaceditem":"000000","childnutrition":"N","brand":"LAYS","brand_extended_description":"LAYS","brand_control_label":"","name":"Chip Potato Reg Lays Lss","favorite":false,"sellsheet":"Y","deviatedcost":"N","temp_zone":"D","categorycode":"42","subcategorycode":"42","categoryname":"Crackers,Dessert,Bakery","class":"Grocery","vendor_num":"004563","upc":"00028400443593","size":"1.5 OZ","pack":"64","packsize":"64 / 1.5 OZ","cases":"116","nutritional":{"diets":["kosher"],"brandowner":"Pepsico Inc.","countryoforigin":"US","grossweight":"7.760","handlinginstruction":"All products are code dated with \"guaranteed fresh by\" date on front of bag. Rotate product to insure fresh products. Destroy products that are beyond the \"guaranteed fresh by\" date. Store at room temperature out of direct sunlight.","ingredients":"Potatoes, Vegetable Oil (Sunflower, Corn, and/or Canola Oil), and Salt.","marketingmessage":"LAY'S Potato Chips are the classic potato chip. Light tasting, crisp and clean. They are a necessity on any grab and go snack display. The Large Single Serve size provides a little extra product to meet the requests of your consumers and to help drive incremental sales for your business.","moreinformation":"","servingsize":"1.000","servingsizeuom":"PH","servingsperpack":"0","servingsuggestion":null,"shelf":"70","storagetemp":"35 FA / 85 FA","unitmeasure":"96.000","unitspercase":"64","volume":"2.559","height":"13.563","length":"20.063","width":"16.250","nutrition":[{"dailyvalue":"24","measurementvalue":"16.000","measurementtypeid":"GM","nutrienttypecode":"FAT","nutrienttype":"Fat, total"},{"dailyvalue":"15","measurementvalue":"0.000","measurementtypeid":"MG","nutrienttypecode":"VITC","nutrienttype":""},{"dailyvalue":"4","measurementvalue":"0.000","measurementtypeid":"ME","nutrienttypecode":"FE","nutrienttype":"Iron"},{"dailyvalue":"10","measurementvalue":"250.000","measurementtypeid":"MG","nutrienttypecode":"NA","nutrienttype":"Sodium"},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"IU","nutrienttypecode":"VITA","nutrienttype":""},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"MG","nutrienttypecode":"CHOL-","nutrienttype":"Cholesterol"},{"dailyvalue":"11","measurementvalue":"2.000","measurementtypeid":"GM","nutrienttypecode":"FASAT","nutrienttype":"Saturated Fat"},{"dailyvalue":"7","measurementvalue":"2.000","measurementtypeid":"GM","nutrienttypecode":"FIB","nutrienttype":""},{"dailyvalue":"0","measurementvalue":"0.000","measurementtypeid":"MG","nutrienttypecode":"CA","nutrienttype":"Calcium"},{"dailyvalue":"8","measurementvalue":"23.000","measurementtypeid":"GM","nutrienttypecode":"CHO-","nutrienttype":"Carbohydrate Total"}],"diet":[{"diettype":"kosher","value":null}],"allergens":{"freefrom":[],"maycontain":[],"contains":[]}},"kosher":"N","manufacturer_number":"44359","manufacturer_name":"PEPSICO FRITO LAY","average_weight":6.0,"catalog_id":"fdf","is_specialty_catalog":false,"specialtyitemcost":0.0,"marketing_name":null,"marketing_description":null,"marketing_brand":null,"marketing_manufacturer":null},
        {"ext_description":"","caseaverage":0,"packageaverage":0,"detail":"Jalapeno Stuffed Fiesta / 370751 / ANCHOR / Frozen Food / 2 / 4 LB","productimages":[{"fileName":"370751-0.jpg","url":"http://shopqa.benekeith.com/multidocsurl/ItemImage/GetFile/?ImageName=370751-0.jpg","width":"569","height":"300"}],"isproprietary":false,"orderhistory":{},"inhistory":false,"itemnumber":"370751","isvalid":true,"description":"Tangy Chile Cream Chs 1216/Lb","nonstock":"N","status1":"G","caseprice":"71.80","caseonly":true,"unitprice":1.2821428571428571428571428571,"packageprice":"0.00","replacementitem":"000000","replaceditem":"000000","childnutrition":"N","brand":"ANCHOR","brand_extended_description":"ANCHOR","brand_control_label":"","name":"Jalapeno Stuffed Fiesta","favorite":false,"sellsheet":"Y","deviatedcost":"N","temp_zone":"F","categorycode":"23","subcategorycode":"23","categoryname":"Entrees-frozen","class":"Frozen Food","vendor_num":"006910","upc":"10072714107181","size":"4 LB","pack":"2","packsize":"2 / 4 LB","cases":"48","nutritional":{"diets":[],"brandowner":"McCain","countryoforigin":"US","grossweight":"8.950","handlinginstruction":"Best if used before 365 days from date of manufacture, when stored at 0°F/-18°C or below.","ingredients":"Red Jalapeno Peppers [Peppers, Water, Acetic Acid, Salt, Calcium Chloride, Sodium Benzoate (Preservative)], Cream Cheese (Pasteurized Milk and Cream, Cheese Culture, Salt, Stabilizers (Carob Bean, Xanthan and/or Guar Gums)), Water, Bleached Wheat Flour, Enriched Bleached Wheat Flour (Wheat Flour, Niacin, Ferrous Sulfate, Thiamine Mononitrate, Riboflavin, Folic Acid), Palm Oil, Modified Corn Starch, Enriched Wheat Flour (Wheat Flour, Niacin, Reduced Iron, Thiamine Mononitrate, Riboflavin, Folic Acid).  Contains 2% or less of  Apocarotenal (color), Carboxymethylcellulose Gum, Cheddar Cheese (Milk, Cheese Culture, Salt, Enzymes), Cream, Dextrose, Disodium Inosinate and Disodium Guanylate, Dried Garlic, Dried Onion, Flavored Granules [Corn Syrup, Enriched Bleached Wheat Flour (Wheat Flour, Niacin, Reduced Iron, Thiamine Mononitrate, Riboflavin, Folic Acid), Corn Cereal, Palm Oil, Jalapeno Powder, Natural and Artificial Flavor, Caramel Color, Yellow 5, Blue 1, Red 40, Yellow 6], Garlic Powder, Lactic Acid, Leavening (Sodium Acid Pyrophosphate, Sodium Bicarbonate, Monocalcium Phosphate), Maltodextrin, Modified Wheat Starch, Mono- and Diglycerides, Natural and Artificial Flavors, Nonfat Milk, Onion Powder, Paprika Oleoresin Color, Paprika, Salt, Silicon Dioxide, Sodium Alginate, Sodium Phosphate, Soybean Oil, Spices, Sugar, Tomato Powder, Wheat Starch, Whey, Yeast, Yellow Corn Flour.","marketingmessage":"Best if used before 365 days from date of manufacture, when stored at 0°F/-18°C or below.","moreinformation":"","servingsize":"65.000","servingsizeuom":"GRM","servingsperpack":"56","servingsuggestion":null,"shelf":"365","storagetemp":"-10 FAH / 0 FAH","unitmeasure":"","unitspercase":"2","volume":"0.410","height":"7.125","length":"11.875","width":"8.313","nutrition":[{"dailyvalue":"4","measurementvalue":"10.000","measurementtypeid":"MGM","nutrienttypecode":"CHOL-","nutrienttype":"Cholesterol"},{"dailyvalue":"4","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"FE","nutrienttype":"Iron"},{"dailyvalue":"6","measurementvalue":"17.000","measurementtypeid":"GRM","nutrienttypecode":"CHO-","nutrienttype":"Carbohydrate Total"},{"dailyvalue":"18","measurementvalue":"3.500","measurementtypeid":"GRM","nutrienttypecode":"FASAT","nutrienttype":"Saturated Fat"},{"dailyvalue":"3","measurementvalue":"1.000","measurementtypeid":"GRM","nutrienttypecode":"FIBTSW","nutrienttype":"Dietary Fiber"},{"dailyvalue":"20","measurementvalue":"470.000","measurementtypeid":"MGM","nutrienttypecode":"NA","nutrienttype":"Sodium"},{"dailyvalue":"2","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"CA","nutrienttype":"Calcium"},{"dailyvalue":"9","measurementvalue":"6.000","measurementtypeid":"GRM","nutrienttypecode":"FATNLEA","nutrienttype":"Total Fat"},{"dailyvalue":"6","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"VITA-","nutrienttype":"Vitamin A"},{"dailyvalue":"20","measurementvalue":"0.000","measurementtypeid":"","nutrienttypecode":"VITC-","nutrienttype":"Vitamin C"}],"diet":[],"allergens":{"freefrom":["fish","eggs","peanuts","crustaceans","sesame seeds","nuts","soybeans"],"maycontain":[],"contains":["Milk","Wheat"]}},"kosher":"N","manufacturer_number":"30010718","manufacturer_name":"MCCAIN FOODS","average_weight":8.0,"catalog_id":"fdf","is_specialty_catalog":false,"specialtyitemcost":0.0,"marketing_name":null,"marketing_description":null,"marketing_brand":null,"marketing_manufacturer":null}          
      ];

    // Tutorial -- Tutorial Ignored 09/25
    // var hideTutorial = LocalStorage.getHideTutorialAddToOrder() || isMobileApp || isMobile,
    //     runTutorial =  hideTutorial || isMobileApp || isMobile ? false : true;
    // 
    // function setHideTutorial(){
    //   LocalStorage.setHideTutorialAddToOrder(true);
    //   guiders.hideAll();
    // };

    // guiders.createGuider({
    //   id: "addtoorder_tutorial",
    //   title: "Refreshed Look And Feel",
    //   buttons: [{name: "Close", onclick: setHideTutorial}],
    //   description: "We changed the color and font of our screens to make everything easier to read.  <br/><br/> We hope these change help. <br/><br/> Also you may notice your list items have now been sorted by the simplified product categories.",
    //   overlay: true
    // })

    // if(runTutorial) {
    //   guiders.show('addtoorder_tutorial');
    // }

    function calculatePieces(items){
      //total piece count for cart info box
      $scope.piecesCount = 0;
        items.forEach(function(item){
          if(item.quantity){
            $scope.piecesCount = $scope.piecesCount + parseInt(item.quantity);
          }
        });
    }

    // redirect to url with correct parameters
    var basketId;
    if ($stateParams.cartId !== 'New') {
      basketId = selectedCart.id || selectedCart.ordernumber;
      selectedCart.items = OrderService.filterDeletedOrderItems(selectedCart);
      calculatePieces(selectedCart.items);
      $scope.origItemCount = selectedCart.items.length;

      if($stateParams.continueToCart){
      //continueToCart indicates the Proceed to Checkout button was pressed.
      $state.go('menu.cart.items', {cartId: basketId});
      }
    } else {
      basketId = 'New';
    }
    // if ($stateParams.cartId !== basketId.toString() || ($stateParams.cartId !== 'New' && $stateParams.listId !== selectedList.listid.toString())) {
    //   $state.go('menu.addtoorder.items', {cartId: basketId, listType: selectedList.type, listId: selectedList.listid, pageLoaded: true}, {location:'replace', inherit:false, notify: false});
    // }

    $scope.basketId = basketId;
    $scope.indexOfSDestroyedRow = '';
    $scope.destroyedOnField = '';
    $scope.useParLevel = false;
    $scope.visitedPages = [];
    $scope.canSaveCart = false;
    $scope.setOrderCanceled = false;

    $scope.removeRowHighlightParLevel = function(){
      $('.ATOrowHighlight').removeClass('ATOrowHighlight');
    };

    function onItemQuantityChanged(newVal, oldVal) {
      var changedExpression = this.exp; // jshint ignore:line
      var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
      var object = changedExpression.substr(0, changedExpression.indexOf('.'));
      var item = $scope[object].items[idx];

      if(newVal !== oldVal && item){
       refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
       calculatePieces($scope.combinedItems);
       $scope.itemCount = $scope.combinedItems.length;
      }
      if(item !== undefined){
        item.extPrice = PricingService.getPriceForItem(item);
      }
    }

    var watches = [];
    $scope.addItemWatches = function(startingIndex, endingIndex) {
      watches = [];
      endingIndex = ($scope.selectedList.itemCount < (startingIndex + (endingIndex - startingIndex))) ? $scope.selectedList.itemCount : endingIndex;
      for (var i = startingIndex; i < endingIndex; i++) {
        watches.push($scope.$watch('selectedList.items[' + i + '].quantity', onItemQuantityChanged));
        watches.push($scope.$watch('selectedList.items[' + i + '].each', onItemQuantityChanged));
      }
    };

    function clearItemWatches(watchers) {
      watchers.forEach(function(watch) {
        watch();
      });
      watchers = [];
    }

    var cartWatches = [];
    $scope.addCartWatches = function() {
      if($scope.selectedCart && $scope.selectedCart.items){
        for (var i = 0; i < $scope.selectedCart.items.length; i++) {
          cartWatches.push($scope.$watch('selectedCart.items[' + i + '].quantity', onItemQuantityChanged));
          cartWatches.push($scope.$watch('selectedCart.items[' + i + '].each', onItemQuantityChanged));
        }
      }
    };

    function getCombinedCartAndListItems(cartItems, listItems) {
      var items = angular.copy(cartItems.concat(listItems));
      // combine quantities if itemnumber is a duplicate
      var newCartItems = [];

      angular.forEach(items, function(item, index) {
        var duplicateItem = UtilityService.findObjectByField(newCartItems, 'itemnumber', item.itemnumber);
        item.quantity = parseInt(item.quantity, 10);
        if (duplicateItem) {
          if(item.quantity){
            duplicateItem.quantity = duplicateItem.quantity ? duplicateItem.quantity += item.quantity : item.quantity;
            duplicateItem.extPrice = PricingService.getPriceForItem(duplicateItem);
            duplicateItem.each = item.each;
            if(item.quantity > 0){
              duplicateItem.iscombinedquantity = true;
            }
          }
        } else {
          // do not double-count items in both the list and cart
          if (item.isHidden === true) {
            item.quantity = '';
          }
          newCartItems.push(item);
        }
      });
        // remove items with 0 quantity
        newCartItems = $filter('filter')(newCartItems, function(item) {
        return (item.quantity > 0 || (item.quantity == 0 && item.status && item.status.toUpperCase() === 'OUT OF STOCK'));
      });
      return newCartItems;
    }

     function flagDuplicateCartItems(cartItems, listItems) {
      angular.forEach(cartItems, function(cartItem) {
        var existingItem = UtilityService.findObjectByField(listItems, 'itemnumber', cartItem.itemnumber);
        if (existingItem) {
          cartItem.isHidden = true;
          // flag cart items that are in the list multiple times
          // hide those duplicate cart items from ui
          //$stateParams.listItems and testDuplicates will indicate whether or not the ATO page is being displayed after saving or after returning to the page from a state change.
          //testDuplicates will tell you if the totals for duplicate items have been combined or not
          var testDuplicates = 0;
          var lastDupeInDisplayedList = {};
          listItems.forEach(function(item){
            if(item.itemnumber === existingItem.itemnumber){
                testDuplicates = testDuplicates + 1;
                lastDupeInDisplayedList = item;
            }
          });

          if($scope.appendedItems && $scope.appendedItems.length > 0){
            var lastInstanceInAppendedItems = {};
            $scope.appendedItems.forEach(function(appendedItem){
              if(appendedItem.itemnumber === cartItem.itemnumber){
                lastInstanceInAppendedItems = appendedItem;
              }
            });
            if(lastInstanceInAppendedItems && lastInstanceInAppendedItems.name){
              var alreadyAccountedFor = false;
              listItems.forEach(function(listItem){
                if(listItem.itemnumber === lastInstanceInAppendedItems.itemnumber && listItem.listitemid !== lastInstanceInAppendedItems.listitemid && $filter('filter')($scope.appendedItems, {listitemid: listItem.listitemid}).length === 0){
                  alreadyAccountedFor = true;
                }
              });
              lastInstanceInAppendedItems.quantity = alreadyAccountedFor ? '' : cartItem.quantity;
              lastInstanceInAppendedItems.each = alreadyAccountedFor ? '' : cartItem.each;
            }
          }
          else{
          if(testDuplicates===0 && !$stateParams.listItems){
            existingItem.quantity = cartItem.quantity; // set list item quantity
          }
          else{
            if(!$stateParams.listItems || $scope.fromQuickAdd){
              $scope.selectedList.items.forEach(function(listItem, index){
              if(listItem.itemnumber === lastDupeInDisplayedList.itemnumber && listItem.listitemid !== lastDupeInDisplayedList.listitemid){
                $scope.selectedList.items[index].quantity = '';
              }
                if(listItem.listitemid === lastDupeInDisplayedList.listitemid){
                  $scope.selectedList.items[index].quantity = cartItem.quantity;
                  $scope.selectedList.items[index].each = cartItem.each;
                }
              });
            }
          }
        }
        } else {
          cartItem.isHidden = false;
        }
      });
      $scope.appendedItems = [];
      refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
    }

    $scope.blockUIAndChangePage = function(page){
      //Check if items for page already exist in the controller
      $scope.startingPoint = 0;
      $scope.endPoint = 0;
      var visited = $filter('filter')($scope.visitedPages, {page: page.currentPage});
       blockUI.start('Loading List...').then(function(){
        if(visited.length > 0){
          $timeout(function() {
            $scope.pageChanged(page, visited);
          }, 100);
        }
        else{
          $scope.pageChanged(page, visited);
        }
      });
    };


    $scope.setCartItemsDisplayFlag = function (){
      if($scope.selectedCart.items && $scope.selectedCart.items.length > 0){
        $scope.selectedCart.items.forEach(function(item){
          if($filter('filter')($scope.selectedList.items.slice($scope.startingPoint, $scope.endPoint), {itemnumber: item.itemnumber}).length > 0){
            item.isShown = true;
          }
          else{
            item.isShown = false;
          }
      });
      }
    };

  $scope.pagingPageSize = LocalStorage.getPageSize();
  $scope.pageChanged = function(page, visited) {
    $scope.currentPage = page.currentPage;
    $scope.startingPoint = ((page.currentPage - 1)*parseInt($scope.pagingPageSize));
    $scope.endPoint = $scope.startingPoint + parseInt($scope.pagingPageSize);
    $scope.setRange();

    if(!visited.length){
      listPagingModel.loadMoreData($scope.startingPoint, $scope.endPoint - 1, $scope.loadingResults, [], listPagingModel.filter);
    }
    else{
      var foundStartPoint = false;
      $scope.selectedList.items.forEach(function(item, index){
        if(item.listitemid && item.listitemid === visited[0].items[0].listitemid){
          $scope.startingPoint = index;
          $scope.endPoint = angular.copy($scope.startingPoint + parseInt($scope.pagingPageSize));
          foundStartPoint = true;
          $scope.addItemWatches($scope.startingPoint, $scope.endPoint);
          $scope.setCartItemsDisplayFlag();
        }
      });

      if(!foundStartPoint && visited[0].items.length > 0){
        appendListItems(visited[0].items);
      }
       blockUI.stop();
    }
  };

  $scope.setCurrentPageAfterRedirect = function(pageToSet){
      var visited = [];
      var page;
      if(!pageToSet && $stateParams.currentPage){
        page = $stateParams.currentPage;
      }
       else{
        page = pageToSet || 1;
      }
      $stateParams.currentPage = '';
      if($scope.visitedPages[0]){
        visited = $filter('filter')($scope.visitedPages, {page: page});
      }
      var selectedPage = {
        currentPage: page
      };
      $scope.pageChanged(selectedPage, visited);
  };

  $scope.setRange = function(){
    $scope.endPoint = $scope.endPoint;
    $scope.rangeStart = $scope.startingPoint + 1;
    $scope.rangeEnd = ($scope.endPoint > $scope.selectedList.itemCount) ? $scope.selectedList.itemCount : $scope.endPoint;
  };

    function setSelectedCart(cart) {
      $scope.selectedCart = cart;

      $scope.addCartWatches();
    }

    function setSelectedList(list) {
      $scope.selectedList = list;

      if($scope.selectedList.type == Constants.listType.Contract) {
        listPagingModel.filter = {
          field: 'delta',
          value: 'active'
        }
      }

      $scope.startingPoint = 0;
      $scope.visitedPages = [];
      $scope.visitedPages.push({page: 1, items: $scope.selectedList.items});
      $scope.endPoint = parseInt($scope.pagingPageSize);
      $scope.setCurrentPageAfterRedirect();
      $scope.setRange();

      SessionService.sourceProductList.push('ATO: ' + $scope.selectedList.name);

      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);

      if($stateParams.listItems){
       $stateParams.listItems.forEach(function(item){
         $scope.selectedList.items.forEach(function(selectedlistitem){
          if(item.listitemid === selectedlistitem.listitemid){
            selectedlistitem.quantity = item.quantity;
            selectedlistitem.onhand = item.onhand;
          }
         });
        });
       $stateParams.listItems = undefined;
      }
      $scope.setCartItemsDisplayFlag();
      getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items);
      $scope.addItemWatches($scope.startingPoint, $scope.endPoint);
    }

    function appendListItems(list) {
      $stateParams.listItems = $scope.selectedList.items;
      var originalItemCount = $scope.selectedList.items.length;
      var entireListReturned = (list.items.length === $scope.selectedList.itemCount) ? true : false;
      if(entireListReturned){
        $scope.visitedPages = [];
        var continueLoop = true;
        var numberOfPages = parseInt(list.items.length/$scope.pagingPageSize);
        for(var i = 1; continueLoop; i++){
          var start = (i -1) * $scope.pagingPageSize;
           continueLoop = (start + $scope.pagingPageSize) < ($scope.selectedList.itemCount -1);
          var end = (continueLoop) ? (start + $scope.pagingPageSize) : ($scope.selectedList.itemCount -1);
          $scope.visitedPages.push({page: i, items: list.items.slice(start,end)});
        }
      }
      else{
        $scope.selectedList.items = $scope.selectedList.items.concat(list.items);
        $scope.visitedPages.push({page: $scope.currentPage, items: list.items});
        $scope.visitedPages = $scope.visitedPages.sort(function(obj1, obj2){
          var sorterval1 = obj1.page;
          var sorterval2 = obj2.page;
          return sorterval1 - sorterval2;
        });
      }
      var firstItemOnCurrentpage = {};
      $scope.selectedList.items = [];
      $scope.visitedPages.forEach(function(page){
        $scope.selectedList.items = $scope.selectedList.items.concat(page.items);
        if($scope.currentPage === page.page){
          firstItemOnCurrentpage = page.items[0];
        }
      });

      $scope.selectedList.items.forEach(function(item, index){
        if(item.listitemid === firstItemOnCurrentpage.listitemid){
          $scope.startingPoint = index;
          $scope.endPoint = angular.copy(index + $scope.pagingPageSize);
          $scope.setCartItemsDisplayFlag();
        }
      });

      $scope.appendingList = true;
      $scope.appendedItems = list.items;
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
      getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items);
      $scope.addItemWatches($scope.startingPoint, $scope.endPoint);
    }

    function startLoading() {
      $scope.loadingResults = true;
    }

    function stopLoading() {
      $scope.loadingResults = false;
       blockUI.stop();
    }

    function init() {
      $scope.lists = lists;

      CartService.getShipDates().then(function(shipdates){

        if(shipdates && shipdates.length > 0){
          $scope.shipDates = shipdates;
          $scope.useParlevel = $stateParams.useParlevel === 'true' ? true : false;

          if (selectedCart) {
            setSelectedCart(selectedCart);
            $scope.isChangeOrder = selectedCart.hasOwnProperty('ordernumber') ? true : false;
            if(selectedCart.requestedshipdate && DateService.momentObject(selectedCart.requestedshipdate.slice(0,10),'') < DateService.momentObject($scope.shipDates[0].shipdate,'') && !$stateParams.pageLoaded){
               $scope.openErrorMessageModal('The ship date requested for this order has expired. Select Cancel to return to the home screen without making changes. Select Accept to update to the next available ship date.');
              selectedCart.requestedshipdate = $scope.shipDates[0].shipdate;
            }
          } else {
            // create new cart if no cart was selected
            $scope.generateNewCartForDisplay();
            $scope.allowSave = true;
            $scope.updateOrderClick(selectedList, $scope.selectedCart).then(function(resp){
                redirect(selectedList.listid, resp);
            });
          }

          $scope.visitedPages.push({page: 1, items: selectedList.items});
          setSelectedList(selectedList);
          $scope.setCartItemsDisplayFlag();
          if($stateParams.cartId !== 'New' && $stateParams.searchTerm){
            $scope.filterItems($stateParams.searchTerm);
          }
          if($stateParams.createdFromPrint){
            $stateParams.createdFromPrint = false;
            $scope.createdFromPrint = false;
            openPrintOptionsModal($scope.selectedList, $scope.selectedCart);
          }
          blockUI.stop();
        }
        else{
          alert('An error has occurred retrieving available shipping dates. Please contact your DSR for more information.');
          $state.go('menu.home');
          return;
        }
      });
    }

    if($stateParams.sortingParams && $stateParams.sortingParams.sort.length > 0){
      $scope.sort = $stateParams.sortingParams.sort;
    }
    else{
      $scope.sort = [{
      field: 'position',
      order: false
    }];
    }

    var listPagingModel = new ListPagingModel(
      selectedList.listid,
      selectedList.type,
      setSelectedList,
      appendListItems,
      startLoading,
      stopLoading,
      $scope.sort
    );

    /**********
    PAGING
    **********/

    $scope.postPageLoadInit = function(){
      $scope.clearedWhilePristine = false;
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
      getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items);
      $timeout(function() {
        $('#rowForFocus').find('input:first').focus();
      }, 100);
    };

    $scope.filterItems = function(searchTerm) {
      if($stateParams.searchTerm || $scope.addToOrderForm.$pristine){
        if($stateParams.searchTerm ){
          $scope.orderSearchTerm = $stateParams.searchTerm;
        }
        $scope.visitedPages = [];
        listPagingModel.filterListItems(searchTerm);
        $stateParams.searchTerm = '';
        clearItemWatches(watches);
      }
      else{
        $scope.fromFilterItems = true;
          $scope.saveAndRetainQuantity().then(function(resp){

            var continueSearch = resp;
            if(continueSearch){
              $scope.visitedPages = [];
              $scope.addToOrderForm.$setPristine();
              listPagingModel.filterListItems(searchTerm);
              clearItemWatches(watches);
            }
          });
      }
    };

    $scope.clearFilter = function(){
      $scope.orderSearchTerm = '';
      $stateParams.searchTerm = '';
      if($scope.addToOrderForm.$pristine){
        $scope.filterItems( $scope.orderSearchTerm);
        $scope.clearedWhilePristine = true;
      }
      else{
        $scope.saveAndRetainQuantity().then(function(resp){
            var clearSearchTerm = resp;
            if(clearSearchTerm){
              $scope.filterItems($scope.orderSearchTerm);
            }
        });
      }
      $scope.setCurrentPageAfterRedirect(1);
      $scope.orderSearchForm.$setPristine();
    };


      Mousetrap.bind(['alt+x'], function(e) {
        $scope.clearFilter();
      });

      Mousetrap.bind(['alt+s'], function(e) {
        $scope.updateOrderClick($scope.selectedList, $scope.selectedCart);
      });

      Mousetrap.bind(['alt+z'], function(e) {
        angular.element(orderSearchForm.searchBar).focus();
      });

      Mousetrap.bind(['alt+o'], function(e){
        angular.element(quickOrder).focus();{
          $scope.openQuickAddModal();
        }
      });

    $scope.confirmQuantity = function(type, item, value) {

      var pattern = /^([0-9])\1+$/; // repeating digits pattern
      if (value > 50 || pattern.test(value)) {
        var isConfirmed = window.confirm('Do you want to continue with entered quantity of ' + value + '?');
        if (!isConfirmed) {
          // clear input
          if(type==='quantity'){
            item.quantity = '';
          }
          else{
            item.onhand=null;
          }
        }
      }
    };

    $scope.openItemUsageSummaryModal = function(item, type) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/itemusagesummarymodal.html',
        controller: 'ItemUsageSummaryModalController',
        windowClass: 'color-background-modal',
        scope: $scope,
        resolve: {
          item: function() {
            return item;
          }
        }
      });
    };

    $scope.sortList = function(sortBy, sortOrder) {
      var r = ($scope.addToOrderForm.$pristine) ? true : confirm('Unsaved data will be lost. Do you wish to continue?');
      if(r){
        $scope.visitedPages = [];

        if (sortBy === $scope.sort[0].field) {
          sortOrder = (sortOrder === 'asc') ? 'desc' : 'asc';
        } else {
          sortOrder = 'asc';
        }
        $scope.sort = [{
          field: sortBy,
          order: sortOrder
        }];
        clearItemWatches(watches);
        $stateParams.listItems = undefined;
        listPagingModel.sortListItems($scope.sort);
        $scope.addToOrderForm.$setPristine();
      }
    };

    $scope.saveBeforeListChange = function(list, cart){

      if(($scope.addToOrderForm.$dirty || $scope.tempCartName) && $scope.addToOrderForm.$valid){
        $scope.saveAndRetainQuantity().then(function(){
          redirect(list, cart);
        });
      } else if($scope.addToOrderForm.$valid) {
        redirect(list, cart);
      } else {
        return;
      }

      calculatePieces(selectedCart.items);
    };

    function redirect(list, cart) {
        $scope.addToOrderForm.$setPristine();
        var cartId;
        if ($scope.isChangeOrder) {
            cartId = cart.ordernumber;
        } else {
            cartId = cart.id;
        }

        LocalStorage.setLastOrderList(list.listid, list.type, cartId);

        var searchTerm = '';
        if($scope.orderSearchTerm && $scope.creatingCart){
            searchTerm = $scope.orderSearchTerm;
        }

        var sameListItems= [];
        if($scope.selectedList && list.listid === $scope.selectedList.listid){
            sameListItems = $scope.selectedList.items;
        }
        else {
            sameListItems = undefined;
        }
        var continueToCart = $scope.continueToCart;

        blockUI.start('Loading List...').then(function(){
            $state.go('menu.addtoorder.items', {
              listId: list.listid,
              listType: list.type,
              cartId: cartId,
              useParlevel: $scope.useParlevel,
              continueToCart: continueToCart,
              listItems: sameListItems,
              searchTerm: searchTerm,
              createdFromPrint: $scope.createdFromPrint,
              createdFromQuickAdd: $scope.createdFromQuickAdd,
              currentPage: $scope.retainedPage
            });
        });
    }

    $scope.unsavedChangesConfirmation = function(){
      if($scope.addToOrderForm.$dirty){
          var r = confirm('Unsaved data will be lost. Do you wish to continue?');
          return r;
      }
      else{
        return true;
      }
    };

    /**********
    CARTS
    **********/

    $scope.startRenamingCart = function(cartName) {
      $scope.tempCartName = cartName;
      $scope.isRenaming = true;
    };

    $scope.renameCart = function(cartId, name) {
      var duplicateName = false;
      CartService.cartHeaders.forEach(function(header){
        if(name === header.name){
          duplicateName = (header.id === cartId) ? false : true;
            $scope.isRenaming = duplicateName;
        }
      });

      if(duplicateName){
        $scope.tempCartName = '';
        angular.element(renameCartForm.cartName).focus();
        toaster.pop('error', 'Error Saving Cart -- Cannot have two carts with the same name. Please rename this cart');
        $timeout(function() {
          angular.element(renameCartForm.cartName).focus();
        }, 100);
      }
      else{
        $scope.addToOrderForm.$setDirty();
        if (cartId === 'New') {
        // don't need to call the backend function for new cart
        $scope.selectedCart.name = name;
        $scope.isRenaming = false;
        CartService.renameCart = false;
        $scope.updateOrderClick($scope.selectedList, $scope.selectedCart);
      } else {
        // call backend to update cart
        var cart = angular.copy($scope.selectedCart);
        cart.name = name;
        CartService.updateCart(cart, null, $scope.selectedList).then(function(updatedCart) {
          $scope.selectedCart.name = updatedCart.name;
          $scope.isRenaming = false;
          CartService.renameCart = false;
          $scope.tempCartName = '';
          if($scope.continueToCart) {
            return $scope.updateOrderClick($scope.selectedList, $scope.selectedCart);
          } else {
            $scope.saveAndRetainQuantity();
          }
        });
      }
    }
      $('#rowForFocus').find('input:first').focus();
    };

    $scope.generateNewCartForDisplay = function() {
      var cart = {};
          cart.items = [];
          cart.id = 'New';
          cart.requestedshipdate = $scope.shipDates[0].shipdate;
      $scope.selectedCart = cart;
      $scope.isChangeOrder = false;
      $scope.startRenamingCart($scope.selectedCart.name);
    };

    /**********
    FORM EVENTS
    **********/



    var processingUpdateCart = false;
    function updateCart(cart) {
      if (!processingUpdateCart && cart.items) {
        processingUpdateCart = true;
        return CartService.updateCart(cart, null, selectedList).then(function(updatedCart) {
          setSelectedCart(updatedCart);
          $scope.setCartItemsDisplayFlag();
          flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
          if($scope.addToOrderForm){
            $scope.addToOrderForm.$setPristine();
          }


          var newItemCount = updatedCart.items.length - $scope.origItemCount;
          $scope.origItemCount = updatedCart.items.length;
           processingUpdateCart = false;
           return updatedCart;
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function() {
          processingUpdateCart = false;
          if($scope.continueToCart){
              $state.go('menu.cart.items', {cartId: basketId});
          }
        });
      }
      else{
         var deferred = $q.defer();
          deferred.resolve(false);
          return deferred.promise;
      }
    }


    function createNewCart(items, shipDate, name) {
      if(!$scope.newCartCreated && !processingSaveCart){
        $analytics.eventTrack('Create Order', {  category: 'Orders', label: 'From List' });
        var processingSaveCart = true;
        return CartService.createCart(items, shipDate, name).then(function(cart) {
            CartService.getCartHeaders().finally(function(cartHeaders) {
              $scope.loadingCarts = false;
              $scope.carts = CartService.cartHeaders;
            });
          $scope.addToOrderForm.$setPristine();
          $scope.retainedPage = $scope.currentPage;
          $scope.newCartCreated = true;

          return cart;
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function(){
          processingSaveCart = false;
        });
      }
    }

    /*******************************

      Cancel Changes And Delete Cart

    ********************************/

    $scope.cancelChanges = function(cartid) {
      $scope.orderCanceled = true;

      if($scope.selectedCart.items.length) {
        $scope.selectedList.items.forEach(function(listitem){
          var itemInCart = $filter('filter')($scope.selectedCart.items, {itemnumber: listitem.itemnumber, each: listitem.each})[0];
          var itemInOtherCartItems = $filter('filter')($scope.filteredCartItems, {itemnumber: listitem.itemnumber, each: listitem.each});

          if(itemInCart && !itemInOtherCartItems.length) {
            var duplicateItem = $filter('filter')($scope.selectedList.items, {itemnumber: itemInCart.itemnumber, each: itemInCart.each});

            if(duplicateItem.length > 1) {
              var lastDuplicateItemIdx = duplicateItem.length - 1;
              $scope.lastDuplicateItem = duplicateItem[lastDuplicateItemIdx];
              duplicateItem.pop();

              duplicateItem.forEach(function(duplicateitem){
                duplicateitem.quantity = '';
                duplicateitem.extPrice = 0.00;
              });

              $scope.lastDuplicateItem.quantity = itemInCart.quantity;
              $scope.lastDuplicateItem.extprice = PricingService.getPriceForItem($scope.lastDuplicateItem);
              $scope.lastDuplicateItem.each = itemInCart.each;
            } else {
              listitem.quantity = itemInCart.quantity;
              listitem.extprice = PricingService.getPriceForItem(listitem);
              listitem.each = itemInCart.each;
            }

          } else {
            listitem.quantity = '';
            listitem.extPrice = 0.00;
          }

        });
      } else {

        $scope.selectedList.items.forEach(function(listitem){
          if(listitem.quantity > 0) {
            listitem.quantity = '';
            listitem.extPrice = 0.00;
          }
        });

      }
    };

    $scope.deleteCart = function(cartid) {
      var cartguid = [];
      $scope.orderCanceled = true;
      cartguid.push(cartid);

      CartService.deleteMultipleCarts(cartguid).then(function() {
        $scope.displayMessage('success', 'Successfully deleted cart.');
        $state.go('menu.home');
      }, function() {
        $scope.displayMessage('error', 'Error deleting cart.');
      });

    };

    var processingSaveChangeOrder = false;
    function updateChangeOrder(order) {
      if (!processingSaveChangeOrder) {
        processingSaveChangeOrder = true;

        return OrderService.updateOrder(order, null, selectedList.listid).then(function(cart) {
          setSelectedCart(cart);
          $scope.setCartItemsDisplayFlag();
          flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
          refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
          if($scope.addToOrderForm){
            $scope.addToOrderForm.$setPristine();
          }

          var newItemCount = cart.items.length - $scope.origItemCount;
          $scope.origItemCount = cart.items.length;

          if(newItemCount > 0){
            $scope.displayMessage('success', 'Successfully added ' + newItemCount + ' Items to Order # ' + order.invoicenumber + '.');
          }else if(newItemCount < 0){
            $scope.displayMessage('success', 'Successfully removed ' + Math.abs(newItemCount) + ' Items from Order # ' + order.invoicenumber + '.');
          }
          else{
            $scope.displayMessage('success', 'Successfully Saved Order ' + order.invoicenumber + '.');
          }

          processingSaveChangeOrder = false;
          return cart;
        }, function() {
          $scope.displayMessage('error', 'Error adding items to change order.');
        }).finally(function() {
          processingSaveChangeOrder = false;
          if($scope.continueToCart){
            $state.go('menu.cart.items', {cartId: order.ordernumber});
          }
        });
      }
       else{
         var deferred = $q.defer();
          deferred.resolve(false);
          return deferred.promise;
      }
    }

    $scope.isRedirecting = function(resp){
      if(resp === 'renamingCart'){
        return true;
      }
      else if(resp.message && resp.message === 'Creating cart...'){
        redirect($scope.selectedList.listid, resp);
        return true;
      }
      else{
        return false;
      }
    };

    $scope.saveAndContinue = function(){
      $scope.continueToCart = true;

      if($scope.tempCartName){
        $scope.renameCart($scope.selectedCart.id, $scope.tempCartName);
      } else {
        return $scope.updateOrderClick($scope.selectedList, $scope.selectedCart);
      }

    };

    //Function includes support for saving items while filtering and saving cart when changing ship date
    $scope.saveAndRetainQuantity = function(noParentFunction) {
      if($scope.selectedList && $scope.selectedList.items){
        $stateParams.listItems = $scope.selectedList.items;
      }
      if($scope.addToOrderForm.$invalid){
        return false;
      } else {
        if($scope.selectedCart && $scope.selectedCart.id === 'New'){
          $scope.creatingCart = true;
        }
        if($scope.tempCartName){
          $scope.renameCart($scope.selectedCart.id, $scope.tempCartName);
          var deferred = $q.defer();
          deferred.resolve('renamingCart');
          return deferred.promise;
        }
        else {
          if($scope.selectedCart && $scope.selectedCart.subtotal === 0){
            $scope.addToOrderForm.$setDirty();
          }
          if($scope.selectedList && $scope.selectedCart) {
            return $scope.updateOrderClick($scope.selectedList, $scope.selectedCart).then(function(resp){

            if(noParentFunction){
              $scope.isRedirecting(resp);
            }
            return resp;
            });
          }
        }
      $scope.addToOrderForm.$setPristine();
      }
    };

    $scope.updateOrderClick = function(list, cart) {
      clearItemWatches(cartWatches);
      var cartItems = getCombinedCartAndListItems(cart.items, list.items);
      UtilityService.deleteFieldFromObjects(cartItems, ['listitemid']);

      var updatedCart = angular.copy(cart);
      updatedCart.items = cartItems;

      var invalidItemFound = false;

      updatedCart.items.forEach(function(cartitem){
        if (!cartitem.extPrice && !(cartitem.extPrice > 0) && !((cartitem.quantity === 0 || cartitem.quantity == '') && cartitem.status && cartitem.status.toUpperCase() === 'OUT OF STOCK')){
          invalidItemFound = true;
          $scope.displayMessage('error', 'Cannot create cart. Item ' + cartitem.itemnumber +' is invalid.  Please contact DSR for more information.');
        }
      });

      if (invalidItemFound){
        var deferred = $q.defer();
        deferred.resolve(invalidItemFound);
        return deferred.promise;
      }

      if ((cartItems && cartItems.length > 0) || ($scope.addToOrderForm && $scope.addToOrderForm.$dirty) || $scope.allowSave){
        if ($scope.isChangeOrder) {
         return updateChangeOrder(updatedCart);
        } else {
          if (updatedCart && updatedCart.id && updatedCart.id !== 'New') {
            return updateCart(updatedCart);

          } else {
            return createNewCart(cartItems, updatedCart.requestedshipdate, updatedCart.name);
          }
        }
      }
      else{
        var deferred = $q.defer();
        deferred.resolve(false);
        return deferred.promise;
      }
    };

    function refreshSubtotal(cartItems, listItems) {
      $scope.combinedItems = getCombinedCartAndListItems(cartItems, listItems);
      $scope.selectedCart.subtotal = PricingService.getSubtotalForItems($scope.combinedItems);
      return $scope.selectedCart.subtotal;
    }

    // update quantity from on hand amount and par level
    $scope.onItemOnHandAmountChanged = function(item) {

      if (!isNaN(item.onhand)) {
        if(item.onhand < 0){
          item.onhand = 0;
        }
        var quantity = Math.ceil(item.parlevel - item.onhand);
        if (quantity > 0) {
          item.quantity = quantity;
        } else if(item.quantity > 0 && (item.onhand.toString() === '0' || item.onhand === '')) {
          return;
        } else{
          item.quantity = 0;
        }
      }
    };

    $scope.saveBeforeQuickAdd = function(){
      if($scope.addToOrderForm.$dirty){
        $scope.saveAndRetainQuantity();
      }
      $scope.openQuickAddModal();
    };

    $scope.openQuickAddModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/cartquickaddmodal.html',
        controller: 'CartQuickAddModalController',
        backdrop:'static',

        resolve: {
          cart: function() {
            return $scope.selectedCart;
          }
        }
      });
    };

    $scope.$on('QuickAddUpdate', function(event, origCartItems, newItems) {
      $scope.fromQuickAdd = true;
      newItems.forEach(function(item){
        item.extPrice = PricingService.getPriceForItem(item);
      });
      if(newItems){
        $scope.selectedCart.items = origCartItems.concat(newItems);
      }
      $scope.saveAndRetainQuantity().then(function(resp){
        $scope.selectedCart = resp;
        refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
        calculatePieces($scope.selectedCart.items);
        $scope.fromQuickAdd = false;
      });
    });

    $scope.saveBeforePrint = function(){
      if($scope.addToOrderForm.$pristine && $scope.selectedCart.id !== 'New'){
        openPrintOptionsModal($scope.selectedList, $scope.selectedCart);
      }
      else{
        if($scope.selectedCart.id === 'New'){
        $scope.createdFromPrint = true;
        $scope.addToOrderForm.$setDirty();
      }
      $scope.saveAndRetainQuantity().then(function(resp){
        if($scope.isRedirecting(resp)){
          //do nothing
        }
        else{
          openPrintOptionsModal($scope.selectedList, $scope.selectedCart);
        }
      });
      }
    };

    function openPrintOptionsModal(list, cart) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/printoptionsmodal.html',
        controller: 'PrintOptionsModalController',
        scope: $scope,
        resolve: {
          list: function() {
            return list;
          },
          cart: function() {
            return cart;
          },
          pagingModelOptions: function() {
            return {
              sort: $scope.sort,
              terms: $scope.orderSearchTerm
            };
          },
          contractFilter: function() {
            return {
              filter: null
            };
          }
        }
      });
    };

    $scope.openErrorMessageModal = function(message) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/errormessagemodal.html',
        controller: 'ErrorMessageModalController',
        scope: $scope,
        backdrop:'static',
        resolve: {
          message: function() {
            return message;
          }
          }
      });

      modalInstance.result.then(function(resp) {
        if(resp){
          selectedCart.requestedshipdate = $scope.shipDates[0].shipdate;
          $scope.updateOrderClick($scope.selectedList, $scope.selectedCart).then(function(resp){
            $scope.isRedirecting(resp);
          });
        }
        else{
          $state.go('menu.home');
          }
      });
    };

    init();

  }]);
