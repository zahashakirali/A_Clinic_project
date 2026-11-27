using clinic_project.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace clinic_project.Controllers
{
    public class HomeController2 : Controller
    {
        ClinicDbContext db = new ClinicDbContext();

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User us)
        {
            // 1. Check user credentials in DB
            var result = db.Users.Include(u => u.Role)
                .FirstOrDefault(u => u.Email == us.Email && u.PasswordHash == us.PasswordHash);

            if (result != null)
            {
                // ✅ 2. Create ClaimsIdentity here
                var identity = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, result.Name ?? ""),
            new Claim(ClaimTypes.Sid, result.UserId.ToString()),
            new Claim(ClaimTypes.Role, result.Role?.RoleName ?? "")
        }, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Create the principal
                var principal = new ClaimsPrincipal(identity);

                // 4. Sign in
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // 5. Redirect based on role
                if (result.Role?.RoleName?.ToLower() == "admin" || result.Role?.RoleName == "Staff")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            // 6. Invalid login
            ViewBag.msg = "Invalid email or password.";
            return View();
        }

        public IActionResult Logout()
        {
            // User ko sign out karte hain
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Logout ke baad login page pe redirect karte hain
            return RedirectToAction("Login"); // "Account" ko aapke controller ke naam se replace karein
        }

        public IActionResult Addtocart()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login");
            int userId = int.Parse(userIdClaim);

            // Database se user ka cart aur uske andar ke CartItems fetch karein
            var cart = db.Carts
                         .Include(c => c.CartItems)
                         .FirstOrDefault(c => c.UserId == userId);

            // Agar cart bilkul khali hai ya koi item nahi hai
            if (cart == null)
            {
                cart = new Cart { CartItems = new List<CartItem>() };
            }

            return View(cart);
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, string productType)
        {
           
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login"); // Agar login nahi hai toh login page par bhejein
            }

            
            var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login");
            }

            int userId = int.Parse(userIdClaim); 

        
            var product = db.Medicines.FirstOrDefault(m => m.MedicineId == productId);
            if (product == null) return NotFound();

            decimal finalPrice = product.Price.Value;
            var cart = db.Carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId, CreatedAt = DateTime.Now };
                db.Carts.Add(cart);
                db.SaveChanges();
            }

            var existingItem = db.CartItems.FirstOrDefault(ci =>
                ci.CartId == cart.CartId && ci.ProductId == productId && ci.ProductType == productType);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                db.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    ProductType = productType,
                    Price = finalPrice,
                    Quantity = 1
                });
            }

            db.SaveChanges();
            return RedirectToAction("addtocart");
        }







        public IActionResult Medicine()
        {
            return View(db.Medicines.ToList());
        }

        public IActionResult ShowEquipment()
        {
            return View(db.Equipment.ToList());
        }
        public IActionResult Checkout()
        {
            // Login check lagaya taaki unauthenticated user access na kare
            var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login");
            }

            int userId = int.Parse(userIdClaim);
            var cart = GetCartFromDbForUser(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Addtocart"); // Agar cart empty hai to Cart page par bhejein
            }

            return View(cart);
        }

        // 2. POST: Process Order Submission (Updated for Logged-in User)
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string shippingAddress, string paymentMethod)
        {
            // Login check
            var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login");
            }

            int userId = int.Parse(userIdClaim);

            // Active user ka cart fetch karein
            var cart = GetCartFromDbForUser(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Addtocart");
            }

            // Calculate total amount
            decimal grandTotal = cart.CartItems.Sum(item => (item.Price ?? 0) * (item.Quantity ?? 1));

            // Start an explicit transaction block to ensure data integrity
            using (var dbTransaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Create and map the new Order with correct UserId
                    var order = new Order
                    {
                        UserId = userId, // Actual Logged-in User ID
                        OrderDate = DateTime.Now,
                        TotalAmount = grandTotal,
                        Status = "Pending",
                        ShippingAddress = shippingAddress,
                        PaymentMethod = paymentMethod
                    };

                    db.Orders.Add(order);
                    await db.SaveChangesAsync(); // Generates the OrderId

                    // 2. Map and transfer CartItems to OrderDetails
                    foreach (var item in cart.CartItems)
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductType = item.ProductType,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = item.Price
                        };
                        db.OrderDetails.Add(orderDetail);
                    }

                    // 3. Log a baseline transactional entry for financial accounting
                    var transaction = new Transaction
                    {
                        OrderId = order.OrderId,
                        Type = paymentMethod == "Cash on Delivery" ? "Pending COD" : "Paid",
                        Amount = grandTotal,
                        CreatedAt = DateTime.Now
                    };
                    db.Transactions.Add(transaction);

                    // 4. Clear the active cart records
                    db.CartItems.RemoveRange(cart.CartItems);

                    await db.SaveChangesAsync();
                    await dbTransaction.CommitAsync(); // Commit changes safely

                    // Pass Order ID to the receipt confirmation page
                    return RedirectToAction("OrderSuccess", new { id = order.OrderId });
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    TempData["Error"] = "Something went wrong while placing your order. Please try again.";
                    return RedirectToAction("Checkout");
                }
            }
        }
        private Cart GetCartFromDbForUser(int userId)
        {
            return db.Carts
                     .Include(c => c.CartItems)
                     .FirstOrDefault(c => c.UserId == userId);
        }
        // 3. GET: Confirmation Landing Page
        public IActionResult OrderSuccess(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
        // Helper stub to match your project execution approach
        private Cart GetCartFromSessionOrDb()
        {
            // Update this block to fetch cart objects matching your existing pattern
            return db.Carts.Include(c => c.CartItems).FirstOrDefault();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult detailed_service()
        {
            return View();
        }
        public IActionResult MyOrders()
{
    // 1. Check karein ke user login hai ya nahi
    var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;
    if (string.IsNullOrEmpty(userIdClaim))
    {
        return RedirectToAction("Login");
    }

    int userId = int.Parse(userIdClaim);

    var userOrders = db.Orders
                       .Where(o => o.UserId == userId)
                       .OrderByDescending(o => o.OrderDate)
                       .ToList();

    return View(userOrders);
}
    }

    }



