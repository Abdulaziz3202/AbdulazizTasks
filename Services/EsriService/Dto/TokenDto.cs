namespace MVCRESTAPI.Services.EsriService.Dto
{
    public class TokenDto
    {
        public string token { get; set; }
        public double expires { get; set; }
        public bool ssl { get; set; }
    }
}
