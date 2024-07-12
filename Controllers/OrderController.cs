using Ecommerce.Data.Concrete;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce;

public class OrderController:Controller
{
    public readonly EcommerceContext _context;
    public OrderController(EcommerceContext context)
    {
        _context = context;
    }
    public IActionResult Cart()
    {
        var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            var viewModel = new CartViewModel{
                Cart = cart
            };
            return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Cart(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if (product != null){
                var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                cart.AddItem(product, 1);
                HttpContext.Session.SetJson("cart", cart);
            }

            return RedirectToAction("Cart");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);
        if(product != null){
            var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            cart.DecreaseItem(product);
            HttpContext.Session.SetJson("cart", cart);
        }

        return RedirectToAction("Cart");
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Orders()
    {
        var orders = await _context.Orders.ToListAsync();
        return View(orders);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(int id){
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
        var orderItems = await _context.OrderItems.Where(x => x.OrderId == id).ToListAsync();
        var products = await _context.Products.ToListAsync();

        var viewModel = new ProductOrderViewModel{
            Order = order,
            Products = products,
            OrderItems = orderItems
        };
        
        if(order != null){
            return View(viewModel);
        } else {
            return NotFound();
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id){
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
        if(order != null){
            return View(order);
        } else {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id, string nameassw){
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id);
        if(order != null){
            _context.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Orders","Order");
        } else {
            return NotFound();
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        var viewModel = new CartViewModel{
            Cart = cart
        };

        ViewBag.Products = viewModel.Cart.Items;
        ViewBag.Total = viewModel.Cart.CalculateTotal().ToString("C2");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

        if (user == null){
            return NotFound();
        } else {
            ViewBag.User = user;
        }
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Checkout(int id, CheckoutViewModel model)
    {
        var cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

        var viewModel = new CartViewModel{
            Cart = cart
        };

        ViewBag.Products = viewModel.Cart.Items;
        ViewBag.Total = viewModel.Cart.CalculateTotal().ToString("C2");

        if(user.Country !=null || user.Address != null || user.City != null){
            ViewBag.User = user;
            if(cart.Items.Count == 0){
                ModelState.AddModelError("", "Sepetinizde ürün yok");
            }
        
            if(ModelState.IsValid){
                var order = new Order {
                    ShippingAddress = user.Address,
                    Country = user.Country,
                    City = user.City,
                    OrderDate = DateTime.Now,
                    ShippingDate = DateTime.Now,
                    TotalAmount = (double)cart.CalculateTotal(),
                    UserId = user.Id,
                    User = user,
                    OrderItems = cart.Items.Select(x => new OrderItem{
                        ProductId = x.Product.ProductId,
                        Price = x.Product.Price,
                        Quantity = x.Quantity,
                    }).ToList()
                };
                
                model.Cart = cart;

                var payment = ProcessPayment(model, order);
                if (payment == null || payment.Status != "success"){
                    ModelState.AddModelError("", "Ödeme işlemi başarısız oldu");
                    ModelState.AddModelError("", payment.ErrorMessage);
                    return View(model);
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Order veritabanına kaydedildikten sonra OrderId atanır
                var orderId = order.OrderId;

                cart.Clear();
                HttpContext.Session.SetJson("cart", cart);
                return RedirectToAction("Completed", "Order", new { OrderId = orderId });
            } else {
                return View();
            }
        } else {
            return RedirectToAction("Edit","Account");
        }        
    }

    private Payment ProcessPayment(CheckoutViewModel model, Order order)
    {
        Options options = new Options();
        options.ApiKey = "sandbox-glyYLHAMOocVLFIb7Nh75Hf7UhHT2IHS";
        options.SecretKey = "sandbox-rTF6olehlzEFCdB6fEd9Fc2MA0GVsT3x";
        options.BaseUrl = "https://sandbox-api.iyzipay.com";
                
        CreatePaymentRequest request = new CreatePaymentRequest();
        request.Locale = Locale.TR.ToString();
        request.ConversationId = new Random().Next(111111111,999999999).ToString();
        request.Price = model.Cart.CalculateTotal().ToString();
        request.PaidPrice = model.Cart.CalculateTotal().ToString();
        request.Currency = Currency.TRY.ToString();
        request.Installment = 1;
        request.BasketId = "B67832";
        request.PaymentChannel = PaymentChannel.WEB.ToString();
        request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

        PaymentCard paymentCard = new PaymentCard();
        paymentCard.CardHolderName = model.CardHolderName;
        paymentCard.CardNumber = model.CardNumber.ToString();
        paymentCard.ExpireMonth = model.ExpirationMonth.ToString();
        paymentCard.ExpireYear = model.ExpirationYear.ToString();
        paymentCard.Cvc = model.Cvc.ToString();
        paymentCard.RegisterCard = 0;
        request.PaymentCard = paymentCard;

        Buyer buyer = new Buyer();
        buyer.Id = order.UserId;
        buyer.Name = order.User.Name;
        buyer.Surname = order.User.Surname;
        buyer.GsmNumber = order.User.PhoneNumber;
        buyer.Email = order.User.Email;
        buyer.IdentityNumber = order.UserId;
        buyer.LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        buyer.RegistrationDate = "";
        buyer.RegistrationAddress = order.User.Address;
        buyer.Ip = "";
        buyer.City = order.User.City;
        buyer.Country = order.User.Country;
        buyer.ZipCode = order.User.PostalCode.ToString();
        request.Buyer = buyer;

        Address shippingAddress = new Address();
        shippingAddress.ContactName = $"{order.User.Name} {order.User.Surname}";
        shippingAddress.City = order.City;
        shippingAddress.Country = order.Country;
        shippingAddress.Description = $"{order.ShippingAddress} {order.City}/{order.Country}";
        shippingAddress.ZipCode = order.User.PostalCode.ToString();
        request.ShippingAddress = shippingAddress;

        Address billingAddress = new Address();
        billingAddress.ContactName = $"{order.User.Name} {order.User.Surname}";
        billingAddress.City = order.City.ToString();
        billingAddress.Country = order.Country;
        billingAddress.Description = $"{order.ShippingAddress} {order.City}/{order.Country}";
        billingAddress.ZipCode = order.User.PostalCode.ToString();
        request.BillingAddress = billingAddress;

        List<BasketItem> basketItems = new List<BasketItem>();
        foreach(var item in model.Cart.Items){
            BasketItem firstBasketItem = new BasketItem();
            firstBasketItem.Id = item.CartItemId.ToString();
            firstBasketItem.Name = item.Product.ProductName;
            firstBasketItem.Category1 = item.Product.CategoryId.ToString();
            firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            firstBasketItem.Price = (item.Product.Price * item.Quantity).ToString();
            basketItems.Add(firstBasketItem);
        }

        request.BasketItems = basketItems;

        Payment payment = Payment.Create(request, options);
        if (payment.Status != "success")
        {
            Console.WriteLine($"Error Code: {payment.ErrorCode}");
            Console.WriteLine($"Error Message: {payment.ErrorMessage}");
        }
        return payment;
    }

    public async Task<IActionResult> Completed(int OrderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == OrderId);
        return View(order);
    }
    
}
