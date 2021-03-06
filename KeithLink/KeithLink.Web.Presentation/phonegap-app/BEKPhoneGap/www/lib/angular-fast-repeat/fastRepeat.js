/* globals angular */
angular.module('gc.fastRepeat', []).directive('fastRepeat', ['$compile', '$parse', '$animate', function ($compile, $parse, $animate, $rootScope) {
    'use strict';
    var $ = angular.element;

    var fastRepeatId = 0,
        showProfilingInfo = false,
        isGteAngular14 = /^(\d+\.\d+)\./.exec(angular.version.full)[1] > 1.3;

    // JSON.stringify replacer function which removes any keys that start with $$.
    // This prevents unnecessary updates when we watch a JSON stringified value.
    function JSONStripper(key, value) {
        if(key.slice && key.slice(0,2) == '$$') { return undefined; }
        return value;
    }

    function getTime() { // For profiling
        if(window.performance && window.performance.now) { return window.performance.now(); }
        else { return (new Date()).getTime(); }
    }

    return {
        restrict: 'A',
        transclude: 'element',
        priority: 1000,
        compile: function(tElement, tAttrs) {
            return function link(listScope, element, attrs, ctrl, transclude) {
                var repeatParts = attrs.fastRepeat.split(' in ');
                var repeatListName = repeatParts[1], repeatVarName = repeatParts[0];
                var getter = $parse(repeatListName); // getter(scope) should be the value of the list.
                var disableOpts = $parse(attrs.fastRepeatDisableOpts)(listScope);
                var currentRowEls = {};
                var t;
                var firstLabel;
                var isIE;
                listScope.currentIndex;

                // The rowTpl will be digested once -- want to make sure it has valid data for the first wasted digest.  Default to first row or {} if no rows
                var scope = listScope.$new();
                scope[repeatVarName] = getter(scope)[0] || {};
                scope.fastRepeatStatic = true; scope.fastRepeatDynamic = false;


                // Transclude the contents of the fast repeat.
                // This function is called for every row. It reuses the rowTpl and scope for each row.
                var rowTpl = transclude(scope, function(rowTpl, scope) {
                    if (isGteAngular14) {
                        $animate.enabled(rowTpl, false);
                    } else {
                        $animate.enabled(false, rowTpl);
                    }
                    renderLabels(scope);
                });

                // detect IE
                // returns isIE is true if IE or false, if browser is not IE
                function detectIE() {
                    var ua = window.navigator.userAgent;
                    var msie = ua.indexOf('MSIE ');//IE <11
                    var trident = ua.indexOf('Trident/');//IE 11
                    if (msie > 0) {
                      isIE = true;
                    } else if( trident > 0) {
                      isIE = true;
                    } else {
                      isIE = false;
                    }
                }
                detectIE();

                function renderLabels(scope){
                    if(listScope.selectedList && listScope.selectedList.name !== "Mandatory"){
                        if(scope.item.position == 1 && scope.item.label){
                            firstLabel = scope.item.label;
                            scope.item.label = '';
                        }
                        scope.fromRenderLabels = true;
                    }
                }

                // Create an offscreen div for the template
                var tplContainer = $("<div/>");
                $('body').append(tplContainer);
                scope.$on('$destroy', function() {
                    tplContainer.remove();
                    rowTpl.remove();
                });
                tplContainer.css({position: 'absolute', top: '-100%'});
                var elParent = element.parents().filter(function() {
                 return $(this).css('display') !== 'inline'; }).first();
                tplContainer.width(elParent.width());
                tplContainer.css({visibility: 'hidden'});

                tplContainer.append(rowTpl);

                var activeElement;

                var updateList = function(rowTpl, scope, forceUpdate) {
                    var deletedItem = false;
                    focusActiveElement(document.activeElement);
                    if(scope.item && scope.item.position == 1 && scope.item.label == ''){
                        scope.item.label = firstLabel;
                        scope.fromRenderLabels = false;
                    }
                    function render(item) {
                        scope[repeatVarName] = item;
                        scope.$digest();
                        rowTpl.attr('fast-repeat-id', item.$$fastRepeatId);
                        return rowTpl.clone();
                    }

                    scope.list = getter(scope);
                    // Generate ids if necessary and arrange in a hash map
                    var listByIds = {};
                    angular.forEach(scope.list, function(item) {
                        if(!item.$$fastRepeatId) {
                            if(item.id) {
                             item.$$fastRepeatId = item.id; }
                            else if(item._id) {
                             item.$$fastRepeatId = item._id; }
                            else {
                             item.$$fastRepeatId = ++fastRepeatId; }
                        }
                        listByIds[item.$$fastRepeatId] = item;
                    });

                    // Delete removed rows
                    angular.forEach(currentRowEls, function(row, id) {
                        if(!listByIds[id]) {
                            row.el.detach();
                        }
                    });
                    // Add/rearrange all rows
                    var previousEl = element;
                    angular.forEach(scope.list, function(item , index) {
                        var id = item.$$fastRepeatId;
                        var row=currentRowEls[id];

                        if(row && ((listScope.currentIndex === index 
                            && !(listScope.isDeletingItem || listScope.isChangingPage || listScope.allSelected))
                             || listScope.isDeletingItem || listScope.allSelected)) {
                            // We've already seen this one
                            if((!row.compiled 
                                && (forceUpdate || !angular.equals(row.copy, item)))
                                 || (row.compiled && row.item!==item)) {
                                // This item has not been compiled and it apparently has changed -- need to rerender
                                var newEl = render(item);
                                row.el.replaceWith(newEl);
                                row.el = newEl;
                                row.copy = angular.copy(item);
                                row.compiled = false;
                                row.item = item;
                                if(scope.list.indexOf(item) % 2 == 0) {
                                    row.el[0].children[0].className += ' even';
                                }
                            }
                        } else if(!row || listScope.isChangingPage) {
                            if(!disableOpts) {
                                row = {
                                    copy: angular.copy(item),
                                    item: item,
                                    el: render(item)
                                };
                                if(scope.list.indexOf(item) % 2 == 0) {
                                    row.el[0].children[0].className += ' even';
                                }
                            }else{
                                row = {
                                copy: angular.copy(item),
                                item: item,
                                el: $('<div/>'),
                                compiled: true
                            };

                                renderUnoptimized(item, function(newEl) {
                                    row.el.replaceWith(newEl);
                                    row.el=newEl;
                                });

                            }
                                currentRowEls[id] =  row; 
                                previousEl.after(row.el.last());
                                previousEl = row.el.last();
                        }


                        if(row && index % 2 == 0) {
                            row.el[0].children[0].className += ' even';
                        }
                        else{
                            row.el[0].children[0].className -= ' even';
                        }

                    });
                    listScope.isDeletingItem = false;
   
                };

                // Here is the main watch. Testing has shown that watching the stringified list can
                // save roughly 500ms per digest in certain cases.
                // JSONStripper is used to remove the $$fastRepeatId that we attach to the objects.
                var busy=false;
                listScope.$watch(function(scp){
                 return JSON.stringify(getter(scp));}, function(list) {                   
                    activeElement = document.activeElement;
                    tplContainer.width(elParent.width());
                    if(!scope.itemIconsActive){
                        scope.itemIconsActive = false;
                    }

                    if(busy) { return; }
                    busy=true;

                    if (showProfilingInfo) {
                        t = getTime();
                    }

                    // Rendering is done in a postDigest so that we are outside of the main digest cycle.
                    // This allows us to digest the individual row scope repeatedly without major hackery.
                    listScope.$$postDigest(function() {
                        tplContainer.width(elParent.width());
                        scope.$digest();

                        updateList(rowTpl, scope, null);
                        if (showProfilingInfo) {
                            t = getTime() - t;
                            console.log("Total time: ", t, "ms");
                            console.log("time per row: ", t/list.length);
                        }
                        busy=false;
                        listScope.isChangingPage = false;
                    });
                }, false);

                function renderRows() {
                    listScope.$$postDigest(function() {
                        tplContainer.width(elParent.width());
                        scope.$digest();
                        updateList(rowTpl, scope, true);
                    });
                }
                if(attrs.fastRepeatWatch) {
                    listScope.$watch(attrs.fastRepeatWatch, renderRows);
                }
                listScope.$on('fastRepeatForceRedraw', renderRows);

                function renderUnoptimized(item, cb) {
                    var newScope = scope.$new(false);

                    newScope[repeatVarName] = item;
                   newScope.fastRepeatStatic = false; newScope.fastRepeatDynamic = true;
                    var clone = transclude(newScope, function(clone) {
                        tplContainer.append(clone);
                    });

                    newScope.$$postDigest(function() {
                        cb(clone);
                        clone[0].children[0].className += " unoptimized"
                    });

                    newScope.$digest();

                    return newScope;
                }

                function focusActiveElement(element){
                    if(element && element.id === "fastRepeatDefaultId"){
                        activeElement = element;
                    }
                }

                function inputFocus(input){
                    listScope.selectedElement = input[0].querySelector('#defaultElement-' + listScope.currentIndex)
                    listScope.selectedElement.className += ' SelectedElement';
                    listScope.selectedElement.focus(function(){
                        $rootScope.$broadcast('changed');
                    });
                }

                var parentClickHandler = function parentClickHandler(evt) {
                    var $target = $(this);
                    focusActiveElement(document.activeElement);
                    if($target.parents().filter('[fast-repeat-id]').length) {
                        return; // This event wasn't meant for us
                    }
                    evt.stopPropagation();

                    var rowId = $target.attr('fast-repeat-id');
                    var item = currentRowEls[rowId].item;
                    listScope.currentIndex = scope.list.indexOf(item) + 1;

                    // Find index of clicked dom element in list of all children element of the row.
                    // -1 would indicate the row itself was clicked.
                    var elIndex = $target.find('*').index(evt.target);
                    var newScope = renderUnoptimized(item, function(clone) {
                        scope.itemIconsActive = true;
                        if(scope.list.indexOf(item) % 2 == 0){
                            $target.replaceWith(clone);
                            clone[0].children[0].className += ' even';
                        }else{
                            $target.replaceWith(clone);
                        }

                        clone[0].children[0].className += ' unoptimized';

                        if(listScope.currentIndex && evt.type == "focusin"){
                            inputFocus(clone);
                            activeElement = '';
                        }
                        
                        currentRowEls[rowId] = {
                            compiled: true,
                            el: clone,
                            item: item
                        };

                            if(elIndex >= 0) {
                                clone.find('*').eq(elIndex).trigger('hover');
                            } else {
                                clone.trigger('hover');
                            }
                    });

                    newScope.$digest();
                };


                element.parent().on('mouseenter focus', '[fast-repeat-id]', parentClickHandler);
                
                // Handle resizes
                var onResize = function() {
                    tplContainer.width(elParent.width());
                };

                var jqWindow = $(window);
                jqWindow.on('resize', onResize);
                scope.$on('$destroy', function() {
                    jqWindow.off('resize', onResize);
                    element.parent().off('mouseleave', '[fast-repeat-id]', parentClickHandler);
                });
            };
        },
    };
}]);