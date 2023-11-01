using HotelListing_Api.Data;
using Microsoft.EntityFrameworkCore;


// Old, so Ignore
//namespace HotelListing_Api.Data
//{
//    public class DatabaseContext : DbContext
//    {
//        // Eighth
//        // then here we will include a constructor for the class which will itake in a parameter
//        // of type "DbContextOptions"
//        // we will also provide an argument to the constructor of the inherited parent "DbContext" class
//        // here in the child class, by passing in the parameter of type "DbContextOptions" also
//        // into it.

//        public DatabaseContext(DbContextOptions options) : base (options)
//        {

//        }

//        // Nineth
//        // It's here we will actually list out what the database should know about when it is being
//        // generated

//        // first we state here below that we want in the remote Database table, a table("DbSet")
//        // of type "Country" and we will set the name of the table to countries
//        public DbSet<Country> Countries { get; set; }

//        public DbSet<Hotel> Hotels { get; set; }

//        // Tenth
//        // After setting the table fields above, we will need to let our "appsettings.json" know
//        // about the connection string that that outlines how the DatabaseContext gets to/connects to
//        // the remote microsoftsql server database
//    }
//}




//// To be done in the program.cs file but is Old, so Ignore
///
//// Since the DatabaseContext will be fed the argument to it's options parameter from the
//// "program.cs", then the "program.cs" file needs to know that when the application starts up,
//// the appliction should be loading it's database configuration/options
//// from "program.cs" file, using the DatabaseContxt.cs file as the bridge.
//builder.Services.AddDbContext<DatabaseContext>(options =>
//    // the first option is to tell the application to use the SqlServer connection String
//    // defined in the appsettings.json file
//    options.UseSqlServer("sqlConnection")
//);
//// then we will move the controller service below to be the last service added
//// builder.Services.AddControllers();

namespace HotelListing_Api.Data
{
    public class DatabaseContext : DbContext
    {
        // here we will declare the Configuration field of type "IConfiguration"
        protected readonly IConfiguration Configuration;

        // then here we will include a constructor for the class which will take in a parameter
        // of type "IConfiguration"
        // we will also provide an argument to the constructor of the inherited parent "DbContext" class
        // here in the child class, by passing in the parameter of type "DbContextOptions" also
        // into it.
        public DatabaseContext(IConfiguration configuration)
        {
            // here we will set the Configuration field value as the passed in configuration parameter
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server with connection string from app settings
            options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"));
        }

        // Fourteenth
        // SEEDING DATA INTO THE DATABASE
        // what we will do here basically is just hard code some records such that
        // when we perform a migration afterwards the migration will have instructions
        // to create these records, update the database with these records, and won't
        // necessary need to rely on user input.
        // at least this will provide nice basis for our testing when we start developing
        // the API endpoint.
        // So to do all this, first here we will have to override a protected method that is inside of the DbContext called "OnModelCreating()"
        protected override void OnModelCreating (ModelBuilder builder)
        {
            // so here for the data seeding, we will build and Entity of type "Country" in the database using the ModelBuilder class parameter (builder) "Entity" collection and then set the type to "Country"
            // Note that we are building the Country instead of the Hotel entity, because according to what we specified in our data i.e. we need to have a Country to have a Hotel, and this is why we used the Hotel
            // as the ForeignKey in the Country table.
            builder.Entity<Country>().HasData(
                // The "HasData" block here takes in an array. so we will define as many Country objects/instances as needed
                new Country
                {
                    Id = 1,
                    Name = "Jamaica",
                    ShortName = "JAM"
                },
                new Country 
                { 
                    Id =2,
                    Name = "Bahamas",
                    ShortName = "BAH"
                },
                new Country 
                { 
                    Id = 3,
                    Name = "Cayman Islands",
                    ShortName = "CAI"
                }
                ) ;

            // Now that we are done with the seeding of data into the Country table, we can go on
            // to seed data into the ForeingnKey Table "Hotel"
            builder.Entity<Hotel>().HasData(
                // The "HasData" block here takes in an array. so we will define as many Country objects/instances as needed
                new Hotel
                {
                    // Jamaica
                    Id = 1,
                    Name = "Sandals Resort and Spa",
                    Address = "Negril",
                    CountryId = 3,
                    Rating = 4.3,
                },
                new Hotel
                {
                    // Caymen Islands
                    Id = 2,
                    Name = "Comfort Suits",
                    Address = "George Town",
                    CountryId = 3,
                    Rating = 4.5,
                },
                new Hotel
                {
                    // Bahamas
                    Id = 3,
                    Name = "Grand Palladium",
                    Address = "Nassua",
                    CountryId = 2,
                    Rating = 4,
                }
                );
            // After seeding in the data the next thing for us to do will be add a Migration in the PMC using the code below
            // Add-Migration SeedingData
            // the next thing we need to do is to update the database with the added/set Migrations using the code below
            // Update-Database
            // then we can navigate back to our "SQL Server Object Explorer" and there we can navigate to the database tables and right click on each one and click on the "View Data"
        }
        public DbSet<Country> Countries { get; set; }



        public DbSet<Hotel> Hotels { get; set; }
    }
}
