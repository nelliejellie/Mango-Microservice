namespace Mango.Services.OrderAPI.Models.Dto
{
    public class CustomerDto
    {
            public long Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string CustomerCode { get; set; }
            public string Phone { get; set; }
            public object Metadata { get; set; }
            public string RiskAction { get; set; }
            public string InternationalFormatPhone { get; set; }

    }
}
