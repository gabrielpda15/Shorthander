using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shorthander
{
    public class ImgManager
    {
        public Bitmap ReadImage(string path)
        {
            return (Bitmap)Image.FromFile(path);
        }

        public void WriteImage(Bitmap image, string path)
        {
            var file = new string(path);
            if (file.EndsWith(".jpg")) file = file.Replace(".jpg", ".jpeg");
            var format = typeof(ImageFormat).GetProperties().SingleOrDefault(x => "." + x.Name.ToLower() == Path.GetExtension(file).ToLower());
            if (format == null) throw new FileFormatException(new Uri(file), "Extensão não suportada!");

            if (File.Exists(path))
                File.Delete(path);
            image.Save(path, (ImageFormat)format.GetValue(null, null));
        }
    }
}
