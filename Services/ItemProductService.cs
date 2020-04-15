using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
