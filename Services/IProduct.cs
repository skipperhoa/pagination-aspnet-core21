using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
