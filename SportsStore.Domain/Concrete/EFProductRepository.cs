using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EFProductRepository : IProductRepository
    {
        private EFDbContext contex = new EFDbContext();

        public IQueryable<Product> Products
        {
            get { return contex.Products; }
        }


        public void SaveProduct(Product product)
        {
            if (product.ProductID == 0)
            {
                contex.Products.Add(product);
            }
            else
            {
                Product dbEntry = contex.Products.Find(product.ProductID);
                if (dbEntry != null)
                {
                    dbEntry.Name = product.Name;
                    dbEntry.Description = product.Description;
                    dbEntry.Price = product.Price;
                    dbEntry.Category = product.Category;
                }
            }
            contex.SaveChanges();
        }


        public Product DeleteProduct(int productId)
        {
            Product dbEntity = contex.Products.Find(productId);
            if (dbEntity != null)
            {
                contex.Products.Remove(dbEntity);
                contex.SaveChanges();
            }
            return dbEntity;
        }
    }
}
