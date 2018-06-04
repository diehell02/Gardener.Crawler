using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Gardener.Crawler.Api.Entity;
using Gardener.Crawler.Api;
using Gardener.Crawler.Client.UWP.Util;
using Gardener.Crawler.Client.UWP.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Gardener.Crawler.Client.UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PostDetail : Page
    {
        private int listCount = 0;
        private volatile int downloadCount = 0;
        private object lockObject = new object();

        string appName = string.Empty;
        string folderName = string.Empty;
        Dictionary<string, Image> dictionary = new Dictionary<string, Image>();

        private Gallery gallery = null;
        private Post post = null;

        public PostDetail()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Gallery _gallery = null;

            if (e.Parameter is Gallery)
            {
                _gallery = e.Parameter as Gallery;
            }

            if (gallery != _gallery)
            {
                gallery = _gallery;

                LoadData();
            }
        }

        private async void LoadData()
        {
            PostImages.ItemsSource = null;

            post = await (await ApiHelper.GetApi()).GetPost(gallery);

            if (post != null)
            {
                if(post.Images == null)
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog("对不起，尚未支持查看该页面")
                    {
                        Title = "提示",
                    };

                    messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", (command) =>
                    {
                        Frame.GoBack();
                    }));

                    await messageDialog.ShowAsync();

                    return;
                }

                PostImages.ItemsSource = post.Contents;
                PhotosFlipView.ItemsSource = post.Images;

                folderName = FileHelper.FilterFolderName(folderName);
                appName = FileHelper.FilterFolderName(Windows.ApplicationModel.Package.Current.DisplayName);

                WebView webView = new WebView();
            }
            else
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog("网络异常，是否重试？")
                    {
                        Title = "提示",
                    };

                    messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("重试", (command) =>
                    {
                        LoadData();
                    }));

                    messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("关闭"));

                    await messageDialog.ShowAsync();
                });
            }
        }

        private void PostImages_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Post.Image)
            {
                FlipViewGridShow(e.ClickedItem);
            }
        }

        private void DownloadAllAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(post != null)
            {
                NotifyPopup notifyPopup = new NotifyPopup("开始下载全部图片...");
                notifyPopup.Show();

                List<Post.Image> imgs = post.Images;

                listCount = imgs.Count;

                imgs.ForEach(async img =>
                {
                    string imageUrl = img.Url;

                    string fileName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);

                    if (dictionary.ContainsKey(imageUrl))
                    {
                        Image image = dictionary[imageUrl];

                        await FileHelper.SaveImageAsync(image, appName, folderName, fileName, () =>
                        {
                            CheckDownloadFinish();
                        });
                    }
                    else
                    {
                        await FileHelper.DownloadImageAsync(imageUrl, appName, folderName, fileName, () =>
                        {
                            CheckDownloadFinish();
                        });
                    }
                });
            }
        }

        private void CheckDownloadFinish()
        {
            lock (lockObject)
            {
                if (downloadCount < listCount)
                {
                    downloadCount++;
                    NotifyPopup notifyPopup = new NotifyPopup("下载完成！");
                    notifyPopup.Show();
                }
            }
        }

        private async void DownloadAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(PhotosFlipView.SelectedItem is Post.Image)
            {
                string path = (PhotosFlipView.SelectedItem as Post.Image).Url;
                string fileName = path.Substring(path.LastIndexOf('/') + 1);
                Image image = null;

                if (dictionary.ContainsKey(path))
                {
                    image = dictionary[path];

                    NotifyPopup notifyPopup = new NotifyPopup("开始下载图片...");
                    notifyPopup.Show();

                    await FileHelper.SaveImageAsync(image, appName, folderName, fileName, async () =>
                    {
                        await notifyPopup.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            notifyPopup.Visibility = Visibility.Collapsed;

                            notifyPopup = new NotifyPopup("下载完成！");
                            notifyPopup.Show();
                        });
                    });
                }
            }            
        }

        private async void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            if (sender is Image)
            {
                Image image = sender as Image;

                if(image.DataContext is Post.Image)
                {
                    Post.Image data = (image.DataContext as Post.Image);

                    string key = data.Url;

                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, image);

                        await PostImages.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            data.IsOpen = true;
                        });
                    }
                }
            }
        }

        private void FlipViewCloseButton_Click(object sender, RoutedEventArgs e)
        {
            FlipViewGridHide();
        }

        private void ScalableGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlipViewGridHide();
        }

        private void FlipViewGridShow(object selectItem)
        {
            PhotosFlipView.SelectedItem = selectItem;
            FlipViewGrid.Visibility = Visibility.Visible;
        }

        private void FlipViewGridHide()
        {
            FlipViewGrid.Visibility = Visibility.Collapsed;
        }
    }
}
