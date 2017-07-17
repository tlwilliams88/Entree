(function() {
    'use strict';


    angular.module( 'ngTextTruncate', [] )


    .directive( "ngTextTruncate", [ "$compile", "ValidationServices", "CharBasedTruncation", "WordBasedTruncation",
    	function( $compile, ValidationServices, CharBasedTruncation, WordBasedTruncation ) {
        return {
            restrict: "A",
            scope: {
                text: "=ngTextTruncate",
                charsThreshold: "@ngTtCharsThreshold",
                wordsThreshold: "@ngTtWordsThreshold",
                customMoreLabel: "@ngTtMoreLabel",
                customLessLabel: "@ngTtLessLabel"
            },
            controller: function( $scope, $element, $attrs ) {
                $scope.toggleShow = function() {
                    $scope.open = !$scope.open;
                };

                $scope.useToggling = $attrs.ngTtNoToggling === undefined;
            },
            link: function( $scope, $element, $attrs ) {
                $scope.open = false;

                ValidationServices.failIfWrongThresholdConfig( $scope.charsThreshold, $scope.wordsThreshold );

                var CHARS_THRESHOLD = parseInt( $scope.charsThreshold );
                var WORDS_THRESHOLD = parseInt( $scope.wordsThreshold );

                $scope.$watch( "text", function() {
                    $element.empty();

                    if( CHARS_THRESHOLD ) {
                            if( $scope.text && CharBasedTruncation.truncationApplies( $scope, CHARS_THRESHOLD ) ) {
                                CharBasedTruncation.applyTruncation( CHARS_THRESHOLD, $scope, $element );

                            } else {
                                $element.append( $scope.text );
                            }

                    } else {

                        if( $scope.text && WordBasedTruncation.truncationApplies( $scope, WORDS_THRESHOLD ) ) {
                            WordBasedTruncation.applyTruncation( WORDS_THRESHOLD, $scope, $element );

                        } else {
                            $element.append( $scope.text );
                        }

                    }
                } );
            }
        };
    }] )



    .factory( "ValidationServices", function() {
        return {
            failIfWrongThresholdConfig: function( firstThreshold, secondThreshold ) {
                if( (! firstThreshold && ! secondThreshold) || (firstThreshold && secondThreshold) ) {
                    throw "You must specify one, and only one, type of Threshold (chars or words)";
                }
            }
        };
    })



    .factory( "CharBasedTruncation", [ "$compile", function( $compile ) {
        return {
            truncationApplies: function( $scope, Threshold ) {
                return $scope.text.length > Threshold;
            },

            applyTruncation: function( Threshold, $scope, $element ) {
                if( $scope.useToggling ) {
                    var el = angular.element(    "<span class='hand-pointer' ng-click='toggleShow()'>" +
                                                    $scope.text.substr( 0, Threshold ) +
                                                    "<span ng-show='!open'>...</span>" +
                                                    "<span class='btn-link ngTruncateToggleText hand-pointer' " +
                                                        "ng-click='toggleShow()'" +
                                                        "ng-show='!open'>" +
                                                        " " + ($scope.customMoreLabel ? $scope.customMoreLabel : "More") +
                                                    "</span>" +
                                                    "<span ng-show='open'>" +
                                                        $scope.text.substring( Threshold ) +
                                                        "<span class='btn-link ngTruncateToggleText hand-pointer'" +
                                                              "ng-click='toggleShow()'>" +
                                                            " " + ($scope.customLessLabel ? $scope.customLessLabel : "Less") +
                                                        "</span>" +
                                                    "</span>" +
                                                "</span>" );
                    $compile( el )( $scope );
                    $element.append( el );

                } else {
                    $element.append( $scope.text.substr( 0, Threshold ) + "..." );

                }
            }
        };
    }])



    .factory( "WordBasedTruncation", [ "$compile", function( $compile ) {
        var TEXTLENGTH_REGEX = /[^\.!\?]+[\.!\?]+/g;

        return {
            truncationApplies: function( $scope, Threshold ) {
                var text = $scope.text.match( TEXTLENGTH_REGEX ),
                    textLength = text != null && $scope.text.match( TEXTLENGTH_REGEX ).length && $scope.text.match( TEXTLENGTH_REGEX ).length > Threshold ? true : false;
                return textLength;
            },

            applyTruncation: function( Threshold, $scope, $element ) {
                var splitText = $scope.text.match( TEXTLENGTH_REGEX );
                if( $scope.useToggling ) {
                    var el = angular.element(    "<span class='hand-pointer' ng-click='toggleShow()'>" +
                                                    splitText.slice( 0, Threshold ).join( "" ) + "" +
                                                    "<span ng-show='!open'>...</span>" +
                                                    "<span class='btn-link ngTruncateToggleText hand-pointer' " +
                                                        "ng-show='!open'>" +
                                                        "" + ($scope.customMoreLabel ? $scope.customMoreLabel : "More") +
                                                    "</span>" +
                                                    "<span ng-show='open'>" +
                                                        splitText.slice( Threshold, splitText.length ).join( "" ) +
                                                        "<span class='btn-link ngTruncateToggleText hand-pointer'>" +
                                                            " " + ($scope.customLessLabel ? $scope.customLessLabel : "Less") +
                                                        "</span>" +
                                                    "</span>" +
                                                "</span>" );
                    $compile( el )( $scope );
                    $element.append( el );

                } else {
                    $element.append( splitText.slice( 0, Threshold ).join( "" ) + "..." );
                }
            }
        };
    }]);

}());
