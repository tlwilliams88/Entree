# Icons and Icomoon

We are using an app called [Icomoon](https://icomoon.io/app) for our icon font.

Follow these steps to load the icon font project and view available icons.

1. In the upper-right corner, click the icon of a stack of papers next to the project name.
2. Select "Import Project"
3. Select the project json located at ```lib/icomoon/icomoon_project.json```
4. You should see a list of icons. The selected icons with the yellow borders are icons that are available in the project currently.
5. Optionally select or remove icons you want
6. Click "Generate Font"
7. Here you will see a list of all the selected icons and their names

## Using an icon

To use an icon, you just need to add a class with the name of the icon and the "icon-" prefix to an HTML element. 

For example:

```<span class="icon-[name of icon]"></span>```

```<span class="icon-dollar"></span>```