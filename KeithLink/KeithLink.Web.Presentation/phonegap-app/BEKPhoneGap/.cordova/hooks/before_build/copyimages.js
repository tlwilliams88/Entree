#!/usr/bin/env node

// This hook copies icon files to the appropriate locations, supports Android mipmap 
   
// Key of object is the source file, 
// Value is the destination location.
 
var filestocopy = [{
    "www/icon.png": 
    "platforms/android/res/drawable/icon.png"
}, {
    "www/res/icon/android/icon-72-hdpi.png": 
    "platforms/android/res/drawable-hdpi/icon.png"
}, {
    "www/res/icon/android/icon-36-ldpi.png": 
    "platforms/android/res/drawable-ldpi/icon.png"
}, {
    "www/res/icon/android/icon-48-mdpi.png": 
    "platforms/android/res/drawable-mdpi/icon.png"
}, {
    "www/res/icon/android/icon-96-xhdpi.png": 
    "platforms/android/res/drawable-xhdpi/icon.png"
}, {   
    "www/res/icon/android/icon-72-hdpi.png": 
    "platforms/android/res/mipmap-hdpi/icon.png"
}, {
    "www/res/icon/android/icon-36-ldpi.png": 
    "platforms/android/res/mipmap-ldpi/icon.png"
}, {
    "www/res/icon/android/icon-48-mdpi.png": 
    "platforms/android/res/mipmap-mdpi/icon.png"
}, {
    "www/res/icon/android/icon-96-xhdpi.png": 
    "platforms/android/res/mipmap-xhdpi/icon.png"
}, {
    "www/icon.png": 
    "platforms/android/res/drawable/icon.png"
}, {
    "www/res/screen/android/splash-land-hdpi.png": 
    "platforms/android/res/drawable-land-hdpi/screen.png"
}, {
    "www/res/screen/android/splash-land-ldpi.png": 
    "platforms/android/res/drawable-land-ldpi/screen.png"
}, {
    "www/res/screen/android/splash-land-mdpi.png": 
    "platforms/android/res/drawable-land-mdpi/screen.png"
}, {
    "www/res/screen/android/splash-land-xhdpi.png": 
    "platforms/android/res/drawable-land-xhdpi/screen.png"
}, {
    "www/res/screen/android/splash-port-hdpi.png": 
    "platforms/android/res/drawable-port-hdpi/screen.png"
}, {
    "www/res/screen/android/splash-port-ldpi.png": 
    "platforms/android/res/drawable-port-ldpi/screen.png"
}, {   
    "www/res/screen/android/splash-port-mdpi.png": 
    "platforms/android/res/drawable-port-mdpi/screen.png"
}, {
    "www/res/screen/android/splash-port-xhdpi.png": 
    "platforms/android/res/drawable-port-xhdpi/screen.png"
}];
 
var fs = require('fs');
var path = require('path');
 
// no need to configure below
var rootdir = process.argv[2];
 
filestocopy.forEach(function(obj) {
    Object.keys(obj).forEach(function(key) {
        var val = obj[key];
        var srcfile = path.join(rootdir, key);
        var destfile = path.join(rootdir, val);
        var destdir = path.dirname(destfile);
        if (fs.existsSync(srcfile) && fs.existsSync(destdir)) {
            fs.createReadStream(srcfile).pipe(
               fs.createWriteStream(destfile));
        }
    });
});