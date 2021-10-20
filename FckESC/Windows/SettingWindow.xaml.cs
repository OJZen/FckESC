using FckESC.Util;
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
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
            Toggle_AutoSave.IsChecked = AppSettings.Default.autoSave;
            Toggle_Boot.IsChecked = AppSettings.Default.autoBoot;
        }

        private void Button_SaveSetting_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.Default.autoSave = Toggle_AutoSave.IsChecked.Value;
            AppSettings.Default.autoBoot = Toggle_Boot.IsChecked.Value;
            AppSettings.Default.Save();
            AutoBoot.SetAutoStart(Toggle_Boot.IsChecked.Value);
            //MessageBox.Show("保存完成", "提示：", MessageBoxButton.OK);
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
