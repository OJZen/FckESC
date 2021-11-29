using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FckESC.Windows
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void Button_QQ_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("639824978");
            SnackbarTips.MessageQueue.Enqueue("已复制");
        }

        private void Button_Email_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("admin@jzen.tech");
            SnackbarTips.MessageQueue.Enqueue("已复制");
        }

        private void Button_Github_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/OJZen/FckESC");
        }

        private void Button_Gitee_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://gitee.com/ojun/FckESC");
        }
    }
}
