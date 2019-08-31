# Firebase ASP.NET WebApi Authentication

A simple sample of incorporating ASP.NET Web API with Google Firebase authentication using OWIN middleware.

## Overview
In order to utilize both Firebase authentication and WebApi authorization filters along with OWIN without using any side libraries we can use UseJwtBearerAuthentication which adds JWT bearer token middleware to the application pipeline:
![](https://github.com/PavelDumin/firebase-webapi-auth/blob/master/Docs/Media/JwtBearerAuthentication.jpg)

IssuerSigningKeys are generated from Google's X509 certificate metadata:
![](https://github.com/PavelDumin/firebase-webapi-auth/blob/master/Docs/Media/X509IssuerSigningKeys.jpg)

And finally controller's action could be secured using **[Authorize]** attribute and get all available user data from the Identity:
![](https://github.com/PavelDumin/firebase-webapi-auth/blob/master/Docs/Media/SecuredController.jpg)

## Usage
Assuming that the Firebase account exists just update appropriate settings in Web.config and Login.html files.

Replace your Firebase project id in **Web.config**:
![](https://github.com/PavelDumin/firebase-webapi-auth/blob/master/Docs/Media/WebConfigSettings.jpg)

Replace your Firebase project id, API key and sender id in **Login.html**:
![](https://github.com/PavelDumin/firebase-webapi-auth/blob/master/Docs/Media/LoginHtmlSettings.jpg)

Then just run the solution and enjoy!
