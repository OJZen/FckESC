using FckESC.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FckESC.View
{
    /// <summary>
    /// LoginResultView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginResultView : UserControl
    {
        public LoginResultView()
        {
            InitializeComponent();
            var viewModel = LoginResultViewModel.GetInstance();
            
            Text_Result.SetBinding(TextBlock.TextProperty, new Binding("LoginInfo") { Source = viewModel});
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
