# Google Analytics

[Google Analytics](www.google.com/analytics)

For more information about how to use the Google Analytics dashboard, see the ```Google Analytics screenshots``` folder.

## Snippet

The Google Analytics code snippet can be found in ```GoogleServicesController.js``` for the web app. The Google Analytics ID is generated from the Grunt build.

## Events

We are also using a library called [Angulartics](https://github.com/luisfarzati/angulartics) to register certain events such as Created Orders and Submitted Orders. This library provides certain directives to us.

See this example from ```cartitems.html```

```
analytics-on="click" analytics-category="Orders" analytics-event="Create Order" analytics-label="Cart Page"
```

The Category would be the type of event such as Orders or Invoices. 

Event (this is also called Action) would be the action the user is performing such as Create Order, Submit Order, Pay Invoices. 

Label would provide more detail about the event such as where the event took place From List, From Context Menu, etc.