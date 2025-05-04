using System.Collections.Generic;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    public class CustomerController : Controller
    {
        private dbContext _context;
        private IWebHostEnvironment _env;
        public CustomerController(dbContext context,IWebHostEnvironment env) { 
            _context = context;
            _env = env;
        }
        public IActionResult Index(int? categoryId)
        {
            var categories = _context.tbl_category.ToList();
            ViewData["category"] = categories;

            List<Product> products;

            if (categoryId.HasValue)
            {
                products = _context.tbl_product
                            .Where(p => p.cart_id == categoryId.Value)
                            .Include(p => p.Category)
                            .ToList();
            }
            else
            {
                products = _context.tbl_product
                            .Include(p => p.Category)
                            .ToList();
            }

            ViewData["product"] = products;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.checkSession = HttpContext.Session.GetString("customerSession");

            return View();
        }


        //public IActionResult Index()
        //{
        //    List<Category> category = _context.tbl_category.ToList();
        //    ViewData["category"] = category;
        //    List<Product> products = _context.tbl_product.ToList();
        //    ViewData["product"] = products;

        //    ViewBag.checkSession = HttpContext.Session.GetString("customerSession");
        //    return View();
        //}
        public IActionResult customerLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult customerLogin(String customerEmail,String customerPassword)
        {
          var customer=  _context.tbl_customer.FirstOrDefault(c=>c.customer_email==customerEmail);
            if (customer != null && customer.customer_password==customerPassword) 
            {
                
                HttpContext.Session.SetString("customerSession",customer.customer_id.ToString()); 
                return RedirectToAction("Index");
            }  
            else
            {
                ViewBag.message = "Incorrect Message or Password";
                return View();

            }
      
        }
        public IActionResult customerRegisration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult customerRegisration(Customer customer)
        {
            _context.tbl_customer.Add(customer);
            _context.SaveChanges();
            return RedirectToAction("customerLogin");
        }
        public IActionResult customerLogout()
        {
            HttpContext.Session.Remove("customerSession");
            return RedirectToAction("Index");
        }
        public IActionResult customerProfile()
        { if(string.IsNullOrEmpty(HttpContext.Session.GetString("customerSession")))
            {
                return RedirectToAction("customerLogin");
            }
            else 
            {
               
                List<Category> category = _context.tbl_category.ToList();
                ViewData["category"] = category;
                var customerId = HttpContext.Session.GetString("customerSession");//get the session value
                var row = _context.tbl_customer.Where(c => c.customer_id == int.Parse(customerId)).ToList();//find the admin id in database
                return View(row);
            }
           
        }
        [HttpPost]
        public IActionResult updateProfile(Customer customer) 
        {
            _context.tbl_customer.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("customerProfile");
        }
        public IActionResult changeProfileImage(Customer customer,IFormFile customer_image)
        {
            string ImagePath = Path.Combine(_env.WebRootPath, "customer_image", customer_image.FileName);
            FileStream fs =new FileStream(ImagePath, FileMode.Create);
            customer_image.CopyTo(fs);
            customer.customer_image = customer_image.FileName;
            _context.tbl_customer.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("customerProfile");
        }
        public IActionResult feedback()
        {
         
            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;
            return View();
        }
        [HttpPost]
        public IActionResult feedback(Feedback feedback)
        {
            TempData["message"] = "Thank You For Your Feedback";
            _context.tbl_feedback.Add(feedback);
            _context.SaveChanges();
            return RedirectToAction("feedback");
        }
        public IActionResult fetcchAllProducts()
        {
            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;
            List<Product> products = _context.tbl_product.ToList();
            ViewData["product"] = products;
            return View();
        }
       public IActionResult productDetails(int id) 
        {
            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;
            var products=_context.tbl_product.Where(p => p.product_id == id).ToList();
            return View(products); 
        }
        //public IActionResult AddToCart(int prod_id, Cart cart)
        //{
        //    string isLogin = HttpContext.Session.GetString("customerSession");
        //    if (isLogin != null)
        //    {
        //        // Check if product exists
        //        var product = _context.tbl_product.FirstOrDefault(p => p.product_id == prod_id);
        //        if (product == null)
        //        {
        //            TempData["error"] = "Product not found.";
        //            return RedirectToAction("fetcchAllProducts");
        //        }

        //        cart.prod_id = prod_id;
        //        cart.cust_id = int.Parse(isLogin);
        //        cart.product_quantity = 1;
        //        cart.cart_status = 1;

        //        _context.tbl_cart.Add(cart);
        //        _context.SaveChanges();

        //        TempData["message"] = "Product Added To Cart";
        //        return RedirectToAction("fetcchAllProducts");
        //    }
        //    else
        //    {
        //        return RedirectToAction("customerLogin");
        //    }
        //}

        public IActionResult AddToCart(int prod_id, Cart cart)
        {
            string isLogin = HttpContext.Session.GetString("customerSession");
            if (isLogin != null)
            {
                int custId = int.Parse(isLogin);

                // Check if this product is already in the cart for this customer and is still active (status = 1)
                var existingCartItem = _context.tbl_cart
                    .FirstOrDefault(c => c.prod_id == prod_id && c.cust_id == custId && c.cart_status == 1);
                var productExists = _context.tbl_product.Any(p => p.product_id == prod_id);
                if (!productExists)
                {
                    TempData["message"] = "Invalid product.";
                    return RedirectToAction("fetcchAllProducts");
                }

                if (existingCartItem != null)
                {
                    TempData["message"] = "Product is already in your cart.";
                }
                else
                {
                    cart.prod_id = prod_id;
                    cart.cust_id = custId;
                    cart.product_quantity = 1;
                    cart.cart_status = 1;
                    _context.tbl_cart.Add(cart);
                    _context.SaveChanges();
                    TempData["message"] = "Product Added To Cart";
                }

                return RedirectToAction("fetcchAllProducts");
            }
            else
            {
                return RedirectToAction("customerLogin");
            }
        }

        public IActionResult fetchCart()
        {
            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;
            string customerId = HttpContext.Session.GetString("customerSession");
            if (customerId!=null)
            {
                var cart = _context.tbl_cart.Where(c => c.cust_id == int.Parse(customerId)).Include(c => c.products).ToList();
                return View(cart);
            }
            else
            {
                return RedirectToAction("customerLogin");
            }
           
        }
        public IActionResult removeProduct(int id)
        {
            var cart = _context.tbl_cart.Find(id);
            _context.tbl_cart.Remove(cart);
            _context.SaveChanges();
            return RedirectToAction("fetchCart");
        }
        public IActionResult checkoutProduct()
        {
            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;
            string isLogin = HttpContext.Session.GetString("customerSession");
            if (isLogin != null)
            {
                int custId = int.Parse(isLogin);

                var cartItems = _context.tbl_cart
                    .Where(c => c.cust_id == custId && c.cart_status == 1)
                    .Include(c => c.products)
                    .ToList();

                return View(cartItems);
            }
            else
            {
                return RedirectToAction("customerLogin");
            }
           
        }
        [HttpPost]
        public IActionResult PlaceOrder(List<Cart> carts)
        {
            string isLogin = HttpContext.Session.GetString("customerSession");
            if (isLogin != null)
            {
                int custId = int.Parse(isLogin);

                var cartItems = _context.tbl_cart
                    .Where(c => c.cust_id == custId && c.cart_status == 1)
                    .ToList();

                foreach (var item in cartItems)
                {
                    var updatedItem = carts.FirstOrDefault(c => c.cart_id == item.cart_id);
                    if (updatedItem != null)
                    {
                        item.product_quantity = updatedItem.product_quantity;
                        item.cart_status = 0; //  Mark as ordered
                    }
                }

                _context.SaveChanges();
                TempData["message"] = "Order placed successfully.";
                return RedirectToAction("OrderConfirmation");
            }

            return RedirectToAction("customerLogin");
        }

        public IActionResult OrderConfirmation()
        {
            List<Category> category = _context.tbl_category.ToList();
            ViewData["category"] = category;
            return View();
        }





    }
}
