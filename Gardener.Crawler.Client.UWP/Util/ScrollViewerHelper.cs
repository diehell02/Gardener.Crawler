using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Gardener.Crawler.Client.UWP.Util
{
    class ScrollViewerHelper
    {
        bool isLoading = false;
        ScrollViewer sv = null;
        ScrollBar sb = null;
        Action action = null;

        uint bufferLength = 0;

        public void Register(DependencyObject parent, uint bufferLength, Action action)
        {
            sv = FindVisualChildByName<ScrollViewer>(parent, "ScrollViewer");
            sb = FindVisualChildByName<ScrollBar>(sv, "VerticalScrollBar");
            this.action = action;
            this.bufferLength = bufferLength;

            sb.ValueChanged += Sb_ValueChanged;
        }

        public void SetBufferLength(uint bufferLength)
        {
            this.bufferLength = bufferLength;
        }

        //寻找子节点，用于查找listview的scrollviewer和scrollbar。
        private T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }

            }
            return null;
        }

        private void Sb_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.NewValue > e.OldValue && e.NewValue >= (sb.Maximum - bufferLength))
            {
                if (!isLoading)
                {
                    isLoading = true;
                    //加载需要加载的内容。
                    Task.Factory.StartNew(() =>
                    {
                        action?.Invoke();
                        isLoading = false;
                    });
                }
            }
        }
    }
}
