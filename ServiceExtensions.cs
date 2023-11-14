using HotelListing_Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;

namespace HotelListing_Api
{
    // we have to make the class a static class
    public static class ServiceExtensions
    {
        // we will ceate a static void function to take care of the configuration of the Identity
        //service
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            // then we will create the builder variable which is basically an amalgamation of all the services which we define in the 
            var builder = services.AddIdentityCore<ApiUser>(I => I.User.RequireUniqueEmail = true);

            // now we will set the builder variable to hold an instance of the IdentityBuilder inorder to build an object of the defined Identity
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);

            // then we will inform the file on where the builder object should be stored using the AddEntityFrameworkStores of type <DatabaseContext>
            // since we want to store this in the Database, so we made the AddEntityFrameworkStores relative to the databaseContext. and then from this
            // we will chain this with the "AddDefaultTokenProviders()" method as well
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
            // now that we have this method in this class, we can now go over to the program.cs class and there include the Authentication() and ConfigureIdentity() methods

        }


        // here we will add another method to hold the configurations for the JWT which we will call "ConfigureJWT()"
        // and in the method parameters we are taking an argument of type "this IServiceCollection" to give us access to the
        // services in our program.cs file here, and the second argument will be of type IConfiguration which will give us
        // access to the settings in the appsettings.json file here in the "ServiceExtensions.cs" file.
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // first we will get the jwt settings using the IConfiguration type method "GetSection()"
            var jwtSettings = configuration.GetSection("Jwt");
            // then we will get the jwt key stored in our Systems Environment using the Environment class type
            // which gives us access to our Systems Enviroonment and then we will use the "GetEnvironmentVariable()"
            // method and store it in a variable to get the set JWT key named "KEY" here the ServiceExtension.cs file 
            var key = configuration.GetSection("Jwt:Key").Value;
            //var key = jwtSettings.GetSection("Key").Value;

            // Next we want to add the Authentication configuration to the service
            services.AddAuthentication(opt =>
            {
                // here for the Default Authenticate Scheme, we will need to get the nuget package "Microsoft.AspNetCore.Authentication.JwtBearer"
                // and then set the option's "DefaultAuthenticateScheme" property to the JwtBearer's default
                // Authentication Scheme.
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // and also what ever information comes across the API system, we also want to challenge it with the JwtBearer's default
                // Authentication Scheme
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                // we are also going to chain the defined AddAuthentication configuration here with the
                // AddJwtBearer configuration as done below
            })
            .AddJwtBearer(opt =>
            {
                // there are quite a few Parameters that we can set up along the way, but the one's included here is just
                // for our API example. but in an enterprise setting(situation), you may have other needs than the one's used here.

                // so here we are going to add some of the parameters that this Token is going to use to validate a registered user
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    // we will include the validation of the Token Issuer, since we went through the trouble of adding the "issuer" to
                    // the "jwt" settings. And so with this if the Token coming in with the a user's request do not match our
                    // set issuer "HotelListing_Api" defined in our appsettings.json file, then we will not be granting the request
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // we want to validate the life time of the Token. setting this to true will reject a valid Token once it is expired.
                    ValidateLifetime = true,
                    // another we will like to do is validate the issuer's singn in key (that is the key set in the Environment variable "KEY")
                    ValidateIssuerSigningKey = true,
                    // here we are going to set the valid issuer for any given Jwt Token must be the issuer defined in the "jwt" setting in the
                    // appsettings.json file which we have stored here in the variable "jwtSettings"
                    // for for this we will set the value to be the jwtSettings variable "jwt" settings "issuer" value as done below
                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validIssuer").Value,
                    // here we will encode the Issuer Signing Key by passing in the variable "key" where the JWT key is stored into the parameter below,
                    // and then hashing it again afterwards
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }
    }
}
