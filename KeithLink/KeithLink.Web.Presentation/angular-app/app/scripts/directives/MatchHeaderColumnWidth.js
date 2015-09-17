'use strict';

angular.module('bekApp')
.directive('matchHeaderColumnWidth', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, elem, attr, ctrl) {
            var attributes = scope.$eval(attr.matchHeaderColumnWidth);
            var columnWidths = [];

            if (attributes.headingTable.length > 0) {
                // wait for the page to render
                $timeout(function () {
                    // set table-layout to fixed. this will
                    // keep the columns from adjusting the width
                    // when a scroll bar changes the size of the table
                    $(elem).css("table-layout", "fixed");
                    $("#" + attributes.headingTable).css("table-layout", "fixed");
                    var headingColumns = $("#" + attributes.headingTable + " thead > tr > th");

                    headingColumns.each(function (index, col) {
                        columnWidths.push(col.getBoundingClientRect().width);
                    });
                });
            }

            scope.$on('repeat-ended', function () {
                var body = elem.children("tbody");
                var firstRow = body.children("tr").first();
                var cells = firstRow.children("td");
                
                cells.each(function (index, col) {
                    $(col).css("width", columnWidths[index] + "px");
                });
            });
        }
    };
});