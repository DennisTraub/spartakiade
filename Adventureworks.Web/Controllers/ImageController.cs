using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Adventureworks.Domain;
using Adventureworks.Infrastructure.Utilities;
using Adventureworks.Web.Helpers;

namespace Adventureworks.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly AdventureWorks2008R2Entities _db = new AdventureWorks2008R2Entities();

        //
        // GET: /Image/ProductThumbnail?ProductPhotoID

        public ActionResult ProductThumbnail(int productPhotoID)
        {
            MemoryStream image = GetProductThumbnail(productPhotoID);

            byte[] buffer = image.ToArray();
            Bitmap bmp = (Bitmap)Bitmap.FromStream(image);
            buffer = GifConverter.ConvertGif(bmp);

            return new ImageResult { Image = buffer, ImageFormat = ImageFormat.Gif };
        }

        //
        // GET: /Image/ProductPhoto?ProductPhotoID

        public ActionResult ProductPhoto(int productPhotoID)
        {
            MemoryStream image = GetProductPhoto(productPhotoID);

            byte[] buffer = image.ToArray();
            Bitmap bmp = (Bitmap)Bitmap.FromStream(image);
            buffer = GifConverter.ConvertGif(bmp);

            return new ImageResult { Image = buffer, ImageFormat = ImageFormat.Gif };
        }

        public MemoryStream GetProductThumbnail(int productPhotoID)
        {
            byte[] thumbNailPhoto = _db.ProductPhotoes.Where<ProductPhoto>(pp => pp.ProductPhotoID == productPhotoID).FirstOrDefault<ProductPhoto>().ThumbNailPhoto;
            MemoryStream ms = new MemoryStream(thumbNailPhoto);
            Image image = Image.FromStream(ms);
            return ms;
        }

        public MemoryStream GetProductPhoto(int productPhotoID)
        {
            byte[] largePhoto = _db.ProductPhotoes.Where<ProductPhoto>(pp => pp.ProductPhotoID == productPhotoID).FirstOrDefault<ProductPhoto>().LargePhoto;
            MemoryStream ms = new MemoryStream(largePhoto);
            Image image = Image.FromStream(ms);
            return ms;
        }
    }
}
