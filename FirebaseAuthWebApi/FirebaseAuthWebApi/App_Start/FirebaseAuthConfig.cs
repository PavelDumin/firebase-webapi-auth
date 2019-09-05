using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using Owin;

namespace FirebaseAuthWebApi
{
    public class FirebaseAuthConfig
    {
        private static readonly string FirebaseValidAudience = ConfigurationManager.AppSettings["FirebaseAuthValidAudience"];
        private static readonly string FirebaseValidIssuer = ConfigurationManager.AppSettings["FirebaseAuthValidIssuer"];

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

        private static Task OnValidateIdentity(OAuthValidateIdentityContext context)
        {
            var calimsIdentity = context.Ticket.Identity;

            //Add custom claim if needed
            calimsIdentity.AddClaim(new Claim("ClaimType", "ClaimValue"));

            return Task.FromResult<int>(0);
        }

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
    }
}
