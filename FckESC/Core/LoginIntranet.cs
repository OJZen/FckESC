using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FckESC
{
    /// <summary>
    /// 登录认证内网
    /// </summary>
    class LoginIntranet
    {

        /// <summary>
        /// 内网认证过程
        /// </summary>
        /// <param name="account">账户，一般是学号</param>
        /// <param name="password">密码，一般是身份证后8位，最后一位如果是X要大写</param>
        /// <returns></returns>
        public static async Task<bool> LoginAsync(string account, string password)
        {
            string viewState = "";

            using (var httpClient = new HttpClient())
            {
                // 设置httpclient的请求头
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36 Edg/87.0.664.66");

                var httpResponse = await httpClient.GetAsync("http://172.17.18.2:8080/byod/index.xhtml");

                if (httpResponse.IsSuccessStatusCode)
                {
                    var html = httpResponse.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(html);

                    if (!html.Contains("javax.faces.ViewState"))
                    {
                        return false;
                    }

                    Regex regex = new Regex(@"[0-9a-zA-Z/+]{100,150}");
                    // 拿到短的ViewState
                    if (regex.IsMatch(html))
                    {
                        Match match = regex.Match(html);
                        viewState = match.Value;
                    }
                }

                if (string.IsNullOrEmpty(viewState))
                {
                    return false;
                }

                // 设置请求文本，注意ViewState要进行url编码
                var data = string.Format("wlannasid=&usermac=&userurl=&userip=&ssid=&btn=&j_id_3_SUBMIT=1&javax.faces.ViewState={0}",
                    Uri.EscapeDataString(viewState));
                var content = new StringContent(data);
                content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

                httpResponse = await httpClient.PostAsync("http://172.17.18.2:8080/byod/index.xhtml", content);
                
                // 清空，后面用
                var redirectUrl = "";
                viewState = "";

                if (httpResponse.IsSuccessStatusCode)
                {
                    // 重定向的地址
                    redirectUrl = httpResponse.RequestMessage.RequestUri.ToString();
                    Console.WriteLine(redirectUrl);
                    // "http://172.17.18.2:8080/byod/templatePage/20200318110733542/guestRegister.jsf"
                    httpResponse = await httpClient.GetAsync(redirectUrl);

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var html = httpResponse.Content.ReadAsStringAsync().Result;
                        if (!html.Contains("javax.faces.ViewState"))
                        {
                            return false;
                        }

                        Regex regex = new Regex(@"[0-9a-zA-Z/+]{500,600}");
                        // 拿到长的ViewState
                        if (regex.IsMatch(html))
                        {
                            Match match = regex.Match(html);
                            viewState = match.Value;
                        }
                    }
                }

                if (string.IsNullOrEmpty(viewState))
                {
                    return false;
                }

                // 请求文本
                var loginParam = "javax.faces.partial.ajax=true&javax.faces.source=mainForm%3Aj_id_r&javax.faces.partial.execute=mainForm&javax.faces.partial.render=mainForm%3Aerror+mainForm%3AforResetPwd&mainForm%3Aj_id_r=mainForm%3Aj_id_r&mainForm%3AforResetPwd={0}&mainForm%3AuserNameLogin={1}&mainForm%3AserviceSuffixLogin=&mainForm%3ApasswordLogin={2}&mainForm_SUBMIT=1&javax.faces.ViewState={3}";
                // 转base64编码
                var passwordB64 = Convert.ToBase64String(Encoding.Default.GetBytes(password));
                // 还要进行url编码
                passwordB64 = Uri.EscapeDataString(passwordB64);
                viewState = Uri.EscapeDataString(viewState);
                // 构建请求文本
                data = string.Format(loginParam, password, account, passwordB64, viewState);
                content = new StringContent(data);
                content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";
                // 进行post请求
                httpResponse = await httpClient.PostAsync(redirectUrl, content);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var html = httpResponse.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(html);
                    //<?xml version="1.0" encoding="utf-8"?><partial-response><redirect url="/byod/templatePage/20200318110733542/show_result.jsf"></redirect></partial-response>
                    if (html.Contains("show_result"))
                    {
                        #region 判断是否登录成功，走到这里应该不用判断了。八成是成了。
                        /*
                        Regex regex = new Regex("(?<=url=\").+ (?= \")");
                        if (regex.IsMatch(html))
                        {
                            redirectUrl = "http://172.17.18.2:8080" + regex.Match(html).Value;

                            httpResponse = await httpClient.GetAsync(redirectUrl);
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                html = httpResponse.Content.ReadAsStringAsync().Result;
                                if (html.Contains("成功"))
                                {

                                }
                            }
                        }
                        */
                        #endregion
                        return true;
                    }
                }

                return false;
            }
        }

        public static void Logout()
        {
            //下线的方法暂时不写，因为没啥卵用
        }
    }
}
