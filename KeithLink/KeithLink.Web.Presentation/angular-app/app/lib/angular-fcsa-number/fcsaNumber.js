/*! angular-fcsa-number (version 1.5.3) 2014-10-17 */
/* https://github.com/FCSAmericaDev/angular-fcsa-number */

/*
using the directive, !!!properties must be strings
fcsa-number="{ 'minDigits': 6 }"

AVAILABLE OPTIONS
addMissingDecimals  boolean (requires maxDecimals to be set)
maxDecimals         number
min                 number
max                 number
preventInvalidInput boolean
maxDigits           number
minDigits           number
prepend             string
append              string
noFormat            boolean
*/

(function() {
  var fcsaNumberModule,
    __hasProp = {}.hasOwnProperty;

  fcsaNumberModule = angular.module('fcsa-number', []);

  fcsaNumberModule.directive('fcsaNumber', [
    'fcsaNumberConfig', function(fcsaNumberConfig) {
      var addCommasToInteger, addTrailingDecimals, controlKeys, defaultOptions, getOptions, hasMultipleDecimals, isNotControlKey, isNotValidOptionKey, isNotDigit, isNumber, makeIsValid, makeMaxDecimals, makeMinDigits, makeMaxDigits, makeMaxNumber, makeMinNumber, unformatNumber;
      defaultOptions = fcsaNumberConfig.defaultOptions;
      getOptions = function(attrs) {
        var option, options, value, _ref;
        options = angular.copy(defaultOptions);
        if (attrs.fcsaNumber != null) {
          _ref = JSON.parse(attrs.fcsaNumber.replace(/'/g, '"'));
          for (option in _ref) {
            if (!__hasProp.call(_ref, option)) continue;
            value = _ref[option];
            options[option] = value;
          }
        }
        return options;
      };
      isNumber = function(val) {
        return !isNaN(parseFloat(val)) && isFinite(val);
      };
      isNotDigit = function(which) {
        return which < 44 || which > 57 || which === 47;
      };
      controlKeys = [0, 8, 13, 40, 39, 38, 37];
      isNotControlKey = function(which) {
        return controlKeys.indexOf(which) === -1;
      };
      isNotValidOptionKey = function(which, options) {
        var decimalKeys = [46],
          negativeKeys = [45];

        var isNotValid = false;

        if (options.maxDecimals <= 0 && decimalKeys.indexOf(which) > -1) {
          isNotValid = true;
        }
        if (options.min >= 0 && negativeKeys.indexOf(which) > -1) {
          isNotValid = true;
        }
        return isNotValid;
      };
      hasMultipleDecimals = function(val) {
        return (val != null) && val.toString().split('.').length > 2;
      };
      makeMaxDecimals = function(maxDecimals) {
        var regexString, validRegex;
        if (maxDecimals > 0) {
          regexString = "^-?\\d*\\.?\\d{0," + maxDecimals + "}$";
        } else {
          regexString = "^-?\\d*$";
        }
        validRegex = new RegExp(regexString);
        return function(val) {
          return validRegex.test(val);
        };
      };
      makeMaxNumber = function(maxNumber) {
        return function(val, number) {
          return number <= maxNumber || !maxNumber;
        };
      };
      makeMinNumber = function(minNumber) {
        return function(val, number) {
          return number >= minNumber;
        };
      };
      makeMaxDigits = function(maxDigits) {
        var validRegex;
        validRegex = new RegExp("^-?\\d{0," + maxDigits + "}(\\.\\d*)?$");
        return function(val) {
          return validRegex.test(val);
        };
      };
      makeMinDigits = function(minDigits) {
        return function(val) {
          return (val + '').length >= minDigits;
        };
      };
      makeIsValid = function(options) {
        var validations;
        validations = [];
        if (options.maxDecimals != null) {
          validations.push(makeMaxDecimals(options.maxDecimals));
        }
        if (options.max != null) {
          validations.push(makeMaxNumber(options.max));
        }
        if (options.min != null) {
          validations.push(makeMinNumber(options.min));
        }
        if (options.maxDigits != null) {
          validations.push(makeMaxDigits(options.maxDigits));
        }
        if (options.minDigits != null) {
          validations.push(makeMinDigits(options.minDigits));
        }
        return function(val) {
          var i, number, _i, _ref;
          if (!isNumber(val)) {
            return false;
          }
          if (hasMultipleDecimals(val)) {
            return false;
          }
          number = Number(val);
          for (i = _i = 0, _ref = validations.length; 0 <= _ref ? _i < _ref : _i > _ref; i = 0 <= _ref ? ++_i : --_i) {
            if (!validations[i](val, number)) {
              return false;
            }
          }
          return true;
        };
      };
      addCommasToInteger = function(val) {
        var commas, decimals, wholeNumbers;
        decimals = (val.indexOf('.') == -1 || val.length - 1 == val.indexOf('.')) ? '' : val.replace(/^-?\d+(?=\.)/, '');
        wholeNumbers = val.replace(/(\.\d+)$/, '');
        commas = wholeNumbers.replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
        return "" + commas + decimals;
      };
      addTrailingDecimals = function(val, options) {
        var zeroesToAdd;
        if (options.addMissingDecimals && options.maxDecimals != null) {
          var decimalIndex = val.indexOf(".");
          if (decimalIndex === -1) { // decimal doesn't exist, add all decimals
            val = val + ".";
            zeroesToAdd = options.maxDecimals;
          } else if (decimalIndex === val.length - 1) { // decimal is the last character, add all decimals
            zeroesToAdd = options.maxDecimals;
          } else if (val.length - 1 - decimalIndex !== options.maxDecimals) { // decimal exists and is not the last character 
            zeroesToAdd = val.length - 1 - decimalIndex;
          }

          if (zeroesToAdd) {
            var zeroes = "";
            for (var i = 0; i < zeroesToAdd; i++) {
              zeroes = zeroes + "0";
            }
            val = val + zeroes;
          }
        }
        return val;
      };
      unformatNumber = function(val, options) {
        if (options.prepend != null) {
          val = val.replace(options.prepend, '');
        }
        if (options.append != null) {
          val = val.replace(options.append, '');
        }
        return val.replace(/,/g, '');
      };
      return {
        restrict: 'A',
        require: 'ngModel',
        // scope: {
        //   options: '@fcsaNumber'
        // },
        link: function(scope, elem, attrs, ngModelCtrl) {
          var isValid, options;
          options = getOptions(attrs);
          isValid = makeIsValid(options);
          ngModelCtrl.$parsers.unshift(function(viewVal) {
            var noCommasVal;
            noCommasVal = viewVal.replace(/,/g, '');
            noCommasVal = unformatNumber(noCommasVal, options);
            if (isValid(noCommasVal) || !noCommasVal) {
              ngModelCtrl.$setValidity('fcsaNumber', true);
              return noCommasVal;
            } else {
              ngModelCtrl.$setValidity('fcsaNumber', false);
              return void 0;
            }
          });
          ngModelCtrl.$formatters.push(function(val) {
            if ((options.nullDisplay != null) && (!val || val === '')) {
              return options.nullDisplay;
            }
            if ((val == null) || !isValid(val)) {
              return val;
            }
            ngModelCtrl.$setValidity('fcsaNumber', true);
            if (!options.noFormat) {
              val = addCommasToInteger(val.toString());
            }
            val = addTrailingDecimals(val.toString(), options);
            // add leading zero for decimals
            if (val.indexOf('.') === 0) {
              val = "0" + val;
            }
            
            if (options.prepend != null) {
              val = "" + options.prepend + val;
            }
            if (options.append != null) {
              val = "" + val + options.append;
            }
            return val;
          });
          elem.on('blur', function() {
            var formatter, viewValue, _i, _len, _ref;
            viewValue = ngModelCtrl.$modelValue;
            if ((viewValue == null) || !isValid(viewValue)) {
              return;
            }
            _ref = ngModelCtrl.$formatters;
            for (_i = 0, _len = _ref.length; _i < _len; _i++) {
              formatter = _ref[_i];
              viewValue = formatter(viewValue);
            }
            ngModelCtrl.$viewValue = viewValue;
            return ngModelCtrl.$render();
          });
          elem.on('focus', function() {
            var val;
            val = elem.val();
            elem.val(unformatNumber(val, options));
            return elem[0].select();
          });
          if (options.preventInvalidInput === true) {
            return elem.on('keypress', function(e) {
              if ((isNotDigit(e.which) && isNotControlKey(e.which)) || isNotValidOptionKey(e.which, options)) {
                return e.preventDefault();
              }
            });
          }
        }
      };
    }
  ]);

  fcsaNumberModule.provider('fcsaNumberConfig', function() {
    var _defaultOptions;
    _defaultOptions = {};
    this.setDefaultOptions = function(defaultOptions) {
      return _defaultOptions = defaultOptions;
    };
    this.$get = function() {
      return {
        defaultOptions: _defaultOptions
      };
    };
  });

}).call(this);
