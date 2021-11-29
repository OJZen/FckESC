using FckESC.Entity;
using FckESC.Util;
using FckESC.ViewModel;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FckESC.View
{
    /// <summary>
    /// UserInputView.xaml 的交互逻辑
    /// </summary>
    public partial class UserInputView : UserControl
    {
        //DispatcherTimer keepActTimer;
        // int reTry = 3;

        public UserInputView()
        {
            InitializeComponent();
            /*
            keepActTimer = new DispatcherTimer();
            keepActTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            keepActTimer.Interval = new TimeSpan(0, 0, 30);
            */

            //判断是否是自动保存账号
            if (AppSettings.Default.autoSave)
            {
                Text_Account.Text = AppSettings.Default.account;
                Text_Password.Password = AppSettings.Default.password;
            }
            else
            {
                // 没有自动保存的就清空
                AppSettings.Default.account = "";
                AppSettings.Default.password = "";
                AppSettings.Default.Save();
            }
        }

        private async Task LoginAsync()
        {
            LoginInternet loginObj = new LoginInternet();
            LoginResult.Type resultType;
            ChallengeReq challengeReq = new ChallengeReq(Text_Account.Text);

            var result = await loginObj.ChallengeAsync(challengeReq);
            if (string.IsNullOrEmpty(result))
            {
                resultType = LoginResult.Type.CodeError;
            }

            LoginReq loginReq = new LoginReq(challengeReq, Text_Password.Password, result);

            resultType = await loginObj.LoginAsync(loginReq);

            // loginResult

            LoginResultViewModel viewModel = LoginResultViewModel.GetInstance();
            viewModel.LoginInfo = new LoginResult(resultType).ReusltInfo;

            //keepActTimer.Start();

            // 自动保存账号
            if (AppSettings.Default.autoSave)
            {
                AppSettings.Default.account = Text_Account.Text;
                AppSettings.Default.password = Text_Password.Password;
                AppSettings.Default.Save();
            }
        }

        /*
        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (reTry <= 0)
            {
                keepActTimer.Stop();
                Console.WriteLine("维持连接失败");
                return;
            }
            ActiveReq activeReq = new ActiveReq(Text_Account.Text);
            var result = await KeepActiveAsync(activeReq);
            if (!result && reTry > 0)
            {
                reTry--;
            }
            else
            {
                Console.WriteLine("维持连接成功");
            }
        }
        
        private async Task<bool> KeepActiveAsync(ActiveReq req)
        {
            LoginInternet laborer = new LoginInternet();
            return await laborer.KeepActiveAsync(req);
        }

        */

        /// <summary>
        /// 登录按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            MoveNext();
            //http://enet.10000.gd.cn:10001/qs/main.jsp?wlanacip=119.146.175.80&wlanuserip=100.2.43.63

            if (string.IsNullOrEmpty(Text_Account.Text) || string.IsNullOrEmpty(Text_Password.Password))
            {
                MessageBox.Show("账号或者密码不能为空！", "提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                MovePrevious();
                return;
            }

            try
            {
                NetworkUtil.ConnectResult connResult = await NetworkUtil.ConnectAsync();
                switch (connResult)
                {
                    case NetworkUtil.ConnectResult.TimeOut:
                        MessageBox.Show("请检查是否连接到校园网。", "网络不通", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case NetworkUtil.ConnectResult.NetWorkError:
                        MessageBox.Show("请检查是否连接到网络。", "网络出错", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    case NetworkUtil.ConnectResult.IntranetError:
                        if (MessageBox.Show("是否自动登录认证内网？", "需要认证内网", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            var isSuccess = await LoginIntranet.LoginAsync(Text_Account.Text, Text_Password.Password);
                            if (isSuccess)
                            {
                                MessageBox.Show("网络连接可能会暂时断开，这是内网认证后的正常现象，稍等半分钟后再次点击登录按钮进行认证。", "认证成功", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show("请手动在浏览器认证登录内网。", "认证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("请手动在浏览器认证内网后再点击登录。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        return;
                    default:
                        break;
                }
                if (connResult != NetworkUtil.ConnectResult.Ok)
                {
                    MovePrevious();
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message, "网络错误");
                MovePrevious();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "其他错误");
                MovePrevious();
                return;
            }

            await LoginAsync();
            MoveNext();
            Console.WriteLine("登录完成");
        }

        /// <summary>
        /// 移动到下一个页面
        /// </summary>
        private void MoveNext()
        {
            Transitioner.MoveNextCommand.Execute(Button_Login, Button_Login);
        }

        /// <summary>
        /// 返回上一个页面
        /// </summary>
        private void MovePrevious()
        {
            Transitioner.MovePreviousCommand.Execute(Button_Login, Button_Login);
        }

    }
}
