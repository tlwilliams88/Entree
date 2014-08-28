module.exports = (grunt) ->

  # Project configuration.
  grunt.initConfig
    pkg: grunt.file.readJSON 'package.json'

    phonegap:
      config:
        plugins: []
        platforms: ['ios','android']
        config:
          template: '_config.xml'
          data:
            id: 'com.benekeith'
            version: '<%= pkg.version %>'
            name: '<%= pkg.name %>'
            description: '<%= pkg.description %>'
            author:
              email: ''
              href: ''
              text: 'Credera'

        versionCode: 1
        permissions: []
        
    sass:
      dist:
        files:[
          expand: true
          cwd: 'www/scss/'
          src: '**/*.scss'
          dest: 'www/css'
          ext: '.css'
        ]

    connect:
      options:
        hostname: 'localhost'
        livereload: 35729
        port: 3000
      server:
        options:
          base: 'www'
          open: true

    watch:
      css:
        files:['www/scss/**/*.scss']
        tasks: ['sass']
        options:
          livereload: true
      options:
        livereload: '<%= connect.options.livereload %>'
      all:
        files: 'www/{,*/}*.{html,js,png}'
        
    

    grunt.loadNpmTasks 'grunt-contrib-sass'
    grunt.loadNpmTasks 'grunt-contrib-watch'
    grunt.loadNpmTasks 'grunt-contrib-connect'
    grunt.loadNpmTasks 'grunt-phonegap'

    grunt.registerTask 'server', ->
      grunt.task.run 'connect:server'
      grunt.task.run 'watch'
