using WebApplication1.Data.DTO;

namespace WebApplication1.Services.IServices
{
    public interface IAddressService
    {
        public Task<List<SearchAddressModel>> Search();
    }
}
