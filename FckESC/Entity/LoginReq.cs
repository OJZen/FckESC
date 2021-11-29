using FckESC.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FckESC.Entity
{
    /// <summary>
    /// 登录请求对象
    /// </summary>
    class LoginReq : RequestEntity
    {
        private string challenge;
        public LoginReq(string username, string password, string code)
        {
            Username = username;
            Clientip = NetworkUtil.GetIP();
            Nasip = NetworkUtil.GetNasip();
            Mac = NetworkUtil.GetMac();
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            Password = password;
            Iswifi = "4060";
            challenge = code;
            Initial();
        }

        public LoginReq(RequestEntity reqEntity, string password, string code)
            : this(reqEntity.Username, password, code)
        { }

        [JsonProperty("password")]
        string Password { get; set; }
        [JsonProperty("iswifi")]
        string Iswifi { get; set; }

        private void Initial()
        {
            string enStr = Clientip + Nasip + Mac + Timestamp + challenge + KEY;
            Console.WriteLine(enStr);
            string md5 = MD5Util.CreateMD5(enStr);
            Authenticator = md5;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override string GetRequestUrl()
        {
            return "http://enet.10000.gd.cn:10001/client/login";
        }
    }
}
