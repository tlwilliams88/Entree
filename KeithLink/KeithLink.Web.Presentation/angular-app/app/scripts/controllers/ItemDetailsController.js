'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$state', '$modal', 'item', 'ProductService', 'AccessService', 'PricingService', 'LocalStorage', 'AnalyticsService',
    function ($scope, $state, $modal, item, ProductService, AccessService, PricingService, LocalStorage, AnalyticsService) {
    
    if(!item){
      $state.go('menu.home');
    }
    var originalItemNotes = item.notes;

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

    $scope.item = item;
    $scope.item.quantity = 1;

    $scope.item.orderHistoryKeys = Object.keys(item.orderhistory);

    $scope.canOrderItemInd = PricingService.canOrderItem(item);
    $scope.casePriceInd = PricingService.hasCasePrice(item);
    $scope.packagePriceInd = PricingService.hasPackagePrice(item);
    
    AnalyticsService.recordProductClick(LocalStorage.getCustomerNumber(), 
                                        LocalStorage.getBranchId(),
                                        $scope.item);

    AnalyticsService.recordViewDetail(LocalStorage.getCustomerNumber(), 
                                      LocalStorage.getBranchId(),
                                      $scope.item);

    if(!(item.is_specialty_catalog || item.isvalid === false)){
      ProductService.saveRecentlyViewedItem(item.itemnumber);
    }

    $scope.openNotesModal = function (item) {

      var modalInstance = $modal.open({
        templateUrl: 'views/modals/itemnotesmodal.html',
        controller: 'ItemNotesModalController',
        resolve: {
          item: function() {
            return angular.copy(item);
          }
        }
      });

      modalInstance.result.then(function(item) {
        $scope.item = item;
      });
    };

  }]);