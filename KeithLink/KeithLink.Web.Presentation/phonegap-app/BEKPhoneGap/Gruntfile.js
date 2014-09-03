module.exports = function(grunt) {

    return grunt.initConfig({
      pkg: grunt.file.readJSON('package.json'),
      phonegap: {
        config: {
          plugins: [],
          platforms: ['ios', 'android'],
          config: {
            template: '_config.xml',
            data: {
              id: 'com.benekeith',
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
          versionCode: 1,
          permissions: []
        }
      },
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
        files: [{
    cwd: '../../angular-app/app/',  // set working folder / root to copy
    src: 'index.html',           // copy index.html
    dest: 'www/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/images/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/images/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/lib/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/lib/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/scripts/app.js/',  // set working folder / root to copy
    src: 'app.js',           // copy app.js
    dest: 'www/scripts/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/scripts/controllers/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/controllers/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/scripts/directives/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/directives/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/scripts/services/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scripts/services/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/views/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/views/',    // destination folder
    expand: true           // required when using cwd
  },
  {
cwd: '../../angular-app/styles/',  // set working folder / root to copy
    src: '**/*',           // copy all files and subfolders
    dest: 'www/scss/',    // destination folder
    expand: true           // required when using cwd
  }]
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
      }
    }, grunt.loadNpmTasks('grunt-contrib-sass'),
     grunt.loadNpmTasks('grunt-contrib-watch'), 
     grunt.loadNpmTasks('grunt-contrib-connect'), 
     grunt.loadNpmTasks('grunt-phonegap'),
     grunt.loadNpmTasks('grunt-contrib-copy'),
     grunt.loadNpmTasks('grunt-contrib-clean'),
     grunt.registerTask('update',[
      'clean',
      'copy']),
     grunt.registerTask('server', function() {
      grunt.task.run('connect:server');
      return grunt.task.run('watch');
    }));
  };