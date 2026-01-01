using MercadoPago.Client.Preference;

namespace Payment.Application.Commands
{
    public class teste
    {
        public async Task Execute()
        {
            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = "Test Item",
                        Quantity = 1,
                        UnitPrice = 100.00M
                    }
                }
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(request);
        }
    };
}
