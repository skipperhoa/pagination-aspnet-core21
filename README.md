# Pagination using ASP.NET Core 2.1
## Create Project
Step 1: Open Visual Studio 2010, File->New->choose Project

Step 2: Create a Project

Step 3: select ASP.NET Core Web Application template

Step 4: Enter name Project

Step 5: Select .NET Core, ASP.NET Core 2.1, choose Web Application (Model-View-Controller) -> Create

## Install EntityframeworkCore : open Nuget Packager Manager
```csharp
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
```
# Models Configuarion 
**Models/Product.cs**
```csharp
public class Product
    {
        public int idProduct { get; set; }
        public string Title { get; set; }
        public string UrlImage { get; set; }
        public string Detail { get; set; }
        public int Price { get; set; }


    }
 ```
 add connectstring to **appsettings.json**
 ```csharp
 {
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "EFDataContext": "Server=DESKTOP-GCABV8F\\SQLExpress;Database=DB_Pagination;Trusted_Connection=True;MultipleActiveResultSets=true"
  }

}
```
Create EFDataContext.cs class in Models directory
```csharp
using Microsoft.EntityFrameworkCore;
namespace PaginationASPCore.Models
{
    public class EFDataContextcs : DbContext
    {
        public EFDataContextcs(DbContextOptions<EFDataContextcs> options): base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(s => s.idProduct);
        }
        public DbSet<Product> Products { get; set; }
    }
}
```
Add services in Startup.cs file
```csharp
services.AddDbContext<EFDataContextcs>(options=>options.UseSqlServer(Configuration.GetConnectionString("EFDataContext"),
                builder => builder.UseRowNumberForPaging()));
```
## Migartion
```csharp
add-migration dbpagination
update-database
```
## Create Services foldel in project
**+ Services/IProduct.cs**
```csharp
using PaginationASPCore.Models;
namespace PaginationASPCore.Services
{
    public interface IProduct
    {
        IEnumerable<Product> getProductAll();
        int totalProduct();
        int numberPage(int totalProduct, int limit);
        IEnumerable<Product> paginationProduct(int start, int limit);


    }
}
```
**+ Services/ItemProductService.cs**
```csharp
using PaginationASPCore.Models;
namespace PaginationASPCore.Services
{
    public class ItemProductService : IProduct
    {
        private readonly EFDataContextcs _db;
        private List<Product> products  = new List<Product>();
        public ItemProductService(EFDataContextcs db)
        {
            _db = db;
            this.products = _db.Products.ToList();
        }
        public IEnumerable<Product> getProductAll()
        {
            return products;
        }
        public int totalProduct()
        {
            return products.Count; 
        }
        public int numberPage(int totalProduct,int limit)
        {
            float numberpage = totalProduct / limit;
            return (int)Math.Ceiling(numberpage);
        }
        public IEnumerable<Product> paginationProduct(int start, int limit)
        {
            var data = (from s in _db.Products select s);
            var dataProduct = data.OrderByDescending(x => x.idProduct).Skip(start).Take(limit);
            return dataProduct.ToList();
        }
    }
}
```
register services in **Startup.cs** file
```
services.AddScoped<IProduct, ItemProductService>();
```
## Controller configuration
**+ Controllers/ProductController.cs**
```csharp
using Microsoft.AspNetCore.Mvc;
using PaginationASPCore.Services;
namespace PaginationASPCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProduct _product;
        public ProductController(IProduct product)
        {
            _product = product;
        }
        public IActionResult Index(int? page=0)
        {
            int limit = 2;
            int start;
            if (page > 0)
            {
                page = page;
            }
            else
            {
                page = 1;
            }
            start = (int)(page - 1) * limit;

            ViewBag.pageCurrent = page;

            int totalProduct = _product.totalProduct();

            ViewBag.totalProduct = totalProduct;

            ViewBag.numberPage = _product.numberPage(totalProduct, limit);

            var data = _product.paginationProduct(start, limit);

            return View(data);
        }
    }
}
```
## Views 
Create index.cshtml file in Views/Product directory
```csharp
@model IEnumerable<Product>
@{Layout = "~/Views/Shared/_Layout.cshtml";}
<div class="row">
    <div class="col-md-8">
        <h2>Pagiantion Product</h2>
        <table class="table">
            <thead>
                <tr>
                    <th>Title</th>
                    <th>Image</th>
                    <th>Detail</th>
                    <th>Price</th>
                </tr>
            </thead>
            <tbody>
               @foreach(var item in Model)
               {
                   <tr>
                       <td>@item.Title</td>
                       <td>@item.UrlImage</td>
                       <td>@item.Detail</td>
                       <td>@item.Price</td>
                   </tr>
               }
            </tbody>
        </table>
        <ul class="pagination">
            @{
                int numberPage = ViewBag.numberPage;
                int pageCurrent = ViewBag.pageCurrent;
                int offset = 2;//number display page


                //config FirstLink
                if (pageCurrent > 1)
                {
                    int prevPage = pageCurrent - 1;
                    var PrevLink = new Dictionary<string, string> { { "page", prevPage.ToString() } };
                    <li><a asp-controller="Product" asp-action="Index" asp-all-route-data="PrevLink">Prev Link</a></li>
                }

                int from = pageCurrent - offset; 
                int to = pageCurrent + offset; 
                if (from <= 0)
                {
                    from = 1;
                    to = offset * 2;
                }

               
                if (to > numberPage)
                {
                    to = numberPage;
                }

                int i;
                for (i = from; i <= to; i++)
                {
                    var parms = new Dictionary<string, string>
                    {
                        { "page",i.ToString()}
                    };
                    if (pageCurrent == i)
                    {
                        <li class="active"><a asp-controller="Product" asp-action="Index" asp-all-route-data="parms">@i</a></li>
                    }
                    else
                    {
                        <li><a asp-controller="Product" asp-action="Index" asp-all-route-data="parms">@i</a></li>
                    }

                }
              
                if (pageCurrent < numberPage - (to / 2))
                {
                    <li><a>...</a></li>
                    var LastLink = new Dictionary<string, string> { { "page", numberPage.ToString() } };
                    <li><a asp-controller="Product" asp-action="Index" asp-all-route-data="LastLink">Last Link</a></li>
                }

                if (pageCurrent < numberPage)
                {
                    int nextPage = pageCurrent + 1;
                    var NextLink = new Dictionary<string, string> { { "page", nextPage.ToString() } };
                    <li><a asp-controller="Product" asp-action="Index" asp-all-route-data="NextLink">Next Link</a></li>
                }
            }
        </ul>
    </div>
</div>
```
## Config Startup.cs
```
app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Product}/{action=Index}/{id?}");
            });
  ```
  

 
