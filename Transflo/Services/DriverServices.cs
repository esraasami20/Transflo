using Dapper;
using Transflo.Database;
using Transflo.Helper;
using Transflo.Models;
using Transflo.ViewModel;

namespace Transflo.Services
{
    public class DriverServices : IDriverServices
    {
        private readonly DriverContext _context;
        private readonly IUriService _uriService;

        public DriverServices(DriverContext context, IUriService uriService)
        {
            _context = context;
            _uriService = uriService;
        }

        public async Task<bool> CheckEmail(string email)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(email))
            {
                var query = "SELECT * FROM Driver WHERE Email = @Email";
                using (var connection = _context.CreateConnection())
                {
                    var driver = await connection.QuerySingleOrDefaultAsync<Driver>(query, new { email });
                    if (driver != null)
                        result = true;
                }
            }
            return result;
        }

        public async Task<Response> CreateDriver(Driver driver)
        {
            var query = @"INSERT INTO Driver (FirstName, LastName, Email, PhoneNumber) VALUES 
                        (@FirstName, @LastName, @Email, @PhoneNumber)
                        SELECT CAST(SCOPE_IDENTITY() as int)";
            
            var driverExistance = CheckEmail(driver.Email).Result;
            if (!driverExistance)
            {
                var parameters = new
                {
                    FirstName = driver.FirstName,
                    LastName = driver.LastName,
                    Email = driver.Email,
                    PhoneNumber = driver.PhoneNumber
                };
                using (var connection = _context.CreateConnection())
                {
                    var id = await connection.QuerySingleAsync<int>(query, parameters);
                    var createdDriver = new Driver
                    {
                        DriverId = id,
                        FirstName = driver.FirstName,
                        LastName = driver.LastName,
                        Email = driver.Email,
                        PhoneNumber = driver.PhoneNumber
                    };
                    return new() { Status = "Sucess", Message = "Driver Added Successfully", data = createdDriver } ;
                }
            }
            return new() { Status = "", Message = "Driver email already Exist" };
        }

        public async Task<PagedPagination<List<Driver>>> GetDriversPaging(PaginationFilter filter, HttpRequest request)
        {
            var route = request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = await GetDrivers();
            var totalRecords = pagedData.Count();
            pagedData = pagedData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToList();
            return PaginationHelper.CreatePagedReponse<Driver>(pagedData.ToList(), validFilter, totalRecords, _uriService, route);
        }

        public async Task DeleteDriver(int id)
        {
            var query = "DELETE FROM Driver WHERE DriverId = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<Driver> GetDriver(int id)
        {
            var query = "SELECT * FROM Driver WHERE DriverId = @Id";
            using (var connection = _context.CreateConnection())
            {
                var driver = await connection.QuerySingleOrDefaultAsync<Driver>(query, new { id });
                return driver;
            }
        }

        public async Task<IEnumerable<Driver>> GetDrivers()
        {
            var query = "SELECT * FROM Driver";
            using (var connection = _context.CreateConnection())
            {
                var drivers = await connection.QueryAsync<Driver>(query);
                return drivers.ToList();
            };
        }

        public async Task UpdateDriver(int id, Driver driver)
        {
            var query = @"UPDATE Driver SET FirstName = @FirstName, LastName = @LastName,
                        Email = @Email, PhoneNumber = @PhoneNumber WHERE DriverId = @DriverId";
            var parameters = new
            {
                DriverId =  id,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                Email = driver.Email,
                PhoneNumber = driver.PhoneNumber
            };
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
