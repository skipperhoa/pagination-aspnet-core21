using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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