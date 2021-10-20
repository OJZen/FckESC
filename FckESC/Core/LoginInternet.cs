using FckESC.Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FckESC
{
    /// <summary>
    /// 登录认证的类，用于获取验证码（challenge）、登录、注销以及保持连接。
    /// </summary>
    class LoginInternet
    {

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="req">验证码请求体</param>
        /// <returns></returns>
        public async Task<string> ChallengeAsync(ChallengeReq req)
        {
            using (var httpClient = new HttpClient())
            {
                var data = new StringContent(req.GetJson());

                var response = await httpClient.PostAsync(req.GetRequestUrl(), data);

                if (response.IsSuccessStatusCode)
                {
                    //{"rescode":"-1","resinfo":"authenticator fail"}
                    //{ "challenge":"9XTR","rescode":"0","resinfo":"this user is ok!"}
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                    JObject o = JObject.Parse(result);
                    if (o.SelectToken("rescode").ToString() == "0")
                    {
                        return o.SelectToken("challenge").ToString();
                    }
                    return "";
                }
            }
            return "";
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req">登录请求体</param>
        /// <returns></returns>
        public async Task<LoginResult.Type> LoginAsync(LoginReq req)
        {
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine(req.GetJson());

                var data = new StringContent(req.GetJson());

                var response = await httpClient.PostAsync(req.GetRequestUrl(), data);

                if (response.IsSuccessStatusCode)
                {
                    //登录失败，一般是参数错误 {"rescode":"-1","resinfo":"authenticator fail"}
                    //登录成功 {"rescode":"0","resinfo":"login success"}
                    //账号错误 {"rescode":"13016000","resinfo":"（Huawei）Product Not Ordered"}
                    //密码错误 {"rescode":"13012000","resinfo":"(Huawei)Password Error"}
                    //暂停服务/账号欠费 {"resinfo":"（Huawei） Service Suspended","rescode":"13017000"}

                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);

                    JObject o = JObject.Parse(result);
                    switch (o.SelectToken("rescode").ToString())
                    {
                        case "0":
                            return LoginResult.Type.Ok;
                        case "13016000":
                            return LoginResult.Type.AccountIncorrect;
                        case "13012000":
                            return LoginResult.Type.PasswordIncorrect;
                        case "13017000":
                            return LoginResult.Type.Suspend;
                        default:
                            return LoginResult.Type.ParamError;
                    }

                }
            }
            return LoginResult.Type.Error;
        }

        /// <summary>
        /// 保持连接
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<bool> KeepActiveAsync(ActiveReq req)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(req.GetRequestUrl());

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    JObject o = JObject.Parse(result);
                    if (o.SelectToken("rescode").ToString() == "0")
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<bool> LogoutAsync(LogoutReq req)
        {
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine(req.GetJson());

                var data = new StringContent(req.GetJson());

                var response = await httpClient.PostAsync(req.GetRequestUrl(), data);

                if (response.IsSuccessStatusCode)
                {
                    //{"rescode":"-1","resinfo":"authenticator fail"}
                    //{"rescode":"0","resinfo":"logout success"}
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(result);
                    JObject o = JObject.Parse(result);
                    if (o.SelectToken("rescode").ToString() == "0")
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }


    }
}
