using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Gardener.Crawler.Client.UWP.Util
{
    class FileHelper
    {
        public static string FilterFolderName(string folderName)
        {
            foreach (char rInvalidChar in System.IO.Path.GetInvalidPathChars())
            {
                folderName = folderName.Replace(rInvalidChar, char.MinValue);
            }
            string errChar = "\\/:*?";
            foreach (char rInvalidChar in errChar)
            {
                folderName = folderName.Replace(rInvalidChar, char.MinValue);
            }

            return folderName;
        }

        public static async Task SaveImageAsync(Image image, string appName, string folderName, string fileName, Action action = null)
        {
            try
            {
                if (image == null)
                {
                    return;
                }
                Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                if (fileName.EndsWith("jpg", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                else if (fileName.EndsWith("png", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
                else if (fileName.EndsWith("bmp", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
                else if (fileName.EndsWith("tiff", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
                else if (fileName.EndsWith("gif", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.GifEncoderId;

                var storageFolder = KnownFolders.PicturesLibrary;
                var appfolder = await storageFolder.CreateFolderAsync(appName, CreationCollisionOption.OpenIfExists);
                var folder = await appfolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);

                    var himage = (BitmapImage)image.Source;
                    RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(himage.UriSour‌​ce);

                    using (IRandomAccessStream randomAccessStream = await random.OpenReadAsync())
                    {
                        var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

                        var myTransform = new BitmapTransform { ScaledHeight = (uint)himage.PixelHeight, ScaledWidth = (uint)himage.PixelWidth };

                        var pixels = await decoder.GetPixelDataAsync(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode,
                        myTransform,
                        ExifOrientationMode.IgnoreExifOrientation,
                        ColorManagementMode.DoNotColorManage);

                        var bytes = pixels.DetachPixelData();

                        encoder.SetPixelData(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode,
                                  (uint)decoder.PixelWidth,
                                  (uint)decoder.PixelHeight,
                                  decoder.DpiX,
                                  decoder.DpiY,
                                  bytes);
                        await encoder.FlushAsync();

                        action?.Invoke();
                    }
                }
            }
            catch
            {
            }
        }

        public static async Task DownloadImageAsync(string requestUri, string appName, string folderName, string fileName, Action action = null)
        {
            try
            {
                if (requestUri == null)
                {
                    return;
                }
                Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                if (fileName.EndsWith("jpg", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                else if (fileName.EndsWith("png", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
                else if (fileName.EndsWith("bmp", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
                else if (fileName.EndsWith("tiff", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
                else if (fileName.EndsWith("gif", StringComparison.CurrentCultureIgnoreCase))
                    BitmapEncoderGuid = BitmapEncoder.GifEncoderId;

                var storageFolder = KnownFolders.PicturesLibrary;
                var appfolder = await storageFolder.CreateFolderAsync(appName, CreationCollisionOption.OpenIfExists);
                var folder = await appfolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                HttpClient httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(requestUri);

                await FileIO.WriteBytesAsync(file, bytes);

                action?.Invoke();
            }
            catch
            {
            }
        }
    }
}
