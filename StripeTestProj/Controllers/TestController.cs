using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace StripeTestProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet("TestStripeIntegration")]
        public async Task<string> TestStripeIntegration(PaymentModal model) 
        {          
            return await MakePayment.MakePaymentAsync(model.CardHolderName,model.CreditCardNumber,model.CVV,model.Year,model.Month);
        }


        [HttpGet("ChargeCustomerWithId")]
        public string TestStripeIntegration(string custid)
        {
            return MakePayment.chargeTheCustomer(custid);
        }

        [HttpGet("TestStripeIntent")]
        public IActionResult TestStripeIntent() 
        {
            StripeConfiguration.ApiKey = "sk_test_51KdC1QSDfXXwbh5U3d9KiLpbIsMebB5obI3UhtRFRzXqF8saD0Xst6oqHJGauZ5fesyH1UEdzRkrNRF3sWyVHMB700z0O2cfXf";

            var optionsCustomer = new CustomerCreateOptions { };

            var serviceCustomer = new CustomerService();
            var customer = serviceCustomer.Create(optionsCustomer);

            var custId = customer.Id;

            var optionsIntent = new SetupIntentCreateOptions
            {
                Customer = "{{CUSTOMER_ID}}",
                PaymentMethodTypes = new List<string> { "card" },
            };
            var serviceIntent = new SetupIntentService();
            serviceIntent.Create(optionsIntent);

            return Ok();
        }




    }
}
