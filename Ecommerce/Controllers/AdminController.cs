using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    public class AdminController : Controller
    {
        private dbContext _context;
        private IWebHostEnvironment _env;   
        public AdminController(dbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {     //if user is loged in then he can accesss information 
           string admin_session= HttpContext.Session.GetString("admin_session");
            if (admin_session != null)
            {
                return View();
            }
            //if user is not loged in then he will be redirected to login page
            else
            {
                return RedirectToAction("Login");
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string adminEmail,string adminPassword)
        {
           var row= _context.tbl_admin.FirstOrDefault(a => a.admin_email == adminEmail);//if the input email matches the email of admin in database
           if(row != null && row.admin_password==adminPassword)
            {
                HttpContext.Session.SetString("admin_session", row.admin_id.ToString());//set session and store the value of that session the form of admin id which is in row
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.message = "Invalid Email or Password";
                return View();
            }   
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin_session");//remove the session which was set during login
           return RedirectToAction("login");

        }
        public IActionResult Profile()
        {
           var adminId = HttpContext.Session.GetString("admin_session");//get the session value
            var row = _context.tbl_admin.Where(a=>a.admin_id==int.Parse(adminId)).ToList();//find the admin id in database
            return View(row);
        }
        [HttpPost]
        public IActionResult Profile(Admin admin)
        {
            _context.tbl_admin.Update(admin);
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
        [HttpPost]
        public IActionResult ChangeProfileImage(IFormFile admin_image,Admin admin)
        { // for uploading img create reference variable of iwebhostingenvironment
            string ImagePath=Path.Combine(_env.WebRootPath, "admin_image", admin_image.FileName);//combine the path of image folder and image name
            FileStream fs = new FileStream(ImagePath, FileMode.Create);//create filestream object
            admin_image.CopyTo(fs);//copy the image to filestream
            admin.admin_image = admin_image.FileName;//store the image name in admin_image
            _context.tbl_admin.Update(admin);
            _context.SaveChanges();//save the changes
            return RedirectToAction("Profile");
        }
        public IActionResult fetchCustomer()
        {
            return View(_context.tbl_customer.ToList());
        }
        public IActionResult customerDetails(int id)
        {
            return View(_context.tbl_customer.FirstOrDefault(a => a.customer_id == id));
        }
        public IActionResult updateCustomer(int id)
        {
                       return View(_context.tbl_customer.Find(id));
        }
        [HttpPost]
        public IActionResult updateCustomer(Customer customer)
        {
            _context.tbl_customer.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("fetchCustomer");
        }
        [HttpPost]
        public IActionResult CustomerProfileImage(Customer customer, IFormFile customer_image)
        {
            // for uploading img create reference variable of iwebhostingenvironment
            string ImagePath = Path.Combine(_env.WebRootPath, "customer_image",customer_image.FileName);//combine the path of image folder and image name
            FileStream fs = new FileStream(ImagePath, FileMode.Create);//create filestream object
            customer_image.CopyTo(fs);//copy the image to filestream
            customer.customer_image = customer_image.FileName;//store the image name in admin_image
            _context.tbl_customer.Update(customer);
            _context.SaveChanges();//save the changes
            return RedirectToAction("fetchCustomer");
        }
        public IActionResult deletePermission(int id)
        {
            return View(_context.tbl_customer.FirstOrDefault(a => a.customer_id == id));
            
        }
        public IActionResult deleteCustomer(int id)
        {
            _context.tbl_customer.Remove(_context.tbl_customer.Find(id));
            _context.SaveChanges();
            return RedirectToAction("fetchCustomer");
        }
        public IActionResult fetchCategory()
        {
            return View(_context.tbl_category.ToList());
        }
        public IActionResult addCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult addCategory(Category cat)
        {
            _context.tbl_category.Add(cat);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }
        public IActionResult updateCategory(int id)
        {//fetch the category from database
           var category= _context.tbl_category.Find(id);
            return View(category);
        }
        [HttpPost]
        public IActionResult updateCategory(Category cat)
        { // update category
            _context.tbl_category.Update(cat);
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }
        public IActionResult deletePermissionCategory(int id)
        {
            return View(_context.tbl_category.FirstOrDefault(a => a.category_id == id));
        }
        public IActionResult deleteCategory(int id)
        {
            _context.tbl_category.Remove(_context.tbl_category.Find(id));
            _context.SaveChanges();
            return RedirectToAction("fetchCategory");
        }
        public IActionResult fetchProduct()
        {
            return View(_context.tbl_product.ToList());
        }
        public IActionResult addProduct()
        {List<Category> categories  = _context.tbl_category.ToList();
            ViewData["category"] = categories;
            return View();
        }
        [HttpPost]
        public IActionResult addProduct(Product product,IFormFile product_image)
        {
            string imageName = Path.GetFileName( product_image.FileName);
            string imagePath = Path.Combine(_env.WebRootPath, "product_image", imageName);
            FileStream fs = new FileStream(imagePath, FileMode.Create);
            product_image.CopyTo(fs);
            product.product_image = imageName;
            _context.tbl_product.Add(product);
            _context.SaveChanges();
            return RedirectToAction("fetchProduct");
        }
        public IActionResult productDetails(int id)
        {
            return View(_context.tbl_product.Include(p=>p.Category).FirstOrDefault
                (p=>p.product_id==id));
        }
        public IActionResult deletePermissionProduct(int id)
        {
            return View(_context.tbl_product.FirstOrDefault(p => p.product_id == id));
        }
        public IActionResult deleteProduct(int id)
        {
            _context.tbl_product.Remove(_context.tbl_product.Find(id));
            _context.SaveChanges();
            return RedirectToAction("fetchProduct");
        }
        public IActionResult updateProduct(int id)
        {// fetch data from category table to show in dropdown
            List<Category> categories = _context.tbl_category.ToList();
            ViewData["category"] = categories;
           
            //fetch the category from database
            var product = _context.tbl_product.Find(id);
            ViewBag.selectedCategoryId = product.cart_id;
            return View(product);
        }
        [HttpPost]
        public IActionResult updateProduct(Product product)
        { // update category
            _context.tbl_product.Update(product);
            _context.SaveChanges();
            return RedirectToAction("fetchProduct");
        }
        [HttpPost]
        public IActionResult ProductProfileImage(IFormFile product_image, Product product)
        { // for uploading img create reference variable of iwebhostingenvironment
            string ImagePath = Path.Combine(_env.WebRootPath, "product_image", product_image.FileName);//combine the path of image folder and image name
            FileStream fs = new FileStream(ImagePath, FileMode.Create);//create filestream object
            product_image.CopyTo(fs);//copy the image to filestream
            product.product_image = product_image.FileName;//store the image name in product_image
            _context.tbl_product.Update(product);
            _context.SaveChanges();//save the changes
            return RedirectToAction("fetchProduct");
        }
        public IActionResult fetchFeedback()
        {
            return View(_context.tbl_feedback.ToList());
        }
        public IActionResult deletePermissionFeedback(int id)
        {
            return View(_context.tbl_feedback.FirstOrDefault(a => a.feedback_id == id));

        }
        public IActionResult deleteFeedback(int id)
        {
            _context.tbl_feedback.Remove(_context.tbl_feedback.Find(id));
            _context.SaveChanges();
            return RedirectToAction("fetchFeedback");
        }
        public IActionResult fetchCart()
        {
           var cart= _context.tbl_cart.Include(a => a.products).Include(a => a.customers).ToList();
            return View(cart);

        }
        public IActionResult deletePermissionCart(int id)
        {
            return View(_context.tbl_cart.FirstOrDefault(a => a.cart_id == id));
        }
        public IActionResult deleteCart(int id)
        {
            _context.tbl_cart.Remove(_context.tbl_cart.Find(id));
            _context.SaveChanges();
            return RedirectToAction("fetchCart");
        }
        public IActionResult updateCart(int id)
        {
            var cart = _context.tbl_cart.Find(id);
            return View(cart);
        }
        [HttpPost]
        public IActionResult updateCart(int cart_status,Cart cart)
        {    cart.cart_status = cart_status;
            _context.tbl_cart.Update(cart);
            _context.SaveChanges();
            return RedirectToAction("fetchCart");
        }
    }
}
