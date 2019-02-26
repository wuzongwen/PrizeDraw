using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace Common
{
    public class QRCodeHelper
    {
        /// <summary>  
        /// 生成二维码  
        /// </summary>  
        /// <param name="content">内容</param>
        /// <param name="moduleSize">二维码的大小</param>
        /// <returns>输出流</returns>  
        public static MemoryStream GetQRCode(string content, int moduleSize = 9)
        {
            var encoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = encoder.Encode(content);
            GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(moduleSize, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            MemoryStream memoryStream = new MemoryStream();
            render.WriteToStream(qrCode.Matrix, ImageFormat.Jpeg, memoryStream);

            return memoryStream;

            //生成图片的代码
            //DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
            //Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
            //Graphics g = Graphics.FromImage(map);
            //render.Draw(g, qrCode.Matrix);
            //map.Save(fileName, ImageFormat.Jpeg);//fileName为存放的图片路径
        }

        /// <summary>
        /// 生成带Logo二维码  
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="iconPath">logo路径</param>
        /// <param name="moduleSize">二维码的大小</param>
        /// <returns>输出流</returns>
        public static MemoryStream GetQRCode(string content, string iconPath, int moduleSize = 9)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = qrEncoder.Encode(content);

            GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(moduleSize, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
            Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
            Graphics g = Graphics.FromImage(map);
            render.Draw(g, qrCode.Matrix);

            //追加Logo图片 ,注意控制Logo图片大小和二维码大小的比例
            //PS:追加的图片过大超过二维码的容错率会导致信息丢失,无法被识别
            Image img = Image.FromFile(iconPath);

            Point imgPoint = new Point((map.Width - img.Width) / 2, (map.Height - img.Height) / 2);
            g.DrawImage(img, imgPoint.X, imgPoint.Y, img.Width, img.Height);

            MemoryStream memoryStream = new MemoryStream();
            map.Save(memoryStream, ImageFormat.Jpeg);

            return memoryStream;

            //生成图片的代码： map.Save(fileName, ImageFormat.Jpeg);//fileName为存放的图片路径
        }
    }
}
