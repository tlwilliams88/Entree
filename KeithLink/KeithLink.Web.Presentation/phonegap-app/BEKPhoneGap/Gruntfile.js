module.exports = function(grunt) {

  var config = grunt.file.readJSON('../../angular-app/config.json');

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    config: config,
    meta: {
      angularPath: '../../angular-app/app',
      phonegapPath: 'www'
    },
    phonegap: {
      config: {
        plugins: [
          'https://github.com/apache/cordova-plugin-network-information.git',
          'phonegap-plugin-push --variable SENDER_ID="1013977805145"',
          'cordova-plugin-device',
          'cordova-plugin-dialogs',
          'cordova-plugin-vibration',
          'cordova-plugin-statusbar',
          'cordova-plugin-console',
          'cordova-plugin-inappbrowser',
          'https://github.com/wildabeast/BarcodeScanner.git',
          'https://github.com/phonegap-build/GAPlugin.git',
          'https://github.com/apache/cordova-plugin-whitelist.git'
          // ,'https://github.com/brodysoft/Cordova-SQLitePlugin.git'
        ],
        maxBuffer: 5000,
        platforms: ['ios', 'android'],
        config: {
          template: '_config.xml',
          data: {
            id: '<%= grunt.option("phonegapAppId") %>',
            version: '<%= config.version %>',
            name: '<%= config.name %>',
            description: '<%= config.build.phonegap.description %>',
            author: {
              email: '<%= config.author.email %>',
              href: '<%= config.author.href %>',
              text: '<%= config.author.name %>'
            }
          }
        },
        versionCode: 10002 // android
      }
    },
    clean: [
      "<%= meta.phonegapPath %>/images/{,*/}*",
      "<%= meta.phonegapPath %>/lib/{,*/}*",
      "<%= meta.phonegapPath %>/scripts/app.js",
      "<%= meta.phonegapPath %>/scripts/state.js",
      "<%= meta.phonegapPath %>/index.html",
      "<%= meta.phonegapPath %>/scripts/controllers/{,*/}*",
      "<%= meta.phonegapPath %>/scripts/directives/{,*/}*",
      "<%= meta.phonegapPath %>/scripts/services/{,*/}*",
      "<%= meta.phonegapPath %>/scripts/resources/{,*/}*",
      "<%= meta.phonegapPath %>/scripts/filters/{,*/}*",
      "<%= meta.phonegapPath %>/scripts/animations/{,*/}*",
      "<%= meta.phonegapPath %>/scss/{,*/}*",
      "<%= meta.phonegapPath %>/views/{,*/}*"
    ],
    sass: {
      dist: {
        files: [{
          expand: true,
          cwd: '<%= meta.phonegapPath %>/scss/',
          src: '**/*.scss',
          dest: '<%= meta.phonegapPath %>/css',
          ext: '.css'
        }]
      }
    },
    connect: {
      options: {
        hostname: 'localhost',
        livereload: 35729,
        port: 3000
      },
      server: {
        options: {
          base: 'www',
          open: true
        }
      }
    },
    copy: {
      logo: {
        expand: true,
        src: '<%= meta.phonegapPath %>/images/bek-logo-<%= grunt.option("logoColor") %>.png',
        dest: '<%= meta.phonegapPath %>/images/',
        rename: function(dest, src) {
          return dest + 'bek-logo.png';
        }
      },
      all: {
        files: [{
          cwd: '<%= meta.angularPath %>/images/', // set working folder / root to copy
          src: '**/*', // copy all files and subfolders
          dest: '<%= meta.phonegapPath %>/images/', // destination folder
          expand: true // required when using cwd
        }, {
          cwd: '<%= meta.angularPath %>/fonts/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/fonts/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/lib/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/lib/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/scripts/',
          src: ['app.js', 'state.js'],
          dest: '<%= meta.phonegapPath %>/scripts/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/',
          src: 'index.html',
          dest: '<%= meta.phonegapPath %>/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/scripts/controllers/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/scripts/controllers/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/scripts/directives/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/scripts/directives/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/scripts/services/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/scripts/services/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/scripts/animations/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/scripts/animations/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/scripts/resources/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/scripts/resources/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/scripts/filters/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/scripts/filters/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/views/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/views/',
          expand: true
        }, {
          cwd: '<%= meta.angularPath %>/styles/',
          src: '**/*',
          dest: '<%= meta.phonegapPath %>/scss/',
          expand: true
        }]
      }
    },
    watch: {
      css: {
        files: ['<%= meta.phonegapPath %>/scss/**/*.scss'],
        tasks: ['sass'],
        options: {
          livereload: true
        }
      },
      options: {
        livereload: '<%= connect.options.livereload %>'
      },
      all: {
        files: '<%= meta.phonegapPath %>/{,*/}*.{html,js,png}'
      }
    },
    compass: {
      all: {
        options: {
          sassDir: '<%= meta.phonegapPath %>/scss',
          cssDir: '<%= meta.phonegapPath %>/css'
        }
      }
    },
    ngconstant: {
      // Options for all targets
      options: {
        space: '  ',
        wrap: '\'use strict\';\n\n {%= __ngModule %}',
        name: 'configenv',
        dest: '<%= meta.phonegapPath %>/scripts/configenv.js',
      },
      // Environment targets
      test: {
        constants: {
          ENV: {
            name: '<%= config.environment.test.name %>',
            apiKey: '<%= config.environment.test.apiKey %>',
            apiEndpoint: '<%= config.environment.test.apiEndpoint %>',
            loggingEnabled: config.environment.test.loggingEnabled,
            googleAnalytics: '<%= config.environment.test.googleAnalytics.mobile %>',
            mobileApp: true,
            username: 'bek.qa.user@gmail.com',
            password: 'Ab12345'
          }
        }
      },
      review: {
        constants: {
          ENV: {
            name: '<%= config.environment.test.name %>',
            apiKey: '<%= config.environment.test.apiKey %>',
            apiEndpoint: '<%= config.environment.test.apiEndpoint %>',
            loggingEnabled: config.environment.prod.loggingEnabled,
            googleAnalytics: '<%= config.environment.test.googleAnalytics.mobile %>',
            mobileApp: true
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
            loggingEnabled: config.environment.prod.loggingEnabled,
            googleAnalytics: '<%= config.environment.prod.googleAnalytics.mobile %>',
            mobileApp: true
          }
        }
      }
    },
    replace: {
      indexhtml: {
        options: {
            patterns: [{
              json: {
                '<!--REPLACEHERE':        ' ',
                'REPLACEHERE-->':         ' ',
                'angulartics-ga.min.js':  'angulartics-ga-cordova.min.js',
                ' ng-app="bekApp"':       ' ',
                'styles/main.css':        'css/main.css'
              }
            }],
            usePrefix: false
          },
          files: [
            {expand: true, flatten: true, src: ['www/index.html'], dest: 'www/'}
          ]
      },
      appjs: {
        options: {
            patterns: [{
              json: {
                '//googleAnalyticsCordovaProvider': 'googleAnalyticsCordovaProvider',
                '$analyticsProvider':               'googleAnalyticsCordovaProvider',
                'angulartics.google.analytics':     'angulartics.google.analytics.cordova'
              }
            }],
            usePrefix: false
          },
          files: [
            {expand: true, flatten: true, src: ['www/scripts/app.js'], dest: 'www/scripts/'}
          ]
      },
      googleservicesjs: {
        options: {
          patterns: [{
            json: {
              '//REPLACEHERESTART': '/*',
              '//REPLACEHEREEND': '*/'
            }
          }],
          usePrefix: false
        },
        files: [
          {expand: true, flatten: true, src: ['www/scripts/controllers/GoogleServicesController.js'], dest: 'www/scripts/controllers/'}
        ]
      }
    }
  });

  grunt.loadNpmTasks('grunt-contrib-sass');
  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-contrib-connect');
  grunt.loadNpmTasks('grunt-phonegap');
  grunt.loadNpmTasks('grunt-contrib-copy');
  grunt.loadNpmTasks('grunt-contrib-clean');
  grunt.loadNpmTasks('grunt-contrib-compass');
  grunt.loadNpmTasks('grunt-ng-constant');
  grunt.loadNpmTasks('grunt-replace');

  grunt.registerTask('update', [
    'clean',
    'copy:all',
    'replace:indexhtml',
    'replace:appjs',
    'replace:googleservicesjs',
    'compass:all'
  ]);

  grunt.registerTask('server', function() {
    grunt.task.run('connect:server');
    return grunt.task.run('watch');
  });
  
  grunt.registerTask('updateVersion', function() {
    var versionNumber = grunt.option('appVersion') || 'No version number set';
    config['version'] = versionNumber;
    grunt.file.write('config.json', JSON.stringify(config, null, 2));
  });

  // takes ios or android as platform
  // takes prod, review, or test as targets
  grunt.registerTask('build', function(platform, target) {
    if (!target) { target = 'test' };

    // select correct app id
    var phonegapAppId = config.environment.test.phonegapAppId;
    // deploy to the production ios app
    if (target === 'prod' || target === 'review') {
      phonegapAppId = config.environment.prod.phonegapAppId;
    }
    grunt.option('phonegapAppId', phonegapAppId);


    if (target === 'prod') {
      grunt.option('logoColor', 'yellow');
    } else {
      grunt.option('logoColor', 'green');
    }

    var phonegapBuild = 'phonegap:build';
    if (platform) {
      phonegapBuild += ':' + platform;
    }

    grunt.task.run([
        'updateVersion',
        'copy:logo',
        'ngconstant:' + target,
        phonegapBuild
    ]);
  });

};
