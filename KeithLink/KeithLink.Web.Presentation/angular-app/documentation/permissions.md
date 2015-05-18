# Permissions

A user has access to certain parts of the site based on their role and other flags.

Permissions are grouped together by functionality (canManageLists, canViewInvoices, etc.) which are defined in the ```AccessService```. The permissions are then set on the $scope in the ```MenuController``` to be used throughout the site to show/hide various features. 

Individual states can also be locked down by functionality. In the ```state.js``` file, some states have authorize properties so only a user with permission to access that functionality can view that state. These properties must correspond to a method in the ```AccessService```. This check is performed in the ```app.js``` file.

In the ```state.js``` file:

```
data: {
  authorize: 'canBrowseCatalog'
}
``` 
