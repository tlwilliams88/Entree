// Karma configuration
// http://karma-runner.github.io/0.10/config/configuration-file.html
 
module.exports = function(config) {
  config.set({
    // base path, that will be used to resolve files and exclude
    basePath: '../',
 
    // testing framework to use (jasmine/mocha/qunit/...)
    frameworks: ['jasmine'],
 
    // list of files / patterns to load in the browser
    files: [
      'app/lib/jquery/jquery-1.11.1.min.js',
      'app/lib/angular/angular.js',
      'app/lib/angular-mocks/angular-mocks.js',
      'app/lib/angular-animate/angular-animate.js',
      'app/lib/angular-cookies/angular-cookies.js',
      'app/lib/angular-resource/angular-resource.js',
      'app/lib/angular-touch/angular-touch.js',
      'app/lib/angular-local-storage/angular-local-storage.js',
      'app/lib/angular-ui-router/angular-ui-router.js',
      'app/lib/angular-ui-bootstrap/ui-bootstrap-tpls-0.11.0.min.js',
      'app/lib/angular-ui-utils/ui-utils.js',
      'app/lib/ng-mobile-menu/ng-mobile-menu.js',
      'app/lib/angular-dragdrop/angular-dragdrop.js',
      'app/lib/angular-ui-sortable/sortable.js',
      'app/lib/ng-infinite-scroll/ng-infinite-scroll.js',
      
      'app/scripts/*.js',
      'app/scripts/**/*.js',
      
      'test/unit/**/*.js'
    ],
 
    // list of files / patterns to exclude
    exclude: [],
 
    // web server port
    port: 9001,
 
    // level of logging
    // possible values: LOG_DISABLE || LOG_ERROR || LOG_WARN || LOG_INFO || LOG_DEBUG
    logLevel: config.LOG_INFO,
 
 
    // enable / disable watching file and executing tests whenever any file changes
    autoWatch: false,
 
    reporters: ['dots', 'junit'],
    junitReporter: {
      outputFile: 'test/test-results/test-results.xml'
    },
 
 
    // Start these browsers, currently available:
    // - Chrome
    // - ChromeCanary
    // - Firefox
    // - Opera
    // - Safari (only Mac)
    // - PhantomJS
    // - IE (only Windows)
    browsers: ['PhantomJS'],
 
 
    // Continuous Integration mode
    // if true, it capture browsers, run tests and exit
    singleRun: false
  });
};