using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Xaml2Png {
    public sealed partial class MainPage {
        public MainPage() {
            InitializeComponent();
            PngButton.Click += PngButton_Click;
        }

        private async void PngButton_Click(object sender, RoutedEventArgs e)
        {
            await SaveImage();
            await LoadImage();
        }

        private async Task SaveImage() {
            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(text);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("text.png", CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    (uint)renderTargetBitmap.PixelWidth,
                    (uint)renderTargetBitmap.PixelHeight, 96d, 96d,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }

            await LoadImage();
        }


        // We cannot do: image.Source = new BitmapImage(new Uri("ms-appdata:///local/text.png"));
        // this locks the file and can cause an UnauthorizedAccessException
        // so we need to open the file as a stream.
        private async Task LoadImage() {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync("text.png");
            var stream = await file.OpenAsync(FileAccessMode.Read);
            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(stream);
            image.Source = bitmapImage;
        }
    }
}
