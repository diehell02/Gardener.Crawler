using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Gardener.Crawler.Client.UWP.Util;
using Gardener.Crawler.Client.UWP.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Gardener.Crawler.Client.UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImageViewer : Page
    {
        string appName = string.Empty;
        string folderName = string.Empty;
        Dictionary<string, Image> dictionary = new Dictionary<string, Image>();

        public ImageViewer()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                PhotosFlipView.ItemsSource = null;

                dynamic parameter = e.Parameter;
                string selectedItem = parameter.SelectedItem;
                List<string> itemsSource = parameter.ItemsSource;
                PhotosFlipView.ItemsSource = itemsSource;
                PhotosFlipView.SelectedItem = selectedItem;
                folderName = parameter.FolderName;

                folderName = FileHelper.FilterFolderName(folderName);
                appName = FileHelper.FilterFolderName(Windows.ApplicationModel.Package.Current.DisplayName);
            }
            catch
            {

            }
        }

        private async void DownloadAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string path = PhotosFlipView.SelectedItem as string;
            string fileName = path.Substring(path.LastIndexOf('/') + 1);
            Image image = null;

            if(dictionary.ContainsKey(path))
            {
                image = dictionary[path];                

                await FileHelper.SaveImageAsync(image, appName, folderName, fileName, () =>
                {
                    NotifyPopup notifyPopup = new NotifyPopup("下载完成！");
                    notifyPopup.Show();
                });
            }
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            if (sender is Image)
            {
                Image image = sender as Image;

                if (!dictionary.ContainsKey(image.DataContext as string))
                {
                    dictionary.Add(image.DataContext as string, image);
                }
            }
        }
    }
}
