using Stripe;
using Stripe.Infrastructure;

namespace StripeTestProj
{
    public class MakePayment
    {
        public static async Task<string> MakePaymentAsync(string CardHolderName,string CreditCardNumber, string CVV, int Year, int Month)
        {

            // my stripe key -- now works
            StripeConfiguration.ApiKey = "sk_test_51KiHJmL8N3Ehe91s6qoNHp666ZiirG7ww7o6sDsta72FXqwPwMUCZH4DRuqnEUOiptF6w10KxLRNqiuXbkgzb41C00KWvtDjLh";

            //previously implemented -- working api key
            //StripeConfiguration.ApiKey = "sk_test_51K7nWTIVmDZlNVvOTIeQqyyox39ACHjvCI3YyLeLipchkxTYhOjrldDkakpApq7aOK3xOmdBBS2BpeZLd9cXHGuu00wsiKCt3U";


            //create an object of credit card to generate token

            
            var options = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = CreditCardNumber,                    
                    ExpMonth = Month,
                    ExpYear = Year,
                    Cvc = CVV,
                },
            };
            var service = new TokenService();
            Token stripetoken = null;
                
            try
            {
                stripetoken = service.Create(options);
                stripetoken = await service.CreateAsync(options);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            // for creating a customer -- this will reflect in stripe
            CustomerCreateOptions customer = new CustomerCreateOptions();

            customer.Name = "Ayush Company";

            customer.Email = "ayush.mathur@yopmail.com";

            customer.Description = "ASCA Membership";

            customer.Source = stripetoken.Id;

            var custService = new CustomerService();

            Customer stpCustomer = custService.Create(customer);

            // instead of charging we can make an setup intent

            var optionsSetupIntent = new SetupIntentCreateOptions 
            {
                Customer = stpCustomer.Id,
                PaymentMethodTypes = new List<string> { "bancontact", "card", "ideal" },
                
            };

            var serviceSetupIntent = new SetupIntentService();
            var setupIntent= serviceSetupIntent.Create(optionsSetupIntent);
            return setupIntent.Status +" Customer ID: "+stpCustomer.Id;           
        }
        public static string chargeTheCustomer(string custId) 
        {
            string res = "";
            StripeConfiguration.ApiKey = "sk_test_51KiHJmL8N3Ehe91s6qoNHp666ZiirG7ww7o6sDsta72FXqwPwMUCZH4DRuqnEUOiptF6w10KxLRNqiuXbkgzb41C00KWvtDjLh";

            //list the payment methods -
            var optionsPML = new PaymentMethodListOptions
            {
                Customer = custId,
                Type = "card",
            };
            var servicePML = new PaymentMethodService();
            StripeList<PaymentMethod> paymentmethods = servicePML.List(optionsPML);

            var firstPaymentMethodID=paymentmethods.ToArray()[0].Id;

            //trying to fetch customer
            var customerfetchServ = new CustomerService();
            var actCustomer= customerfetchServ.Get(custId);
            

            // create payment intent with the customer id 
            // if  payment confirm is true -- it will make the payment instantaneous and set payment intent status to confirmation
            // if offsession is true --to indicate that the customer is not in your checkout flow during this payment

            var options = new PaymentIntentCreateOptions
            {
                Customer = custId,
                Amount = 1099,
                Currency = "usd",
                Confirm=true,
                OffSession=true,
                PaymentMethod=firstPaymentMethodID,
                ReceiptEmail=actCustomer.Email,
            };
            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            // charge the customer with id
            res = paymentIntent.Status;          
            return $@"{res} , Recipt Email : {paymentIntent.ReceiptEmail}";

        }

    }
}