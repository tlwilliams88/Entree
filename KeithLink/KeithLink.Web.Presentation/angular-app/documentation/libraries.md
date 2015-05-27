# 3rd Party Libraries

## Angular UI Block

[Angular UI Block](https://github.com/McNull/angular-block-ui) is used for the full-screen loading overlay throughout the site.

The overlay is activated for the following requests:

- All requests that have a ```message``` param
- ```/authen``` request
- Submit payments request
- All ```PUT``` requests except for the update to set the active cart
- All ```DELETE``` requests

For an example of how to set the ```message``` param, see ```resubmitOrder``` in the ```OrderService``` or ```getUserProfile``` in the ```UserProfileService```

## Angular Carousel

[Angular Carousel](http://blog.revolunet.com/angular-carousel/) is used for the slideshow carousels on the home and catalog pages when viewing mobile.

**WARNING:** This library was modified to fix and issue where swiping back on the carousel would open the mobile sidebar menu.

## Angular DragDrop

[Angular DragDrop](https://github.com/codef0rmer/angular-dragdrop) is used for the drag-and-drop features on the Lists page and Export modals. It implements all the options for the jQuery UI [Draggable](http://api.jqueryui.com/draggable/) and [Droppable](http://api.jqueryui.com/droppable/) components.

## Angular FCSA Number

[Angular FCSA Number](https://github.com/FCSAmerica/angular-fcsa-number) provides number formatting and validation. An example can be found on the Invoice page.

**WARNING:** This library has been modified to add more flexibility with how many decimals are considered valid.

## Angular File Upload

[ng-file-upload](https://github.com/danialfarid/ng-file-upload) is used on the import modal.

## Angular Loading Bar

[Angular Loading Bar](http://chieffancypants.github.io/angular-loading-bar/) is used for the blue loading bar at the top of the page. This loading bar appears when any API call is made.

## Angular Local Storage

Used for accessing HTML5 Local Storage

## Angular Sticky Header

[Sticky Header](https://github.com/FutureStateMobile/sticky-header) is used to make table headers stick to the top of the page when scrolling.

**WARNING:** This library has been modified. The library didn't work with tables that are within a `position: relative` element. The scrollOffset option was added was added to handle this basically by allowing you to hardcode in the position where the table should start being sticky.

## Angular Toaster



## Angular UI Bootstrap
## Angular UI Router
## Angular UI Select2
## Angular UI Sortable
## Angular Unsaved Changes
## C3, D3
## FileSaver
## jQuery UI
## jstz
## moment
## ng infinite scroll
## ng sticky
## select2
## Touch Punch