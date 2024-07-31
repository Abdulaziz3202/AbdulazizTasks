namespace MVCRESTAPI.Services.EsriService
{
    public interface IEsriService
    {
        public Task<Dto.TokenDto> GetEsriToken();
        public  Task<string> QueryFeatures(string token);
    }
}
