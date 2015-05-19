# API

Documentation for available api endpoints can be found at:
[https://shopapidev.benekeith.com/swagger](https://shopapidev.benekeith.com/swagger)

## Headers

Most api calls require certain headers to be set on the request. These headers are set in the ```scripts/services/AuthenticationInterceptor.js``` file.

```apiKey```: Required by all api calls. The api key varies by environment and is determined by the Grunt build task which sets the value in the configenv.js file.

```Authorization```: This is the token for the logged in user using the following format. This header is not required for all api calls. The exclusions are listed in the Authentication Interceptor.

```
Bearer <user_token>
```

```userSelectedContext```: This contains the selected customer number and branch ID. This header is not required for all api calls. The exclusions are listed in the Authentication Interceptor.

```
{
	customerid: 111111,
	branchid: "FAM"
}
```