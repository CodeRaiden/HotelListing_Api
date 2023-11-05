using AutoMapper;
using HotelListing_Api.IRepository;
using HotelListing_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing_Api.Controllers
{
    // Now here there are basically two types of routing mechanisms that we can use during API development(at least MVC API development)
    // and they are:
    // 1. The Convention Based Routing:
    // In this type of Routing we will have to go to the program.cs file and there we will have to configure in "app.UseEndpoints();" parameter, just beneath the "app.UseAuthorization();" line code
    // But the problem with this here is we will need to be very specific with our route, where we will specify the path to the exact endpoint
    // this will be okay for an MVC application, but for REAST API standard application, this will require the verb
    // to determine what it is we will be doing with the route. And so we will be making use of "Attribute Routing" instead the "Convention Based Routing".

    // Attribute Routing
    // here, in specifying the route we will only need to use the verb as defined in the Route parameter below "api/[controller]" where "controller" would signify the particular controller we are dealing with
    // i.e. "Country" or "Hotel".
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        // so first thing we need to do here is create a private readonly field to be used to inject the workload done in the UnitOfWork here in the CountryController
        private readonly IUnitOfWork _unitOfWork;
        // And then we will need to create an Ilogger field of type CountryControler, so we can write to the logger file here
        private readonly ILogger<CountryController> _logger;
        // To Mapp the CountryController to the CountryDTO, we will need to inject the AutoMapper dependency here
        private readonly IMapper _mapper;

        // Now we can go on to inject the dependencies here in the "CountryController" via the constructor
        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            // then we inject the mapper into the file
            _mapper = mapper;
        }

        // Now here we can go on to include our routes

        // First the Route Function to get all Countries
        [HttpGet]
        // here we can also indicate the response types which at the same time also Informs swagger of the expected response type
        // using the Data anotations below. so that swagger does not interpret it as undocumented
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                // here we will create a variable "var" to hold the gotten Countries and we will make sure that the execution awaits for the countries to be gotten
                // this is where the unit of work dependency injection comes in handy
                var countries = await _unitOfWork.Countries.GetAll();

                // we will include a variable here called "result", which will map the gotten Countries to the CountryDTO
                // we will do this by pulling the Map class from the Injected Automapper dependency, and then we will make this map to a list "IList" of type "CountryDTO"
                // and then we will include the variable "countries" as the parameter to be mapped to the CountryDTO.
                var results = _mapper.Map<IList<CountryDTO>>(countries);
                // and if everything goes right we want to return a 200 Ok response for the goetten IList of type CountryDTO store in the "result" variable
                return Ok(results);
            }
            catch (Exception ex)
            {
                // here in the catch, this is where the logger becomes very important. In that we can log the error
                // in the logger file.
                // for example we can say "something went wrong with the name of the method of the "GetCountries""
                _logger.LogError(ex, $"Something went wrong with the {nameof(GetCountries)}");
                // Note that the logger file is specificaly for internal information
                // So for the user information, we will return a 500 inetrnal server error
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }
        
        // Next we will create an endpoint for getting a single Country element from the database
        // here the difference is we are going to state to the datanotation decorator that we need to
        // get the country by it's id which should be an integer
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                // here we will create a variable "var" to hold the gotten Country and we will make sure that the execution awaits for the countries to be gotten
                // And also in the Get function we will include the parameters as defined in the Generic Repository. The first of which is a lamda expression that must
                // return true for the search to be successful, and the other (if added) should be an object of type "Hotels" to be stored in a list of type string note that
                // the "Hotels" object name must match with the name defined class name defined in the IUnitOfWork "Hotel"
                var country = await _unitOfWork.Countries.Get(country => country.Id == id, new List<string> { "Hotels" });

                // here we will map a single entity of the CoutryDTO to the country instaed of an Ilist
                var result = _mapper.Map<CountryDTO>(country);
                // and if everything goes right we want to return a 200 Ok response for the goetten IList of type CountryDTO store in the "result" variable
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong with the {nameof(GetCountry)}");
                
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // we can try the Get() method on postman using the link https://localhost:7131/api/Country/1

    }


    // Now we can go on to create Hotel API Controller
    
}

// Now we can go register the Controller in the boot strapper "program.cs" file, just below the "builder.Services.AddAutoMapper(typeof(MapperInitializer));" line code.