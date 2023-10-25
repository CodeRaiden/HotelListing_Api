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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// First
// include serilog in the http request pipeline
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

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

builder.Services.AddSwaggerGen();

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

// So now that we have the Logger created we can actually satrt using it
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
