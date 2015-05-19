# Bootstrap and the theme

We are using the sass version of the [Bootstrap 3](http://getbootstrap.com/) grid for our responsive framework. Located in the ```styles/vendor/bootstrap/``` directory.

## Bootstrap theme

We are also using a theme to override some variables in the bootstrap files such as colors and font sizes. This is located in the ```styles/vendor/_boostrap-theme.scss``` file. Any variables listed in the Bootstrap documentation [here](http://getbootstrap.com/customize/) can be overridden in the theme file. Just replace the '@' with a '$'.