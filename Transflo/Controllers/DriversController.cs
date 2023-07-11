using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Transflo.Models;
using Transflo.Services;
using Transflo.ViewModel;

namespace Transflo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly IDriverServices _driverService;
        public DriversController(IDriverServices driverService)
        {
            _driverService = driverService;
        }
        //List all Drivers
        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            try
            {
                var drivers = await _driverService.GetDrivers();
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //Get Driver By Id
        [HttpGet("{id}", Name = "DriverById")]
        public async Task<IActionResult> GetDriver(int id)
        {
            try
            {
                var driver = await _driverService.GetDriver(id);
                if (driver == null)
                    return NotFound();

                return Ok(driver);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //List all Drivers Paging
        [HttpGet]
        [Route("GetPaging")]
        public async Task<IActionResult> GetDriversPaging([FromQuery] PaginationFilter filter)
        {
            try
            {
                var result = await _driverService.GetDriversPaging(filter, Request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        //New Driver
        [HttpPost]
        public async Task<IActionResult> CreateDriver(Driver driver)
        {
            try
            {
                if(driver == null)
                    return BadRequest();
                var createdDriver = await _driverService.CreateDriver(driver);
                return Ok(createdDriver);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("ExportData")]
        //Export data for driver
        public FileResult Export()
        {
            string[] columnsNames = new string[] { "Driver Id", "Driver Full Name", "Driver Email", "Driver Phone" };
            var drivers = _driverService.GetDrivers().Result;
            string csv = string.Empty;
            foreach (var columnsName in columnsNames)
            {
                csv += $"{columnsName} , ";
            }
            csv += "\r\n";
            foreach (var driver in drivers)
            {
                csv += $"{driver.DriverId} , ";
                csv += $"{driver.FirstName} {driver.LastName} , ";
                csv += $"{driver.Email} , ";
                csv += $"{driver.PhoneNumber} , ";
                csv += "\r\n";
            }
            byte[] bytes = Encoding.ASCII.GetBytes(csv);
            return File(bytes, "text/csv", $"Driver{DateTime.Now}.csv");
        }
        //Update Driver data
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(Driver driver)
        {
            try
            {
                if(driver == null) 
                    return BadRequest();
                var dbDriver = await _driverService.GetDriver(driver.DriverId);
                if (dbDriver == null)
                    return NotFound();

                await _driverService.UpdateDriver(driver.DriverId, driver);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        //Delete Driver data
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            try
            {
                var dbCompany = await _driverService.GetDriver(id);
                if (dbCompany == null)
                    return NotFound();

                await _driverService.DeleteDriver(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
