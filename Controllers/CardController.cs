using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Hosting.Internal;
using PayPal.Api;
using Seminar_1.Models;

namespace Seminar_1.Controllers
{
    [Route("[Controller]")]
    public class CardController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly Seminar1Context context;
        private IConfiguration configuration;
        private Payment payment;

        public CardController(IHttpContextAccessor httpContextAccessor, Seminar1Context context, IConfiguration configuration)
        {      
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var prodList = context.Products.Select(p => new ProductVM().ProdToProdVM(p)).ToList();
            var shopList = HttpContext.Session.Get<Dictionary<int,int>>(SessionHelper.ShoppingCart);
            if (shopList == null)
                shopList = new Dictionary<int,int>();

            var products = prodList.Where(p => shopList.ContainsKey(p.Id)).ToList();
            return View(products);
        }


        [HttpGet]
        [Route("PaymentWithPaypal")]
        public ActionResult PaymentWithPayPal(string cancel = null, string blogId = "", string payerId = "", string guid = "")
        {
            var clientId = configuration.GetValue<string>("PayPal:Key");
            var clientSecret = configuration.GetValue<string>("PayPal:Secret");
            var mode = configuration.GetValue<string>("PayPal:mode");
            var apiContext = PayPalConfiguration.GetAPIContext(clientId, clientSecret, mode);

            try
            {
                if (string.IsNullOrWhiteSpace(payerId))
                {
                    var baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Card/PaymentWithPayPal?";
                    var guidd = Convert.ToString((new Random()).Next(100000));
                    guid = guidd;

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, blogId);
                    var links = createdPayment.links.GetEnumerator();
                    string? paypalRedirectUrl = null;
                    while(links.MoveNext())
                    {
                        var lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var paymentId = httpContextAccessor.HttpContext.Session.GetString("payment");
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("PaymentFailed");
                    }

                    var blogs = executedPayment.transactions[0].item_list.items[0].sku;

                    // empty the bag
                    var shopList = new Dictionary<int,int>();
                    HttpContext.Session.Set(SessionHelper.ShoppingCart, shopList);

                    return View("PaymentSuccess");
                }
            }
            catch(Exception e)
            {
                return View("PaymentFailed");
            }
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            { payer_id = payerId,
            };

            this.payment = new Payment()
            {
                id = paymentId,
            };

            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            var prodList = context.Products.Select(p => new ProductVM().ProdToProdVM(p)).ToList();
            var shopList = HttpContext.Session.Get<Dictionary<int,int>>(SessionHelper.ShoppingCart);
            var prodsInBag = prodList.Where(p => shopList.ContainsKey(p.Id)).ToList();

            decimal totalAmount = 0;

            foreach(var prod in prodsInBag)
            {
                itemList.items.Add(new Item()
                {
                    name = prod.Name,
                    currency = "EUR",
                    price = string.Format("{0}", prod.Price),
                    quantity = string.Format("{0}", shopList[prod.Id]),
                    sku = "asd"
                });

                totalAmount += prod.Price * shopList[prod.Id];
            }

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirectUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var amount = new Amount()
            {
                currency = "EUR",
                total = string.Format("{0}", totalAmount)
            };

            var tramsactionList = new List<Transaction>();

            tramsactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = tramsactionList,
                redirect_urls = redirectUrls
            };

            return this.payment.Create(apiContext);
        }
    }
}
