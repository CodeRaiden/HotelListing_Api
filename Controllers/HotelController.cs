using AutoMapper;
using HotelListing_Api.IRepository;
using HotelListing_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace HotelListing_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        // so first thing we need to do here is create a private readonly field to be used to inject the workload done in the UnitOfWork here in the CountryController
        private readonly IUnitOfWork _unitOfWork;
        // And then we will need to create an Ilogger field of type CountryControler, so we can write to the logger file here
        private readonly ILogger<CountryController> _logger;
        // To Mapp the CountryController to the CountryDTO, we will need to inject the AutoMapper dependency here
        private readonly IMapper _mapper;

        // Now we can go on to inject the dependencies here in the "CountryController" via the constructor
        public HotelController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            // then we inject the mapper into the file
            _mapper = mapper;
        }

        // Now here we can go on to include our routes

        // First the Route Function to get all Hotels
        [HttpGet]
        // here we can also Inform swagger of the the expected response type
        // using the Data anotations below. so that swagger does not interpret it as undocumented
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                // here we will create a variable "var" to hold the gotten Hotels and we will make sure that the execution awaits for the hotels to be gotten
                // this is where the unit of work dependency injection comes in handy
                var hotels = await _unitOfWork.Hotels.GetAll();

                // we will include a variable here called "result", which will map the gotten Hotels to the HotelsDTO
                // we will do this by pulling the Map class from the Injected Automapper dependency, and then we will make this map to a list "IList" of type "HotelsDTO"
                // and then we will include the variable "hotels" as the parameter to be mapped to the CountryDTO.
                var results = _mapper.Map<IList<HotelDTO>>(hotels);
                // and if everything goes right we want to return a 200 Ok response for the goetten IList of type CountryDTO store in the "result" variable
                return Ok(results);
            }
            catch (Exception ex)
            {
                // here in the catch, this is where the logger becomes very important. In that we can log the error
                // in the logger file.
                // for example we can say "something went wrong with the name of the method of the "GetCountries""
                _logger.LogError(ex, $"Something went wrong with the {nameof(GetHotels)}");
                // Note that the logger file is specificaly for internal information
                // So for the user information, we will return a 500 inetrnal server error
                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // Next we will create an endpoint for getting a single Hotel element from the database
        // heer the difference is we are going to state to the datanotation decorator that we need to
        // get the country by it's id which should be an integer

        // In order to prevent an Authorized user access to the endpoint we can include the authorized verb
        // Authorize inside the [HttpGet("{id:int}")] data anotation "[HttpGet("{id:int}"), Authorize]", or
        // we can create another data anotation for the Authorize verb "[Authorize]" and that's what we will do.
        // TESTING THE AUTHORIZE ENDPOINT
        
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                // here we will create a variable "var" to hold the gotten Hotel and we will make sure that the execution awaits for the hotel to be gotten
                // And also in the Get function we will include the parameters as defined in the Generic Repository. The first of which is a lamda expression that must
                // return true for the search to be successful, and the other (if added) should be an object of type "Country" to be stored in a list of type string note that
                // the "Country" object name must match with the name defined class name defined in the IUnitOfWork "Country"
                var hotel = await _unitOfWork.Hotels.Get(hotel => hotel.Id == id, new List<string> { "Country" });

                // here we will map a single entity of the CoutryDTO to the country instaed of an Ilist
                var result = _mapper.Map<HotelDTO>(hotel);
                // and if everything goes right we want to return a 200 Ok response for the goetten IList of type CountryDTO store in the "result" variable
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong with the {nameof(GetHotel)}");

                return StatusCode(500, "Internal Server Error. Please Try Again Later.");
            }
        }

        // we can try the Get() method on postman using the link https://localhost:7131/api/Hotel/1

    }
}

// we can go on to commit the progress to Github.
