using static System.Net.Mime.MediaTypeNames;

public class ImageConvertHelper
{
// public static Image ConvertBase64StringToImage(string imageBase64String)
// {
//     var imageBytes = Convert.FromBase64String(imageBase64String);
//     var imageStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
//     imageStream.Write(imageBytes, 0, imageBytes.Length);
//     var image = Image.FromStream(imageStream, true);
//     return image;
// }
// public static string ConvertImageToBase64String(Image image)
//     {
//         var imageStream = new MemoryStream();
//         image.Save(imageStream, ImageFormat.Png);
//         imageStream.Position = 0;
//         var imageBytes = imageStream.ToArray();
//         return Convert.ToBase64String(imageBytes);
//     }
//       public static int CompareImages(Image i1, Image i2)
//     {
//         string img1 = ConvertImageToBase64String(i1);
//         string img2 = ConvertImageToBase64String(i2);
//         return String.Compare(img1, img2);
//     }
}

 