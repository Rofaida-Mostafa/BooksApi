using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Threading.Tasks;
using System.Transactions;

namespace BooksApi.Areas.Customer.Controllers
{
    [Area(SD.CustomerArea)]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<Order> _orderRepository;

        public CartsController(UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Promotion> promotionRepository, IRepository<Order> orderRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _promotionRepository = promotionRepository;
            _orderRepository = orderRepository;
        }

        [HttpPost("")]
        public async Task<IActionResult> AddToCart(CartRequest cartRequest)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.BookId == cartRequest.BookId);

            if (cart is not null)
            {
                cart.Count += cartRequest.Count;
            }
            else
            {
                await _cartRepository.CreateAsync(new()
                {
                    ApplicationUserId = user.Id,
                    BookId = cartRequest.BookId,
                    Count = cartRequest.Count
                });
            }

            await _cartRepository.comitChanges();

            return Ok(new
            {
                msg = "Add Product To Cart Successfully"
            });
        }

        [HttpGet("")] 
        public async Task<IActionResult> Index(string? code = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var carts = await _cartRepository.GetAllAsync(e => e.ApplicationUserId == user.Id, includes: [e => e.Book]);

            var totalPrice = carts.Sum(e => e.Book.Price * e.Count);

            string msg = "";
            if (code is not null)
            {

                var promotion = await _promotionRepository.GetOne(e => e.Code == code);
                if (promotion is null || !promotion.Status || DateTime.UtcNow > promotion.ValidTo)
                {
                    msg = "Invalid Code OR Expired";
                }
                else
                {
                    promotion.TotalUsed += 1;
                    await _promotionRepository.comitChanges();

                    totalPrice = totalPrice - (totalPrice * 0.05m);
                    msg = "Apply Promotion";
                }
            }

            return Ok(new
            {
                carts,
                totalPrice,
                msg
            });
        }

        [HttpPatch("IncrementCart/{BookId}")]
        public async Task<IActionResult> IncrementCart(int BookId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.BookId == BookId);

            if (cart is null)
                return NotFound();

            cart.Count += 1;
            await _cartRepository.comitChanges();

            return NoContent();
        }

        [HttpPatch("DecrementCart/{BookId}")]
        public async Task<IActionResult> DecrementCart(int BookId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.BookId == BookId);

            if (cart is null)
                return NotFound();

            if (cart.Count > 1)
            {
                cart.Count -= 1;
                await _cartRepository.comitChanges();
            }

            return NoContent();
        }

        [HttpDelete("{BookId}")]
        public async Task<IActionResult> DeleteCart(int BookId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await _cartRepository.GetOne(e => e.ApplicationUserId == user.Id && e.BookId == BookId);

            if (cart is null)
                return NotFound();

            _cartRepository.Delete(cart);
            await _cartRepository.comitChanges();

            return NoContent();
        }

        [HttpGet("Pay")]
        public async Task<IActionResult> Pay()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var carts = await _cartRepository.GetAllAsync(e => e.ApplicationUserId == user.Id,
                includes: [e => e.Book]);


            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/api/Customer/Checkouts/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/api/Customer/checkouts/cancel",
            };

            foreach (var item in carts)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Book.AuthorName,
                            Description = item.Book.Description,
                        },
                        UnitAmount = (long)item.Book.Price * 100,
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new
            {
                url = session.Url
            });
        }
    }
}
