// Generated on 2014-07-03 using generator-angular 0.9.2
'use strict';

// # Globbing
// for performance reasons we're only matching one level down:
// 'test/spec/{,*/}*.js'
// use this if you want to recursively match all subfolders:
// 'test/spec/**/*.js'

module.exports = function (grunt) {

  // Load grunt tasks automatically
  require('load-grunt-tasks')(grunt);

  // Time how long tasks take. Can help when optimizing build times
  require('time-grunt')(grunt);

  // Configurable paths for the application
  var appConfig = {
    app: 'app',
    dist: 'dist',
    dev: '../app',
  };

  var config = grunt.file.readJSON('config.json');

  // Define the configuration for all the tasks
  grunt.initConfig({

      // Project settings
      yeoman: appConfig,
      config: config,

      // Watches files for changes and runs tasks based on the changed files
      watch: {
          js: {
              files: ['<%= yeoman.app %>/scripts/{,*/}*.js'],
              tasks: ['newer:jshint:all'],
              options: {
                  livereload: '<%= connect.options.livereload %>'
              }
          },
          jsTest: {
              files: ['test/unit/{,*/}*.js'],
              tasks: []//['newer:jshint:test']
          },
          compass: {
              files: ['<%= yeoman.app %>/styles/{,*/}*.{scss,sass}', '<%= yeoman.app %>/styles/**/*.{scss,sass}'],
              tasks: ['compass:server', 'autoprefixer']
          },
          gruntfile: {
              files: ['Gruntfile.js']
          },
          livereload: {
              options: {
                  livereload: '<%= connect.options.livereload %>'
              },
              files: [
                '<%= yeoman.app %>/**/*.html',
                '.tmp/styles/{,*/}*.css',
                // '<%= yeoman.app %>/images/{,*/}*.{png,jpg,jpeg,gif,webp,svg}'
              ]
          }
      },

      // The actual grunt server settings
      connect: {
          options: {
              port: 9000,
              // Change this to '0.0.0.0' to access the server from outside.
              hostname: '0.0.0.0',
              livereload: 35729
          },
          livereload: {
              options: {
                  open: true,
                  middleware: function (connect) {
                      return [
                        connect.static('.tmp'),
                        connect.static(appConfig.app)
                      ];
                  }
              }
          },
          dist: {
              options: {
                  open: true,
                  base: '<%= yeoman.dist %>'
              }
          }
      },

      // Make sure code styles are up to par and there are no obvious mistakes
      jshint: {
          options: {
              jshintrc: '.jshintrc',
              reporter: require('jshint-stylish'),
              reporterOutput: ''
          },
          all: {
              src: [
                'Gruntfile.js',
                '<%= yeoman.app %>/scripts/**/*.js'
              ]
          },
          test: {
              options: {
                  jshintrc: 'test/.jshintrc'
              },
              src: ['test/unit/{,*/}*.js']
          }
      },

      // Empties folders to start fresh
      clean: {
          dist: {
              files: [{
                  dot: true,
                  src: [
                    '.tmp',
                    '<%= yeoman.dist %>/{,*/}*',
                    '!<%= yeoman.dist %>/.git*'
                  ]
              }]
          },
          server: '.tmp',
          dev: {
              options: { force: true },
              files: [{
                  dot: true,
                  src: ['<%= yeoman.dev %>/{,*/}*'
                  ]
              }]
          }
      },

      // Add vendor prefixed styles
      autoprefixer: {
          options: {
              browsers: ['last 1 version']
          },
          dist: {
              files: [{
                  expand: true,
                  cwd: '.tmp/styles/',
                  src: '{,*/}*.css',
                  dest: '.tmp/styles/'
              }]
          }
      },

      // Compiles Sass to CSS and generates necessary files if requested
      compass: {
          options: {
              sassDir: '<%= yeoman.app %>/styles',
              cssDir: '.tmp/styles',
              generatedImagesDir: '.tmp/images/generated',
              imagesDir: '<%= yeoman.app %>/images',
              javascriptsDir: '<%= yeoman.app %>/scripts',
              fontsDir: '<%= yeoman.app %>/styles/fonts',
              httpImagesPath: '/images',
              httpGeneratedImagesPath: '/images/generated',
              httpFontsPath: '/styles/fonts',
              relativeAssets: false,
              assetCacheBuster: false,
              raw: 'Sass::Script::Number.precision = 10\n'
          },
          dist: {
              options: {
                  generatedImagesDir: '<%= yeoman.dist %>/images/generated'
              }
          },
          server: {
              options: {
                  debugInfo: true
              }
          }
      },

      // Renames files for browser caching purposes
      filerev: {
          dist: {
              src: [
                '<%= yeoman.dist %>/scripts/{,*/}*.js',
                '<%= yeoman.dist %>/styles/{,*/}*.css',
                // '<%= yeoman.dist %>/images/{,*/}*.{png,jpg,jpeg,gif,webp,svg}'
              ]
          }
      },

      // Reads HTML for usemin blocks to enable smart builds that automatically
      // concat, minify and revision files. Creates configurations in memory so
      // additional tasks can operate on them
      useminPrepare: {
          html: '<%= yeoman.app %>/index.html',
          options: {
              dest: '<%= yeoman.dist %>',
              flow: {
                  html: {
                      steps: {
                          js: ['concat', 'uglifyjs'],
                          css: ['cssmin']
                      },
                      post: {}
                  }
              }
          }
      },

      // Performs rewrites based on filerev and the useminPrepare configuration
      usemin: {
          html: ['<%= yeoman.dist %>/*.html', '<%= yeoman.dist %>/views/{,*/}*.html'],
          css: ['<%= yeoman.dist %>/styles/{,*/}*.css'],
          js: '<%= yeoman.dist %>/scripts/*.js',
          options: {
              assetsDirs: ['<%= yeoman.dist %>', '<%= yeoman.dist %>/images'],
              patterns: {
                  // FIXME While usemin won't have full support for revved files we have to put all references manually here
                  js: [
                    [/(demoimage1\.jpg)/g, 'Replacing reference to demoimage1.jpg'],
                    [/(demoimage2\.jpg)/g, 'Replacing reference to demoimage2.jpg'],
                    [/(demoimage3\.jpg)/g, 'Replacing reference to demoimage3.jpg'],
                    [/(demoimage4\.jpg)/g, 'Replacing reference to demoimage4.jpg'],
                    [/(placeholder-dsr\.jpg)/g, 'Replacing reference to placeholder-dsr.jpg'],
                    [/(placeholder-user\.png)/g, 'Replacing reference to placeholder-user.png']
                  ]
              }
          }
      },

      svgmin: {
          dist: {
              files: [{
                  expand: true,
                  cwd: '<%= yeoman.app %>/images',
                  src: '{,*/}*.svg',
                  dest: '<%= yeoman.dist %>/images'
              }]
          }
      },

      htmlmin: {
          dist: {
              options: {
                  collapseWhitespace: true,
                  conservativeCollapse: true,
                  collapseBooleanAttributes: true,
                  removeCommentsFromCDATA: true,
                  removeOptionalTags: true
              },
              files: [{
                  expand: true,
                  cwd: '<%= yeoman.dist %>',
                  src: ['*.html', 'views/{,*/}*.html'],
                  dest: '<%= yeoman.dist %>'
              }]
          }
      },

      // ngmin tries to make the code safe for minification automatically by
      // using the Angular long form for dependency injection. It doesn't work on
      // things like resolve or inject so those have to be done manually.
      ngmin: {
          dist: {
              files: [{
                  expand: true,
                  cwd: '.tmp/concat/scripts',
                  src: '*.js',
                  dest: '.tmp/concat/scripts'
              }]
          }
      },

      // Copies remaining files to places other tasks can use
      copy: {
          dist: {
              files: [{
                  expand: true,
                  dot: true,
                  cwd: '<%= yeoman.app %>',
                  dest: '<%= yeoman.dist %>',
                  src: [
                    '*.{ico,png,txt}',
                    '.htaccess',
                    '*.html',
                    'views/{,*/}*.html',
                    'images/**',
                    'fonts/{,*/}*'
                  ]
              }, {
                  expand: true,
                  cwd: '.tmp/images',
                  dest: '<%= yeoman.dist %>/images',
                  src: ['generated/*']
              }, {
                  expand: true,
                  flatten: true,
                  cwd: '<%= yeoman.app %>/lib',
                  dest: '<%= yeoman.dist %>/styles/fonts',
                  src: '**/fonts/*'
              }]
          },
          newRelic: {
              files: [{
                expand: true,
                cwd: '<%= yeoman.app %>/newrelic/newrelic-dev',
                dest: '<%= yeoman.dist %>/newrelic',
                src: '**/newrelic.js'
              }]
          },
          newRelicDebug: {
              files: [{
                expand: true,
                cwd: '<%= yeoman.app %>/newrelic/newrelic-dev',
                dest: '<%= yeoman.dev %>/newrelic',
                src: '**/newrelic.js'
              }]
          },
          newRelicTest: {
              files: [{
                expand: true,
                cwd: '<%= yeoman.app %>/newrelic/newrelic-test',
                dest: '<%= yeoman.dist %>/newrelic',
                src: '**/newrelic.js'
              }]
          },
          newRelicProd: {
              files: [{
                expand: true,
                cwd: '<%= yeoman.app %>/newrelic/newrelic-prod',
                dest: '<%= yeoman.dist %>/newrelic',
                src: '**/newrelic.js'
              }]
          },
          inspectletDebug: {
              files: [{
                expand: true,
                cwd: '<%= yeoman.app %>/inspectlet/inspectlet-dev',
                dest: '<%= yeoman.dev %>/inspectlet',
                src: '**/inspectlet.js'
              }]
          },
          inspectletTest: {
              files: [{
                expand: true,
                cwd: '<%= yeoman.app %>/inspectlet/inspectlet-test',
                dest: '<%= yeoman.dist %>/inspectlet',
                src: '**/inspectlet.js'
              }]
          },
          inspectletProd: {
              files: [{
                expand: true,
                cwd: '<%= yeoman.app %>/inspectlet/inspectlet-prod',
                dest: '<%= yeoman.dist %>/inspectlet',
                src: '**/inspectlet.js'
              }]
          },
          styles: {
              expand: true,
              cwd: '<%= yeoman.app %>/styles',
              dest: '.tmp/styles/',
              src: '{,*/}*.css'
          },
          dev: {
              files: [{
                  expand: true,
                  dot: true,
                  cwd: '<%= yeoman.app %>',
                  dest: '<%= yeoman.dev %>',
                  src: [
                    '*.{ico,png,txt}',
                    '.htaccess',
                    '*.html',
                    'views/{,*/}*.html',
                    'scripts/**/*.js',
                    'images/{,*/}*.{webp,png,gif}',
                    'lib/**/*.{js,css,eot,svg,ttf,woff}',
                    'fonts/{,*/}*'
                  ]
              }, {
                  expand: true,
                  cwd: '<%= yeoman.app %>',
                  dest: '<%= yeoman.dev %>',
                  src: ['images/**']
              }, {
                  expand: true,
                  cwd: '.tmp',
                  dest: '<%= yeoman.dev %>',
                  src: 'styles/*.css'
              }]
          }
      },

      // Run some tasks in parallel to speed up the build process
      concurrent: {
          options: {
              limit: 3
          },
          server: [
            'compass:server'
          ],
          test: [
            'compass'
          ],
          dist: [
            'compass:dist',
            'svgmin'
          ]
      },

      // Test settings
      karma: {
          unit: {
              singleRun: true,
              configFile: 'test/karma.conf.js'
          }
      },

      // start mock api server
      run: {
          mock_server: {
              options: {
                  wait: false
              },
              args: [
                'mockApi/apiserver.js'
              ]
          }
      },

      includeSource: {
          options: {
              basePath: 'app',
              baseUrl: ''
          },
          server: {
              files: {
                  '<%= yeoman.app %>/index.html': '<%= yeoman.app %>/index.html'
              }
          },
          dist: {
              files: {
                  '<%= yeoman.dist %>/index.html': '<%= yeoman.app %>/index.html'
              }
          },
          dev: {
              files: {
                  '<%= yeoman.dev %>/index.html': '<%= yeoman.app %>/index.html'
              }
          }
      },

      ngconstant: {
          // Options for all targets
          options: {
              space: '  ',
              wrap: '\'use strict\';\n\n {%= __ngModule %}',
              name: 'configenv',
              dest: '<%= yeoman.app %>/scripts/configenv.js'
          },
          // Environment targets
          debug: {
              constants: {
                  ENV: {
                    version: '<%= config.version %>',
                    name: '<%= config.environment.debug.name %>',
                    apiKey: '<%= config.environment.debug.apiKey %>',
                    apiEndpoint: '<%= config.environment.debug.apiEndpoint %>',
                    loggingEnabled: '<%= config.environment.debug.loggingEnabled %>',
                    googleAnalytics: '<%= config.environment.debug.googleAnalytics.web %>',
                    cognosUrl: '<%= config.environment.debug.cognosUrl %>',
                    menuMaxUrl: '<%= config.environment.debug.menuMaxUrl %>',
                    flipsnackUrl: '<%= config.environment.debug.flipsnackUrl %>',
                    username: 'sabroussard@somecompany.com',
                    password: 'L1ttleStev1e',
                    mobileApp: '<%= config.environment.debug.mobileApp %>',
                    lastListStorageTimeout: '<%= config.environment.debug.lastListStorageTimeout %>',
                    enableDebugInfo: '<%= config.environment.debug.enableDebugInfo %>'
                  }
              }
          },
		      demo: {
              constants: {
                  ENV: {
                    version: '<%= config.version %>',
                    name: '<%= config.environment.demo.name %>',
                    apiKey: '<%= config.environment.demo.apiKey %>',
                    apiEndpoint: '<%= config.environment.demo.apiEndpoint %>',
                    loggingEnabled: '<%= config.environment.demo.loggingEnabled %>',
                    googleAnalytics: '<%= config.environment.demo.googleAnalytics.web %>',
                    cognosUrl: '<%= config.environment.demo.cognosUrl %>',
                    mobileApp: false,
                    lastListStorageTimeout: 48,
                    enableDebugInfo: true
                  }
              }
          },
          dev: {
              constants: {
                  ENV: {
                    version: '<%= config.version %>',
                    name: '<%= config.environment.dev.name %>',
                    apiKey: '<%= config.environment.dev.apiKey %>',
                    apiEndpoint: '<%= config.environment.dev.apiEndpoint %>',
                    loggingEnabled: '<%= config.environment.dev.loggingEnabled %>',
                    googleAnalytics: '<%= config.environment.dev.googleAnalytics.web %>',
                    cognosUrl: '<%= config.environment.dev.cognosUrl %>',
                    menuMaxUrl: '<%= config.environment.dev.menuMaxUrl %>',
                    flipsnackUrl: '<%= config.environment.dev.flipsnackUrl %>',
                    username: 'sabroussard@somecompany.com',
                    password: 'L1ttleStev1e',
                    mobileApp: false,
                    lastListStorageTimeout: 48,
                    enableDebugInfo: true
                  }
              }
          },
          test: {
              constants: {
                  ENV: {
                    version: '<%= config.version %>',
                    name: '<%= config.environment.test.name %>',
                    apiKey: '<%= config.environment.test.apiKey %>',
                    apiEndpoint: '<%= config.environment.test.apiEndpoint %>',
                    loggingEnabled: '<%= config.environment.test.loggingEnabled %>',
                    googleAnalytics: '<%= config.environment.test.googleAnalytics.web %>',
                    cognosUrl: '<%= config.environment.test.cognosUrl %>',
                    menuMaxUrl: '<%= config.environment.test.menuMaxUrl %>',
                    flipsnackUrl: '<%= config.environment.test.flipsnackUrl %>',
                    mobileApp: false,
                    lastListStorageTimeout: 48,
                    enableDebugInfo: false
                  }
              }
          },
          prod: {
              constants: {
                  ENV: {
                    version: '<%= config.version %>',
                    name: '<%= config.environment.prod.name %>',
                    apiKey: '<%= config.environment.prod.apiKey %>',
                    apiEndpoint: '<%= config.environment.prod.apiEndpoint %>',
                    loggingEnabled: '<%= config.environment.prod.loggingEnabled %>',
                    googleAnalytics: '<%= config.environment.prod.googleAnalytics.web %>',
                    cognosUrl: '<%= config.environment.prod.cognosUrl %>',
                    menuMaxUrl: '<%= config.environment.prod.menuMaxUrl %>',
                    flipsnackUrl: '<%= config.environment.prod.flipsnackUrl %>',
                    mobileApp: false,
                    lastListStorageTimeout: 48,
                    enableDebugInfo: false
                  }
              }
          },
		      beta: {
              constants: {
                  ENV: {
                    version: '<%= config.version %>',
                    name: '<%= config.environment.beta.name %>',
                    apiKey: '<%= config.environment.beta.apiKey %>',
                    apiEndpoint: '<%= config.environment.beta.apiEndpoint %>',
                    loggingEnabled: '<%= config.environment.beta.loggingEnabled %>',
                    googleAnalytics: '<%= config.environment.beta.googleAnalytics.web %>',
                    cognosUrl: '<%= config.environment.beta.cognosUrl %>',
                    mobileApp: false,
                    lastListStorageTimeout: 48,
                    enableDebugInfo: false
                  }
              }
          },
          stage: {
              constants: {
                  ENV: {
                      name: 'stage',
                      apiKey: 'web_stage_v1',
                      apiEndpoint: 'https://shopstaging.benekeith.com/api',
                      googleAnalytics: '<%= config.environment.stage.cognosUrl %>',
                      loggingEnabled: false,
                      cognosUrl: 'UA-58495303-1',
                      lastListStorageTimeout: 48,
                      enableDebugInfo: false
                  }
              }
          }
      }
  });

  grunt.registerTask('updateVersion', function() {
    var versionNumber = grunt.option('appVersion') || 'No version number set';
    config['version'] = versionNumber;
    grunt.file.write('config.json', JSON.stringify(config, null, 2));
  });

  grunt.registerTask('serve', 'Compile then start a connect web server', function (target) {
    if (target === 'prod') {
      return grunt.task.run(['build', 'connect:dist:keepalive']);
    } else if (target === 'test') {
      return grunt.task.run(['build-for-test', 'connect:dist:keepalive']);
    } else if (target === 'beta') {
      return grunt.task.run(['build-for-beta', 'connect:dist:keepalive']);
    }

    grunt.task.run([
      'clean:server',
      'ngconstant:dev',
      'concurrent:server',
      'autoprefixer',
      'connect:livereload',
      'run:mock_server',
      'watch'
    ]);
  });

  grunt.registerTask('build-for-debug', [
      'clean:dev',
      'ngconstant:debug',
      'compass:server',
      'copy:dev',
      'copy:newRelicDebug',
      'copy:inspectletDebug',
      'karma'
    ]);

  grunt.registerTask('build-for-dev', [
    'clean:dev',
    'ngconstant:dev',
    'compass:server',
    'copy:dev',
    'copy:newRelicDebug',
    'copy:inspectletDebug',
    'karma'
  ]);

  grunt.registerTask('build-for-test', [
    'updateVersion',
    'clean:dist',
    'ngconstant:test',
    'useminPrepare',
    'concurrent:dist',
    'autoprefixer',
    'concat',
    'ngmin',
    'copy:dist',
    'copy:newRelicTest',
    'copy:inspectletTest',
    'cssmin',
    'uglify',
    'filerev',
    'usemin',
    'htmlmin'
  ]);

  grunt.registerTask('build-for-beta', [
    'updateVersion',
    'clean:dist',
    'ngconstant:beta',
    'useminPrepare',
    'concurrent:dist',
    'autoprefixer',
    'concat',
    'ngmin',
    'copy:dist',
    'copy:newRelic',
    'copy:inspectletTest',
    'cssmin',
    'uglify',
    'filerev',
    'usemin',
    'htmlmin'
  ]);

  grunt.registerTask('build', [
    'updateVersion',
    'clean:dist',
    'ngconstant:prod',
    'useminPrepare',
    'concurrent:dist',
    'autoprefixer',
    'concat',
    'ngmin',
    'copy:dist',
    'copy:newRelicProd',
    'copy:inspectletProd',
    'cssmin',
    'uglify',
    'filerev',
    'usemin',
    'htmlmin'
  ]);

};
