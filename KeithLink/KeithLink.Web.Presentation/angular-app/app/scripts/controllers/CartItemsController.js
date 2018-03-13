'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartItemsController', ['$scope', '$state', '$stateParams', '$filter', '$modal', '$q', 'ENV', 'Constants', 'LocalStorage',
   'CartService', 'OrderService', 'UtilityService', 'PricingService', 'DateService', 'changeOrders', 'originalBasket', 'criticalItemsLists', 'AnalyticsService',
    function($scope, $state, $stateParams, $filter, $modal, $q, ENV, Constants, LocalStorage, CartService, OrderService, UtilityService,
     PricingService, DateService, changeOrders, originalBasket, criticalItemsLists, AnalyticsService) {

    // redirect to url with correct ID as a param
    var basketId = originalBasket.id || originalBasket.ordernumber;
    if ($stateParams.cartId !== basketId.toString()) {
      $state.go('menu.cart.items', {cartId: basketId}, {location:'replace', inherit:false, notify: false});
    }

    // update cartHeaders in MenuController
    $scope.$parent.$parent.cartHeaders = CartService.cartHeaders;

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

    var watches = [];
    function onQuantityChange(newVal, oldVal) {
      var changedExpression = this.exp; // jshint ignore:line
      var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
      var item = $scope.currentCart.items[idx];
      if (item) {
        item.extPrice = PricingService.getPriceForItem(item);

        calculatePieces($scope.currentCart.items);
      }

      $scope.currentCart.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.currentCart.items);
    }
    function addItemWatches(startingIndex) {
      for (var i = startingIndex; i < $scope.currentCart.items.length; i++) {
        watches.push($scope.$watch('currentCart.items[' + i + '].quantity', onQuantityChange));
        watches.push($scope.$watch('currentCart.items[' + i + '].each', onQuantityChange));
      }
    }
    function clearItemWatches() {
      watches.forEach(function(watch) {
        watch();
      });
      watches = [];
    }

    $scope.sortBy = 'createddate'; // sort items in the order they were added to the cart
    $scope.sortOrder = false;
    CartService.updateNetworkStatus();
    $scope.isOffline = CartService.isOffline;
    $scope.cartContainsSpecialItems = CartService.cartContainsSpecialItems;
    $scope.carts = CartService.cartHeaders;
    $scope.shipDates = CartService.shipDates;
    $scope.changeOrders = OrderService.changeOrderHeaders;
    $scope.isChangeOrder = originalBasket.hasOwnProperty('ordernumber') ? true : false;
    if($scope.isChangeOrder){
      originalBasket.items =  OrderService.filterDeletedOrderItems(originalBasket);
    }
    $scope.currentCart = angular.copy(originalBasket);
    $scope.currentCart.isRenaming = false;
    $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);
    $scope.isMobile = ENV.mobileApp;
    $scope.invalidSelectedDate = false;
    $scope.$watch(function () {
      return CartService.isOffline;
    }, function (newVal, oldVal) {
      if (typeof newVal !== 'undefined') {
        $scope.isOffline = CartService.isOffline;
        $scope.resetSubmitDisableFlag(true);
      }
    });

    OrderService.getRecentlyOrderedUNFIItems().then(function(recentlyOrdered){
      if(recentlyOrdered){
        $scope.recentlyOrderedUnfiItems = recentlyOrdered.items;
      }
      else{
        $scope.recentlyOrderedUnfiItems = [];
      }
    });

    if (!$scope.isChangeOrder) {
      CartService.setActiveCart($scope.currentCart.id);
    }

    addItemWatches(0);

    // set default selected critical items list
    criticalItemsLists.forEach(function(list) {
      if (list.ismandatory) {
        $scope.mandatoryList = list;
      } else if (list.isreminder) {
        $scope.reminderList = list;
      }
    });

    // set mandatory and reminder lists
    // add property isMandatory for carts items that are on the mandatory list
    function setMandatoryAndReminder(cart){
      if ($scope.mandatoryList) {
        $scope.mandatoryList.active = true;
        if($scope.mandatoryList.items && $scope.mandatoryList.items.length > 0){
          cart.items.forEach(function(item){
            if($filter('filter')($scope.mandatoryList.items, {itemnumber: item.itemnumber}).length>0){
              item.isMandatory = true;
            }
          });
        }
      } else if ($scope.reminderList) {
        $scope.reminderList.active = true;
      } else {
        $scope.mandatoryList = {};
        $scope.reminderList = {};
      }
    }
    setMandatoryAndReminder($scope.currentCart);

    $scope.resetSubmitDisableFlag = function(checkForm){
      $scope.disableSubmitButtons = ((!$scope.currentCart.items || $scope.currentCart.items.length === 0) || $scope.isOffline || $scope.invalidSelectedDate);
    };
    $scope.resetSubmitDisableFlag(false);

    function selectNextCartId() {
      var redirectId;
      if ($scope.carts.length > 0) {
        redirectId = $scope.carts[0].id;
      } else if ($scope.changeOrders.length > 0) {
        redirectId = $scope.changeOrders[0].ordernumber;
      }
      return redirectId;
    }

    $scope.goToCart = function(cartId, isChangeOrder) {
      if (!cartId) {
        cartId = selectNextCartId();
      }
      $state.go('menu.cart.items', {cartId: cartId} );
    };

    $scope.cancelChanges = function() {
      var originalCart = angular.copy(originalBasket);
      originalCart.items.forEach(function(item) {
        item.extPrice = PricingService.getPriceForItem(item);
      });
      originalCart.subtotal = PricingService.getSubtotalForItemsWithPrice(originalCart.items);
      setMandatoryAndReminder(originalCart);
      $scope.currentCart = originalCart;
      $scope.resetSubmitDisableFlag(true);
      $scope.cartForm.$setPristine();
    };

    $scope.startEditCartName = function(cartName) {
      $scope.editCart = {};
      $scope.editCart.name = angular.copy(cartName);
      $scope.currentCart.isRenaming = true;
    };

    $scope.validateShipDate = function(shipDate){
      var cutoffDate = DateService.momentObject(shipDate.cutoffdatetime,'').format();
      var now = DateService.momentObject().tz('America/Chicago').format();

      $scope.invalidSelectedDate = (now > cutoffDate) ? true : false;
      if($scope.invalidSelectedDate){
        CartService.getShipDates().then(function(result){
          $scope.shipDates = result;
        });
      }
      $scope.resetSubmitDisableFlag(true);
      return $scope.invalidSelectedDate;
    };

    $scope.selectShipDate = function(shipDate) {

      if($scope.validateShipDate(shipDate)){
          return;
        }

      $scope.currentCart.requestedshipdate = shipDate.shipdate;
      $scope.selectedShipDate = shipDate;

      if($scope.cartForm){
        $scope.cartForm.$setDirty();
       }
    };

    $scope.sortByPrice = function(item) {
       if (item.price) {
         return item.price;
       } else {
        return item.each ? item.packageprice : item.caseprice;
       }
    };

    var invalidItemFound = false;
    var processingSaveCart = false;

    function invalidItemCheck(items) {
      var invalidItemFound = false;
      items.forEach(function(item){
        if (!item.extPrice && !(item.extPrice > 0) && !item.isMandatory && (item.status && item.status.toUpperCase() !== 'OUT OF STOCK') && (item.status && item.status.toUpperCase() !== 'DELETED')){
          invalidItemFound = true;
          $scope.displayMessage('error', 'Please delete or enter a quantity for item ' + item.itemnumber +' before saving or submitting the cart.');
        } else if(item.isMandatory && item.status && item.quantity == 0 && $scope.isChangeOrder && (item.status && item.status.toUpperCase() !== 'OUT OF STOCK' && item.status.toUpperCase() !== 'DELETED')){
          invalidItemFound = true;
          $scope.displayMessage('error', 'Please enter a quantity for item ' + item.itemnumber +' before saving or submitting the cart.');
        }
      });
      return invalidItemFound;
   }

    processingSaveCart = false;

    $scope.saveCart = function(cart) {

     var invalidItemFound =  invalidItemCheck(cart.items);
     if (!processingSaveCart && !invalidItemFound) {
        processingSaveCart = true;
        var updatedCart = angular.copy(cart);

        // delete items if quantity is 0 or price is 0
          updatedCart.items = $filter('filter')( updatedCart.items, function(item){
            return (item.quantity > 0 || (item.quantity === 0 && item.status && item.status.toUpperCase() === 'OUT OF STOCK')) && (PricingService.hasPackagePrice(item) || PricingService.hasCasePrice(item) || (item.price && PricingService.hasPrice(item.price)));
          });
          $scope.currentCart.items = updatedCart.items;
          $scope.currentCart.items = $filter('filter')($scope.currentCart.items, {status: '!Deleted'});
          $scope.resetSubmitDisableFlag(true);
          return CartService.updateCart(updatedCart).then(function(savedCart) {
            $scope.currentCart.isRenaming = false;
            $scope.sortBy = null;
            $scope.sortOrder = false;

            // clear and reapply all watches on item quantity and each fields
            clearItemWatches();
            $scope.currentCart = savedCart;
            addItemWatches(0);

            $scope.cartForm.$setPristine();
            $scope.displayMessage('success', 'Successfully saved cart ' + savedCart.name);
            return savedCart.id;
        }, function() {
          $scope.displayMessage('error', 'Error saving cart ' + updatedCart.name);
        }).finally(function() {
          processingSaveCart = false;
        });
      }
    };

    var processingSubmitOrder = false;
    $scope.submitOrder = function(cart) {
      var invalidItemFound =  invalidItemCheck(cart.items);
      if (!processingSaveCart && !invalidItemFound) {
        processingSubmitOrder = true;

        if($scope.validateShipDate($scope.selectedShipDate)){
        return;
        }

         CartService.isSubmitted(cart.id).then(function(hasBeenSubmitted){
          if(!hasBeenSubmitted){
            $scope.saveCart(cart)
            .then(CartService.submitOrder)
            .then(function(data) {
              $scope.setRecentlyOrderedUNFIItems(cart);
              var orderNumber = -1;
              var index;
              for (index in data.ordersReturned) {
                if (data.ordersReturned[index].catalogType === 'BEK')
                {
                  orderNumber = data.ordersReturned[index].ordernumber;
                }
              }

              var status = '';
              var message  = '';

              if(orderNumber === -1 ) {
                //no BEK items bought
                if (data.ordersReturned && data.ordersReturned.length && data.ordersReturned.length !== data.numberOfOrders) {
                  status = 'error';
                  message = 'One or more catalog orders failed. Please contact your DSR representative for assistance';
                } else {
                  status = 'success';
                  message  = 'Successfully submitted order.';
                }

                if (data.ordersReturned && data.ordersReturned[0] !== null) {
                  orderNumber = data.ordersReturned[0].ordernumber;
                } else {
                  orderNumber = null;
                }
              } else {
                //BEK oderNumber exists
                if (data.ordersReturned.length !== data.numberOfOrders) {
                  status = 'error';
                  message = 'We are unable to fulfill your special order items. Please contact your DSR representative for assistance';
                } else {
                  status = 'success';
                  message  = 'Successfully submitted order.';
                }
              }
            
            var customerName = $scope.selectedUserContext.customer.customerName;

            AnalyticsService.recordCheckout(cart, 
                                            Constants.checkoutSteps.SubmitCart, // step
                                            ""); //option

            AnalyticsService.recordTransaction(orderNumber, 
                                               cart,
                                               $scope.selectedUserContext.customer.customerNumber,
                                               $scope.selectedUserContext.customer.customerBranch);

            $state.go('menu.orderitems', { invoiceNumber: orderNumber });
            $scope.displayMessage(status, message);
            }, function(error) {
              $scope.displayMessage('error', 'Error submitting order.');
            }).finally(function() {
              processingSubmitOrder = false;
            });
          }
        });
      }
    };

    $scope.setRecentlyOrderedUNFIItems = function(cart){
      var itemsAdded = false;
      if(cart.items && cart.items.length > 0){
        var unfiItems = $filter('filter')(cart.items, {is_specialty_catalog: true});
        if(unfiItems.length > 0){
          unfiItems.forEach(function(unfiItem){
            if($filter('filter')($scope.recentlyOrderedUnfiItems, {itemnumber: unfiItem.itemnumber}).length === 0){
              $scope.recentlyOrderedUnfiItems.unshift(unfiItem);
              itemsAdded = true;
            }
          });
          if(itemsAdded){
            OrderService.UpdateRecentlyOrderedUNFIItems($scope.recentlyOrderedUnfiItems);
          }
        }
      }
    };

    $scope.renameCart = function (cartId, cartName) {
      var cart = angular.copy($scope.currentCart);
      cart.name = cartName;

      CartService.updateCart(cart).then(function(data) {
        $scope.currentCart.isRenaming = false;
        $scope.currentCart.name = cartName;
        $scope.displayMessage('success', 'Successfully renamed cart to ' + cartName + '.');
      }, function() {
        $scope.displayMessage('error', 'Error renaming cart.');
      });
    };

    $scope.createNewCart = function() {
      CartService.renameCart = true;
      CartService.createCart().then(function(newCart) {
        $state.go('menu.cart.items', {cartId: newCart.id});
        $scope.displayMessage('success', 'Successfully created new cart.');
      }, function() {
        $scope.displayMessage('error', 'Error creating new cart.');
      });
    };

    $scope.deleteCart = function(cart) {
      CartService.deleteCart(cart.id).then(function() {
        $scope.displayMessage('success', 'Successfully deleted cart.');
        $state.go('menu.order');
      }, function() {
        $scope.displayMessage('error', 'Error deleting cart.');
      });
    };

    $scope.deleteItem = function(item) {
        AnalyticsService.recordRemoveItem(
          item,
          LocalStorage.getCustomerNumber(),
          LocalStorage.getBranchId());

        var idx = $scope.currentCart.items.indexOf(item);
        $scope.currentCart.items.splice(idx, 1);
        $scope.resetSubmitDisableFlag(true);
        $scope.cartForm.$setDirty();
    };

    /************
    CHANGE ORDERS
    ************/

    var processingSaveChangeOrder = false;
    $scope.saveChangeOrder = function(order) {
      var invalidItemFound =  invalidItemCheck(order.items);
      if (!processingSaveChangeOrder && !invalidItemFound) {
        processingSaveChangeOrder = true;

        var changeOrder = angular.copy(order);

        changeOrder.items.forEach(function(item) {
          if (typeof item.quantity === 'string') {
            item.quantity = parseInt(item.quantity, 10);
          }
        });

        changeOrder.items = $filter('filter')( changeOrder.items, function(item){
          return (item.quantity > 0 || (item.quantity === 0 && item.status && item.status.toUpperCase() === 'OUT OF STOCK')) && (PricingService.hasPackagePrice(item) || PricingService.hasCasePrice(item) || (item.price && PricingService.hasPrice(item.price)));
        });

        return OrderService.updateOrder(changeOrder).then(function(order) {
          $scope.currentCart = order;
          $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);

          // recalculate ext price
          $scope.currentCart.items.forEach(function(item) {
            item.extPrice = PricingService.getPriceForItem(item);
          });
          $scope.cartForm.$setPristine();
          $scope.currentCart.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.currentCart.items);
          setMandatoryAndReminder($scope.currentCart);
          $scope.displayMessage('success', 'Successfully updated change order.');
          return order.ordernumber;
        }, function(error) {
          $scope.displayMessage('error', 'Error updating change order ' + order.invoicenumber + '.');
        }).finally(function() {
          processingSaveChangeOrder = false;
        });
      }
    };

    var processingResubmitOrder = false;
    $scope.resubmitOrder = function(order) {
      var invalidItemFound =  invalidItemCheck(order.items);

      if (!processingSaveChangeOrder && !invalidItemFound) {
        processingResubmitOrder = true;

        if($scope.validateShipDate($scope.selectedShipDate)){
          return;
        }

        //OrderService.isSubmitted(order.ordernumber).then(function(hasBeenSubmitted){
          //if(!hasBeenSubmitted){
            $scope.saveChangeOrder(order)
            .then(OrderService.resubmitOrder)
            .then(function(invoiceNumber) {
              $scope.setRecentlyOrderedUNFIItems(order);
              $scope.displayMessage('success', 'Successfully submitted change order.');

              AnalyticsService.recordCheckout(order, 
                                              Constants.checkoutSteps.SubmitChangeOrder, // step
                                              ''); //option

              $state.go('menu.orderitems', { invoiceNumber: invoiceNumber });
            }, function(error) {
              $scope.displayMessage('error', 'Error re-submitting order.');
            }).finally(function() {
              processingResubmitOrder = false;
            });
          //}
        //});
      }
    };

    var processingCancelOrder = false;
    $scope.cancelOrder = function(changeOrder) {
      if (!processingCancelOrder) {
        processingCancelOrder = true;

        OrderService.cancelOrder(changeOrder.commerceid).then(function() {
          var changeOrderFound = UtilityService.findObjectByField($scope.changeOrders, 'commerceid', changeOrder.commerceid);          var idx = $scope.changeOrders.indexOf(changeOrderFound);
          $scope.changeOrders.splice(idx, 1);
          $scope.goToCart();
          $scope.displayMessage('success', 'Successfully cancelled order ' + changeOrder.ordernumber + '.');
          $state.go('menu.order');
        }, function(error) {
          $scope.displayMessage('error', 'Error cancelling order ' + changeOrder.ordernumber + '.');
        }).finally(function() {
          processingCancelOrder = false;
        });
      }
    };

    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      var items = $scope.currentCart.items.length;

      if ($scope.currentCart && $scope.itemsToDisplay < items) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };

    $scope.openOrderImportModal = function () {

      var modalInstance = $modal.open({
        templateUrl: 'views/modals/orderimportmodal.html',
        controller: 'ImportModalController',
        resolve: {
          customListHeaders: ['ListService', function(ListService) {
            return ListService.getCustomListHeaders();
          }]
        }
      });
    };

    $scope.openCartExportModal = function(cartid) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/exportmodal.html',
        controller: 'ExportModalController',
        resolve: {
          location: function() {
            return {category:'Cart', action:'Export Cart'};
          },
          headerText: function () {
            return 'Orders';
          },
          exportMethod: function() {
            return CartService.exportCart;
          },
          exportConfig: function() {
            return CartService.getCartExportConfig(cartid);
          },
          exportParams: function() {
            return cartid;
          },
          exportType: function() {
             return Constants.exportType.cartItemsExport;
          }
        }
      });
    };

    /*************************
    REMINDER / MANDATORY ITEMS
    *************************/

    $scope.changeAllSelectedItems = function(items, selectAll) {
      angular.forEach(items, function(item, index) {
        item.isSelected = selectAll;
      });
    };

    $scope.addSelectedToCart = function(items) {

      // add watches to new items to update price
      var originalItemCount = $scope.currentCart.items.length;
      $scope.currentCart.items = $scope.currentCart.items.concat(items);
      addItemWatches(originalItemCount);
      $scope.resetSubmitDisableFlag(true);
      $scope.cartForm.$setDirty();
      if ($scope.reminderList.active) {
        $scope.reminderList.allSelected = false;
      }
      if ($scope.mandatoryList.active) {
        $scope.mandatoryList.allSelected = false;
      }
      // $scope.changeAllSelectedItems(items, false);

      if (items && items.length > 0) {
        // items = $filter('filter')(items, {isSelected: true});

        // set quantity
        items.forEach(function(item) {
          item.quantity = Math.ceil(item.parlevel - item.qtyInCart) || 1;

          if ($scope.isChangeOrder) {
            item.price = item.each ? item.packageprice : item.caseprice;
          }

        });

      } else {
        items.quantity = Math.ceil(items.parlevel - items.qtyInCart) || 1;

        if ($scope.isChangeOrder) {
          items.price = items.each ? items.packageprice : items.caseprice;
        }
      }
    };

    function calculatePieces(items){
      //total piece count for cart info box

      $scope.itemCount = $filter('filter')($scope.currentCart.items, {quantity: '!0'})
      $scope.piecesCount = 0;
        items.forEach(function(item){
          if(item.quantity){
            $scope.piecesCount = $scope.piecesCount + parseInt(item.quantity);
          }
        });
    }

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
          $scope.selectShipDate($scope.shipDates[0]);
          if($scope.isChangeOrder){
            $scope.saveChangeOrder($scope.currentCart);
          }
          else{
            $scope.saveCart($scope.currentCart);
          }
        }
        else{
          $state.go('menu.order');
        }
      });
    };

      if($scope.currentCart && !$scope.currentCart.requestedshipdate){
          $scope.selectShipDate($scope.shipDates[0]);
      }else{
            if(DateService.momentObject($scope.currentCart.requestedshipdate.slice(0,10),'') < DateService.momentObject($scope.shipDates[0].shipdate,'')) {
            $scope.openErrorMessageModal('The ship date requested for this order has expired. Select Cancel to return to the home screen without making changes. Select Accept to update to the next available ship date.');
          }
      }

    // on page load
    // if ($stateParams.renameCart === 'true' && !$scope.isChangeOrder) {
    //   $scope.startEditCartName(originalBasket.name);
    // }
    if (CartService.renameCart === true) {
      // console.log('rename cart');
      $scope.startEditCartName(originalBasket.name);
      CartService.renameCart = false;
    }

    AnalyticsService.recordCheckout($scope.currentCart, 
                                    Constants.checkoutSteps.ViewCart, // step
                                    ""); //option

  }]);
