using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading;
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
    public sealed partial class RecentPost : Page
    {
        const double targetWidth = 200;
        const double targetHeight = 300;

        Category category = null;
        string address = string.Empty;

        ObservableCollection<Gallery> galleries = new ObservableCollection<Gallery>();
        ScrollViewerHelper scrollViewerHelper = null;

        int minOverflowCount = 0;
        bool isLoading = false;
        int pageCount = 0;

        public RecentPost()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Category)
            {
                category = e.Parameter as Category;
                address = category.Link;
            }

            WorkListBox.ItemsSource = galleries;            
        }

        private void LoadGalleries()
        {
            Console.WriteLine("LoadGalleries");
            Task.Factory.StartNew(async () =>
            {
                await LoadMoreGalleries(1, async (count) =>
                {
                    pageCount = count;
                    await ListProgressBar.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        ListProgressBar.Visibility = Visibility.Collapsed;
                    });
                });
            });
        }

        private async void LoadMoreGalleries()
        {
            if(isLoading)
            {
                return;
            }

            isLoading = true;

            int fromPage = 0;

            if (galleries.Count > 0)
            {
                fromPage = (galleries.Count / pageCount);
            }

            int toPage = fromPage + 1;

            await LoadMoreGalleries(toPage);
        }

        private async Task LoadMoreGalleries(int page = 1, Action<int> action = null)
        {
            try
            {
                var galleries = await (await ApiHelper.GetApi()).GetGalleriesAsync(category, page++);

                isLoading = false;

                if (galleries != null && galleries.Count > 0)
                {
                    action?.Invoke(galleries.Count);

                    AddGalleries(galleries);
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
                            LoadMoreGalleries();
                        }));

                        messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("关闭"));

                        await messageDialog.ShowAsync();
                    });                    
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }            
        }

        private async void AddGalleries(List<Gallery> galleries)
        {
            await WorkListBox.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                galleries.ForEach(gallery =>
                {
                    this.galleries.Add(gallery);
                });                

                if (this.galleries.Count <= minOverflowCount)
                {
                    Task.Delay(500);
                    LoadMoreGalleries();
                }
            });
        }

        private void WorkListBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Gallery)
            {
                Gallery gallery = e.ClickedItem as Gallery;

                this.Frame.Navigate(typeof(PostDetail), e.ClickedItem, new Windows.UI.Xaml.Media.Animation.CommonNavigationTransitionInfo());
            }
        }

        private void WorkListBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (galleries.Count == 0)
            {
                LoadGalleries();

                scrollViewerHelper = new ScrollViewerHelper();
                scrollViewerHelper.Register(WorkListBox, (uint)ItemBridge.Height, () =>
                {                    
                    LoadMoreGalleries();
                });
            }
        }        

        private void WorkListBox_RefreshRequested(object sender, Controls.RefreshRequestedEventArgs e)
        {
            using (Deferral deferral = WorkListBox.AutoRefresh ? e.GetDeferral() : null)
            {
                galleries.Clear();
                ListProgressBar.Visibility = Visibility.Visible;
                LoadGalleries();
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            CalculateSize(sender);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateSize(sender);
        }

        private void CalculateSize(object sender)
        {
            var grid = (Grid)sender;

            var count = (int)(grid.ActualWidth / targetWidth);

            if(count < 2)
            {
                count = 2;
            }

            double itemWidth = grid.ActualWidth / count;
            double itemHeight = itemWidth * (targetHeight / targetWidth);

            ItemBridge.Width = (int)itemWidth;
            ItemBridge.Height = (int)itemHeight;

            var rowCount = (int)(grid.ActualHeight / ItemBridge.Height);

            minOverflowCount = rowCount * count;

            scrollViewerHelper?.SetBufferLength((uint)ItemBridge.Height);
        }
    }
}
