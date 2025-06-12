namespace Mango.Services.OrderAPI.Models.Dto
{
    public class Authorization
    {
        public string AuthorizationCode { get; set; }
        public string Bin { get; set; }
        public string Last4 { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Channel { get; set; }
        public string CardType { get; set; }
        public string Bank { get; set; }
        public string CountryCode { get; set; }
        public string Brand { get; set; }
        public bool Reusable { get; set; }
        public string Signature { get; set; }
        public string AccountName { get; set; }
    }
}
