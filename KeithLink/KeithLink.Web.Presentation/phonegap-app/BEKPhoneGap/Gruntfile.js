module.exports = function(grunt) {

    return grunt.initConfig({
      pkg: grunt.file.readJSON('package.json'),
      phonegap: {
        config: {
          plugins: [
              'https://github.com/apache/cordova-plugin-network-information.git',
              'https://github.com/phonegap-build/PushPlugin.git',
              'org.apache.cordova.device',
              'org.apache.cordova.dialogs',
              'org.apache.cordova.vibration',
              'org.apache.cordova.statusbar',
              'org.apache.cordova.console',
              'org.apache.cordova.inappbrowser',
              'https://github.com/wildabeast/BarcodeScanner.git'
          ],
          maxBuffer: 500,
          platforms: ['ios', 'android'],
          config: {
            template: '_config.xml',
            data: {
              id: 'com.benekeith.entree',
              version: '<%= pkg.version %>',
              name: '<%= pkg.name %>',
              description: '<%= pkg.description %>',
              author: {
                email: '',
                href: '',
                text: 'Credera'
              }
            }
          },
          versionCode: 1
        }
      },
      clean: [
      "www/images/{,*/}*",
      "www/lib/{,*/}*",
      "www/scripts/app.js",
      "www/scripts/state.js",
      "www/index.html",
      "www/scripts/controllers/{,*/}*",
      "www/scripts/directives/{,*/}*",
      "www/scripts/services/{,*/}*",
      "www/scripts/resources/{,*/}*",
      "www/scripts/filters/{,*/}*",
      "www/scripts/animations/{,*/}*",
      "www/scss/{,*/}*",
      "www/views/{,*/}*"
      ],
      sass: {
        dist: {
          files: [
            {
              expand: true,
              cwd: 'www/scss/',
              src: '**/*.scss',
              dest: 'www/css',
              ext: '.css'
            }
          ]
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
        all: {
        files: [
{
cwd: '../../angular-app/app/images/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/images/',    // destination folder
    expand: true           // required when using cwd
  }, {
cwd: '../../angular-app/app/fonts/',
    src: '**/*',
    dest: 'www/fonts/',
    expand: true
  },
  {
cwd: '../../angular-app/app/lib/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/lib/',    // destination folder
    expand: true           // required when using cwd
  },
    {
cwd: '../../angular-app/app/scripts/',  // set working folder / root to copy
    src: ['app.js', 'state.js'],           // copy app.js
    dest: 'www/scripts/',    // destination folder
    expand: true           // required when using cwd
  },
//       {
// cwd: '../../angular-app/app/scripts/',  // set working folder / root to copy
//     src: 'state.js',           // copy state.js
//     dest: 'www/scripts/',    // destination folder
//     expand: true           // required when using cwd
//   },
      {
cwd: '../../angular-app/app/',  // set working folder / root to copy
    src: 'index.html',           // copy index.html
    dest: 'www/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/app/scripts/controllers/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/controllers/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/app/scripts/directives/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/directives/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/app/scripts/services/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/services/',    // destination folder
    expand: true           // required when using cwd
  },
    {
cwd: '../../angular-app/app/scripts/animations/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/animations/',    // destination folder
    expand: true           // required when using cwd
  },
    {
cwd: '../../angular-app/app/scripts/resources/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/resources/',    // destination folder
    expand: true           // required when using cwd
  },
    {
cwd: '../../angular-app/app/scripts/filters/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/filters/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/app/views/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/views/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/app/styles/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scss/',    // destination folder
    expand: true           // required when using cwd
  }]
}
      },
      watch: {
        css: {
          files: ['www/scss/**/*.scss'],
          tasks: ['sass'],
          options: {
            livereload: true
          }
        },
        options: {
          livereload: '<%= connect.options.livereload %>'
        },
        all: {
          files: 'www/{,*/}*.{html,js,png}'
        }
      },
      compass: {
        all: {
          options: {
            sassDir: 'www/scss',
            cssDir: 'www/css'
          }
        }
      }
    }, grunt.loadNpmTasks('grunt-contrib-sass'),
     grunt.loadNpmTasks('grunt-contrib-watch'), 
     grunt.loadNpmTasks('grunt-contrib-connect'), 
     grunt.loadNpmTasks('grunt-phonegap'),
     grunt.loadNpmTasks('grunt-contrib-copy'),
     grunt.loadNpmTasks('grunt-contrib-clean'),
     grunt.loadNpmTasks('grunt-contrib-compass'),
     grunt.registerTask('update',[
      'clean',
      'copy:all',
      'compass:all']),
     grunt.registerTask('server', function() {
      grunt.task.run('connect:server');
      return grunt.task.run('watch');
    }));
  };