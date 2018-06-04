using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Gardener.Crawler.Api.Entity;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Gardener.Crawler.Client.UWP.Controls
{
    public class CustomTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Post.Content)
            {
                if (item is Post.Image)
                {
                    if (App.Current.Resources.ContainsKey("PostImageTemplate"))
                    {
                        return App.Current.Resources["PostImageTemplate"] as DataTemplate;
                    }
                }
                else if (item is Post.Body)
                {
                    if (App.Current.Resources.ContainsKey("PostBodyTemplate"))
                    {
                        return App.Current.Resources["PostBodyTemplate"] as DataTemplate;
                    }
                }
                else if (item is Post.Title)
                {
                    if (App.Current.Resources.ContainsKey("PostTitleTemplate"))
                    {
                        return App.Current.Resources["PostTitleTemplate"] as DataTemplate;
                    }
                }
            }            

            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if(item is Post.Content)
            {
                if(item is Post.Image)
                {
                    if(App.Current.Resources.ContainsKey("PostImageTemplate"))
                    {
                        return App.Current.Resources["PostImageTemplate"] as DataTemplate;
                    }
                }
                else if(item is Post.Body)
                {
                    if (App.Current.Resources.ContainsKey("PostBodyTemplate"))
                    {
                        return App.Current.Resources["PostBodyTemplate"] as DataTemplate;
                    }                    
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
