using System;

using Microsoft.Owin.Hosting;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace Owin.AzureAuth.Demo
{
    class Program
    {
        public const string Port = "27898";

        static void Main(string[] args)
        {
            WebApp.Start<Startup>(string.Format("http://+:{0}", Port));

            Console.WriteLine("Started on port " + Port);
            Console.ReadKey();
        }
    }

    internal class Startup
    {
        private const string ApplicationName = "<your domain name here>";
        private const string ClientId = "<your code here>";

        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = ClientId,
                Authority = string.Format("https://login.windows.net/{0}.onmicrosoft.com", ApplicationName),
                PostLogoutRedirectUri = string.Format("http://localhost:{0}/", Program.Port),
            });


            app.Use(async (context, next) =>
            {
                if (context.Authentication.User == null)
                {
                    context.Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
                else
                {
                    await next();
                }
            });

#if DEBUG
            app.UseErrorPage();
#endif
            app.UseWelcomePage("/");
        }
    }
}
