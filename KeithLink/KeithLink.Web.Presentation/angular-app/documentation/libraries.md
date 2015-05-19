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

