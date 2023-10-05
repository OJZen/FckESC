using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FckESC.Util
{
    public class NetworkUtil
    {
        private static string MAC;
        private static string IP;
        private static string NASIP = "119.146.175.80"; //默认值
        private static string INTRANETIP = "172.17.18.2";   // 内网IP
        public enum ConnectResult
        {
            Ok,
            TimeOut,
            NetWorkError,
            IntranetError
        }

        public static string GetMac()
        {
            // 如果IP为空就尝试判断是否有连接
            if (string.IsNullOrEmpty(IP))
            {
                return "";
            }

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var interf in interfaces)
            {
                foreach (var item in interf.GetIPProperties().UnicastAddresses)
                {
                    // 判断本机所有IP与已获取到能够连接的IP相同的网卡的mac
                    if (item.Address.ToString() == IP)
                    {
                        MAC = BitConverter.ToString(interf.GetPhysicalAddress().GetAddressBytes());
                        return MAC;
                    }
                }
            }
            // 没有找到匹配的Mac，可能IP改变了。这里改回空。
            IP = "";
            return "";
        }

        public static string GetIP()
        {
            // 如果IP为空就尝试判断是否有连接
            if (string.IsNullOrEmpty(IP))
            {
                return "";
            }
            return IP;
        }

        /// <summary>
        /// 初始连接过程，这是最开始要执行的方法。
        /// </summary>
        /// <returns></returns>
        public static async Task<ConnectResult> ConnectAsync()
        {
            var redirectUrl = await GetRedirectUrlAsync();

            // 判断重定向地址，是否需要执行内网认证。
            if (redirectUrl.Contains(INTRANETIP))
            {
                // 需要内网认证才能连接
                return ConnectResult.IntranetError;
            }

            // 从天翼页面的url中拿到ip和nasip，
            if (redirectUrl.Contains("ip="))
            {
                if (await GetIPandNasIP(redirectUrl))
                {
                    //能拿到的话说明就连接成功了。
                    return ConnectResult.Ok;
                }
            }
            
            using (TcpClient tcp = new TcpClient())
            {
                try
                {
                    // 上面没拿到，就尝试用tcp连接，看看能不能成。
                    // 如果走到这里，八成是连接不上了。
                    if (tcp.ConnectAsync(INTRANETIP, 9092).Wait(1500))
                    {
                        string ip = ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address.ToString();
                        Console.WriteLine(ip);
                        IP = ip;
                        return ConnectResult.Ok;
                    }
                    else
                    {
                        Console.WriteLine("连接超时，可能不是校园网");
                        return ConnectResult.TimeOut;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("连接失败，没有网络");
                    return ConnectResult.NetWorkError;
                }
            }

        }

        /// <summary>
        /// 获取重定向的URL
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetRedirectUrlAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("http://www.msftconnecttest.com/redirect");
                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.ReadAsStringAsync().Result.Contains(INTRANETIP))
                    {
                        return INTRANETIP;
                    }
                    return response.RequestMessage.RequestUri.ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// 从重定向地址中拿到ip和nasip
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        private static async Task<bool> GetIPandNasIP(string redirectUrl = "")
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = await GetRedirectUrlAsync();
            }

            if (redirectUrl.Contains("ip="))
            {
                // 由重定向得到的url包含nasip和主机ip
                //http://enet.10000.gd.cn:10001/qs/main.jsp?wlanacip=119.146.175.80&wlanuserip=100.2.141.174
                Console.WriteLine("重定向：" + redirectUrl);
                Regex regex = new Regex(@"(?<=acip=)([0-9\.]+)");
                if (regex.IsMatch(redirectUrl))
                {
                    Match match = regex.Match(redirectUrl);
                    NASIP = match.Value;
                }

                regex = new Regex(@"(?<=userip=)([0-9\.]+)");
                if (regex.IsMatch(redirectUrl))
                {
                    Match match = regex.Match(redirectUrl);
                    IP = match.Value;
                }
                return true;
            }

            return false;
        }


        public static string GetNasip()
        {
            return NASIP;
        }

    }
}
