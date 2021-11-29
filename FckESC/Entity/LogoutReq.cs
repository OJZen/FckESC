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
    /// 注销请求对象
    /// </summary>
    class LogoutReq : RequestEntity
    {
        public LogoutReq(string username)
        {
            Username = username;
            Clientip = NetworkUtil.GetIP();
            Nasip = NetworkUtil.GetNasip();
            Mac = NetworkUtil.GetMac();
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            Initial();
        }

        private void Initial()
        {
            string enStr = Clientip + Nasip + Mac + Timestamp  + KEY;
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
            return "http://enet.10000.gd.cn:10001/client/logout";
        }
    }
}
