using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Security.Claims;
using System.Collections;
using System.Threading.Tasks;

namespace validate_jwt
{
    class Program
    {
        static string authority = "https://login.microsoftonline.com/swearjarbank.onmicrosoft.com";
        static string clientId = "202e7978-0008-4c07-8c54-440c9be7cb94"; // AConsoleApp
        // static string resourceId = "https://graph.microsoft.com"; // Won't validate, JWT comes with a nonce in header
        // also you should _never_ validate tokens that aren't yours to consume.
        
        // App Service Easy Auth needs AppId to validate token
        static string resourceId = "c0b94ebd-7c76-4fa7-ade9-00047b5feb75"; // swearjarbank.azurewebsites.net
        static Uri redirectUri = new Uri("http://unnecessary");

        static async Task Main()
        {
            // Flip to true for ADAL logging -
            // it's pretty verbose thus pretty handy.
            LoggerCallbackHandler.UseDefaultLogging = false;

            AuthenticationContext context = new AuthenticationContext(authority);    
            DeviceCodeResult codeResult = await context.AcquireDeviceCodeAsync(resourceId, clientId);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\nUsing OAuth 2.0 Device Flow. You need to sign in.");
            Console.WriteLine(codeResult.Message + "\n");
            Console.ResetColor();
            AuthenticationResult result = await context.AcquireTokenByDeviceCodeAsync(codeResult);

            // Get the JWT bearer token from the authorization header
            // Use https://jwt.ms to decode
            string accessToken = result.AccessToken;
            string accessTokenType = result.AccessTokenType;
            Console.WriteLine("Azure AD returned this JWT:");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{accessTokenType} {accessToken}\n");
            Console.ResetColor();

            // Get tenant information to validate incoming JWT tokens
            string stsDiscoveryEndpoint = $"{authority}/.well-known/openid-configuration";
            OpenIdConnectConfigurationRetriever configRetriever = new OpenIdConnectConfigurationRetriever();
            ConfigurationManager<OpenIdConnectConfiguration> configManager = 
                new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, configRetriever);
            OpenIdConnectConfiguration config = await configManager.GetConfigurationAsync();

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidIssuer = config.Issuer, // gets pulled from https://login.microsoftonline.com/swearjarbank.onmicrosoft.com/.well-known/openid-configuration
                ValidateIssuer = true, // compares token issuer claim with https://login.microsoftonline.com/swearjarbank.onmicrosoft.com/.well-known/openid-configuration
                ValidAudience = resourceId,
                // or ValidAudiences = new [] { resourceId, "https://2ndresource" },
                ValidateAudience = true, // compares aud claim to ValidAudience
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = config.SigningKeys,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            // VALIDATE TOKEN
            // https://stackoverflow.com/a/39870281/4148708
            // "There are two steps to verity the token.
            // First, verify the signature of the token to ensure the token
            // was issued by Azure Active Directory. Second verify the claims
            // in the token based on the business logic."

            SecurityToken validatedToken = new JwtSecurityToken();
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal validationResult = null;

            try
            {
                validationResult = tokenHandler.ValidateToken(accessToken, validationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("TOKEN DID NOT PASS VALIDATION LOGIC.");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadKey();
                Environment.Exit(-1);
            }

            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("TOKEN IS VALID.");
            Console.ResetColor();
            Console.WriteLine($"Issuer: {validatedToken.Issuer}\n" +
                $"ValidFrom: {validatedToken.ValidFrom}\n" +
                $"ValidTo: {validatedToken.ValidTo}\n" +
                $"Public Signing Key: {validatedToken.SigningKey.KeyId}\n\n" +
                "Claims:");
            IEnumerator claims = validationResult.Claims.GetEnumerator();
            while (claims.MoveNext())
            {
                Console.WriteLine($"  {claims.Current}");
            }
            Console.ReadKey();
       }
    }
}