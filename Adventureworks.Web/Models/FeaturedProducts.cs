using System.Collections.Generic;
using System.Linq;
using Adventureworks.Domain;

namespace Adventureworks.Web.Models
{
    public class FeaturedProducts
    {
        private readonly AdventureWorks2008R2Entities _db = new AdventureWorks2008R2Entities();

        public IQueryable<Product> Load()
        {
            IList<Product> products = new List<Product>();
            Product product;
            ProductPhoto prodPhoto;
            ProductProductPhoto prodProdPhoto;

            product = new Product();
            product.Name = "Road-150 Red, 48";
            prodPhoto = new ProductPhoto();
            prodPhoto.PhotoPath = "~/Content/Images/SampleCycles/cycle1.gif";
            prodProdPhoto = new ProductProductPhoto() { ProductPhoto = prodPhoto };
            product.ProductProductPhotoes.Add(prodProdPhoto);
            products.Add(product);

            product = new Product();
            product.Name = "Touring-2000 Blue, 60";
            prodPhoto = new ProductPhoto();
            prodPhoto.PhotoPath = "~/Content/Images/SampleCycles/cycle2.gif";
            prodProdPhoto = new ProductProductPhoto() { ProductPhoto = prodPhoto };
            product.ProductProductPhotoes.Add(prodProdPhoto);
            products.Add(product);

            product = new Product();
            product.Name = "Mountain-100 Black, 42";
            prodPhoto = new ProductPhoto();
            prodPhoto.PhotoPath = "~/Content/Images/SampleCycles/cycle3.gif";
            prodProdPhoto = new ProductProductPhoto() { ProductPhoto = prodPhoto };
            product.ProductProductPhotoes.Add(prodProdPhoto);
            products.Add(product);

            product = new Product();
            product.Name = "Road-350-W Yellow, 44";
            prodPhoto = new ProductPhoto();
            prodPhoto.PhotoPath = "~/Content/Images/SampleCycles/cycle4.jpg";
            prodProdPhoto = new ProductProductPhoto() { ProductPhoto = prodPhoto };
            product.ProductProductPhotoes.Add(prodProdPhoto);
            products.Add(product);


            return products.AsQueryable<Product>();
        }
    }
}