# Firebase ASP.NET WebApi Authentication

A simple sample of incorporating ASP.NET Web API with Google Firebase authentication using OWIN middleware.

## Overview
In order to utilize both Firebase authentication and WebApi authorization filters along with OWIN without using any side libraries we can use UseJwtBearerAuthentication which adds JWT bearer token middleware to the application pipeline:
```cs
public static void Register(IAppBuilder app)
{
    List<X509SecurityKey> issuerSigningKeys = GetIssuerSigningKeys();

    app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
    {
        AuthenticationMode = AuthenticationMode.Active,
        AllowedAudiences = new[] { FirebaseValidAudience },
        Provider = new OAuthBearerAuthenticationProvider
        {
            OnValidateIdentity = OnValidateIdentity
        },
        TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKeys = issuerSigningKeys,
            ValidAudience = FirebaseValidAudience,
            ValidIssuer = FirebaseValidIssuer,
            IssuerSigningKeyResolver = (arbitrarily, declaring, these, parameters) => issuerSigningKeys
        }
    });
}
```

IssuerSigningKeys are generated from Google's X509 certificate metadata:
```cs
private static List<X509SecurityKey> GetIssuerSigningKeys()
{
    HttpClient client = new HttpClient();
    string jsonResult = client.GetStringAsync("https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com").Result;

    //Extract X509SecurityKeys from JSON result
    List<X509SecurityKey> x509IssuerSigningKeys = JObject.Parse(jsonResult)
                        .Children()
                        .Cast<JProperty>()
                        .Select(i => BuildSecurityKey(i.Value.ToString())).ToList();
    
    return x509IssuerSigningKeys;
}

private static X509SecurityKey BuildSecurityKey(string certificate)
{
    //Removing "-----BEGIN CERTIFICATE-----" and "-----END CERTIFICATE-----" lines
    var lines = certificate.Split('\n');
    var selectedLines = lines.Skip(1).Take(lines.Length - 3);
    var key = string.Join(Environment.NewLine, selectedLines);

    return new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(key)));
}
```

And finally controller's action could be secured using **[Authorize]** attribute and get all available user data from the Identity:
```cs
[Authorize]
[HttpPost]
public void Post(object value)
{
    ClaimsIdentity userIdentity = User.Identity as ClaimsIdentity;
    string userEmail = value.ToString();
}
```

## Usage
Assuming that the Firebase account exists just update appropriate settings in Web.config and Login.html files.

Replace your Firebase project id in **Web.config**:
```xml
<appSettings>
  <!--Firebase authentication settings
  FirebaseAuthValidAudience is a Firebase Project Id
  FirebaseAuthValidIssuer is a https://securetoken.google.com/ + Firebase Project Id
  -->
  <add key="FirebaseAuthValidAudience" value="your_firebase_project_id" />
  <add key="FirebaseAuthValidIssuer" value="https://securetoken.google.com/your_firebase_project_id" />
</appSettings>
```

Replace your Firebase project id, API key and sender id in **Login.html**:
```js
// Initialize Firebase
// TODO: Replace with your project's customized code snippet
var config = {
	apiKey: "your_firebase_api_key",
	authDomain: "your_firebase_project_id.firebaseapp.com",
	databaseURL: "https://your_firebase_project_id.firebaseio.com",
	projectId: "your_firebase_project_id",
	storageBucket: "",
	messagingSenderId: "your_firebase_sender_id"
};
```

Then just run the solution and enjoy!
