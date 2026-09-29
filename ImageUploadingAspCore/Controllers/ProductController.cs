using ImageUploadingAspCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageUploadingAspCore.Controllers
{
    public class ProductController : Controller
    {
        ImageDbContext context;
        IWebHostEnvironment env;

        public ProductController(ImageDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.env = webHostEnvironment;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View(context.Products.ToList<Product>());
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(ProductViewModel productViewModel)
        {
            string fileName = "";

            if (productViewModel.Photo != null)
            {
                var extension = Path.GetExtension(productViewModel.Photo.FileName);
                var size = productViewModel.Photo.Length;

                if (extension.Equals(".png") || extension.Equals(".jpg") || extension.Equals(".jpeg"))
                {
                    if (size <= 1000000)
                    {
                        string folder = Path.Combine(env.WebRootPath, "images");
                        fileName = Guid.NewGuid().ToString() + "_" + productViewModel.Photo.FileName;

                        string filePath = Path.Combine(folder, fileName);

                        productViewModel.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

                        Product product = new Product()
                        {
                            Name = productViewModel.Name,
                            Price = productViewModel.Price,
                            ImagePath = fileName,
                        };

                        context.Add(product);
                        context.SaveChanges();
                        TempData["Success"] = "Product Added..";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Size_Error"] = "Image must be less than 1 MB.";
                    }
                }
                else
                {
                    TempData["Extension_Error"] = "Only PNG JPG JPEG images allowed.";
                }

            }

            return View();
        }
    }
}
