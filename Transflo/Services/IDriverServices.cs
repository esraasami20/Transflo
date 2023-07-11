using Transflo.Helper;
using Transflo.Models;
using Transflo.ViewModel;

namespace Transflo.Services
{
    public interface IDriverServices
    {
        public Task<IEnumerable<Driver>> GetDrivers();
        public Task<Driver> GetDriver(int id);
        public Task<Response> CreateDriver(Driver driver);
        public Task UpdateDriver(int id, Driver driver);
        public Task DeleteDriver(int id);
        public Task<bool> CheckEmail(string email);
        public Task<PagedPagination<List<Driver>>> GetDriversPaging(PaginationFilter filter, HttpRequest request);


    }
}
