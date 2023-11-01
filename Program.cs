// Second
// we make sure that the using statement for serilog library is included
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.AspNetCore;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelListing_Api.Data;
using Microsoft.EntityFrameworkCore;
using HotelListing_Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// First
// include serilog in the http request pipeline
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Twelveth
// Now we will go and include the fields for our Country and Hotel classes in the
// Country.cs and Hotel.cs files

// Thirteenth
// we will go to the Package manager console by clicking on "tools", and then clicking on
// "nuget package manager" and then on "package manager console"
// And there is where the EntityFramework.Tools comes in, as this will allow us to scafold/create our
// database in the remote server
// the first thing we need to do in the PMC is to add a Migration name "DatabaseCreated" using the code below
// Add-Migration DatabaseCreated
// when we run this command in the PMC, we will find that a new folder has been created called
// Migrations, which contains a file with a snapshot of what each instance/object(i.e Country and Hotel objects)
// will look like in the database when it is created.
// So next we will create the Database using the code below
// Update-Database
// Now to check the creeated database, we will need to click on "View" above and click on
// "Sql Server Object Explorer"
// then there we will expand the in built "(localdb)\MSSQLLOCALDB" and
// then we will expand the "Databases", and then we expand the created "Hotellisting_db" database


// Fourteenth
// SEEDING DATA INTO THE DATABASE
// To seed data into the database we will navigate to the DatabaseContext.cs file


// Fifteenth
// SET UP SERVICE REPOSITORIES AND DEPENDENCY INJECTION
//To get this started we are going to be creating two new folders "IRepository" and "Repository"
//we will be using the "Separation of Concerns" concept, where we want to make sure that every file is responsible for only a particular task
//to keep things Generic, making sure there is no repeatition of anything

//So in the "IRepository" folder, we will create a public Interface, and we will call it "IGenericRepository"

// Sixteenth
// SETTING UP DTO (DATA TRANSFER OBJECT) AND USING AUTOMAPPER TO AUTOMATE
// THE PROCESS OF LINKING OUR DTO TO THE DOMAIN OBJECTS
// So then we can think of DTO's as a midle layer which will enforce certain validations at the frontend part of the application amongst other things
// So we can use DTO to sanitize our data before it actually gets over to our actual data Class (Country or Hotel and via extension, our database)
// To create the DTO's to interface with our classes, first we need to create a folder called Models which will contain the DTO Models of our actual data Class
//(Country or Hotel and via extension, our database).
//Now Inside of the Models folder, we generally have a number of classes that represents each variation of a request relative to each Domain Object (Country and Hotel)

// Sixth
// here to configure the CORS (Cross Origin Resourse Sharing), we will first add the
// service, and then in it's parenthesis we will add the policy/configuration on how we
// want it to behave using a lambda expression as done here below
builder.Services.AddCors(cor =>
{
    // the first configuration here will be to build the policy using the ".AddPolicy" method
    // which takes in a customized policy name (in this case "AllowAll") as it's fist parameter , and then
    // a lambda expiression to buld the policy in which we can set the policy to alllow/do a list of
    // things in the properties of the "build" name variable("builder" in this example case) provided
    cor.AddPolicy("AllowAll", builder =>
        // here since we will be developing an API that can be acessed in the web by anybody who
        // needs it, then we will use the "AllowAnyOrigin()" which makes it possible for anybody
        // on the web to access it. and with allowing any origin we will also make sure that the
        // accessing origin can also gain access using every Method/request the API is set to serve.
        // we will do this by chainning the "AllowAnyMethod()" method with the CorsPolicyBuilder class' "AllowAnyOrigin()" method.
        // and also for the API to accept any Method/Request Header, we will also chain the CorsPolicyBuilder class' "AllowAnyHeader()" method
        // to the list of allowed access functionalities as seen below
        builder.AllowAnyOrigin().
        AllowAnyMethod().AllowAnyHeader()
    // but remember that context of the API you are building is what determines how strict you are with the accessibility of your API
    // So after doing all this we will have to go down the file to the "Confiure/app.Use" section, in order
    // to let the application know that it should use the set cors policy "AllowAll".
    );
});

// Add the AutoMapper Service of Type "MapperInitializer" for the Mapping between the Domain Classes and the DTO Models
// when we start developing our endpoints we will actually get to see the power of AutoMapper and how the DTO's work and
// how everything relates to the Data Classes
// So getting the configurations of the Domain Classes, The DTO Models and the AutoMapper out of the way, is very important for that.
builder.Services.AddAutoMapper(typeof(MapperInitializer));
// So with this milestoe crossed we can now check in the Migation to Github
// to do this we can click on the "Git Changes" next to "Solutons Explorer"
// or if the option is missing, you can click on "View" and navigate to the "Git Changes" option from there
// and after including the Migration comment in the text box, you can commit the changes by clicking on the
// "Commit All" drop box arrow and then select ""

builder.Services.AddSwaggerGen();

////placing the AddControllers service as the last service added to our file
//builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Fourth
    // here the "app.UseSwagger" and "app.UseSwaggerUI()" code below means that we will use swagger
    // only when in the development environment.
    // but Swagger is extremely useful also for documentation during the production stage.
    // And so we will take swagger out of this code block to display it also in the production environment
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

// Ensuring that swagger works both on development and production envionment
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Seventh
// Ensuring that the application knows that it should use the set cors policy "corspolicy"
app.UseCors("AllowAll"); 

app.UseAuthorization();

app.MapControllers();

// Third
// here we will need to modify the main application which is going to initialize our logger
// when the application starts up. And here we are going to put in some test log scenarios right here
// in the main section as our test cases.
// the firstly here is to create the "Log" variable, and we will make it point to the inherited class
// "LoggerConfiguration()" instance/object. This will help us setup some default and expected
// behaviours. we will put a line break for each configuration added just to make things simple
Log.Logger = new LoggerConfiguration()
    // the first configuration would be to confgure where we want the information written to
    // which we will set to write to a file
    .WriteTo.File(
        path: @"C:\Users\HP\Desktop\HotelListings\logs\log-.txt",
        // the next will be the output template, i.e how we want each line to look
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        // the next parameter will be the "rollingInterval", which means at what interval we will want to
        // create a new file. "Note that the interval is represented by the hyphe "-" in the "log-.txt" file
        // where we can set maybe a date as the rollingInterval using "RollingInterval.Day".
        // this makes it easier to keep track of the log file of a specific day to be reviewed latter
        rollingInterval: RollingInterval.Day,
        // next parameter would be the restricted minimum level, which is to set a limit to what is logged
        // in the file. So here we only want to log Event level infomation, so we will set it to that by
        // first including the "Serilog.Event" using statement above
        restrictedToMinimumLevel: LogEventLevel.Information
        // so after including all the configuration necessary here, we can then go ahead to create the
        // logger using the ".CreateLogger();"as seen below
        ).CreateLogger();

// So now that we have the Logger created we can actually start using it
// to do this we are going to wrap the appication "app.run();" in a try catch
try
{
    // but first we are going to log an information to the file stating that the application has
    // started. Just so we can see what time and when the application started
    Log.Information("The Application Is Starting");
    app.Run();
}
catch (Exception ex)
{
    // then we will go on to write the caught exception to the file using Log.Fatal() method
    // which takes in the exception object as well as a string containing a customized message
    Log.Fatal(ex, "Application Failed to Start");
    // so basically what happens here is that when an exception is caught, the Logger will format it
    // according to what we specified in the logger configuration before logging it to the file
}
// we will also include a finally block here to make sure that we close and flush the log session
// after we are done whether there was an exception or not
finally
{
    Log.CloseAndFlush();
}

// Fifth
// we will now take a look at the controller file to see an
// Example of SHOWING HOW LOGGING MESSAGES WITH SWAGGER WORKS
