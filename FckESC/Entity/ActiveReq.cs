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
    /// 心跳包请求实体
    /// </summary>
    class ActiveReq : RequestEntity
    {
        public ActiveReq(string username)
        {
            Username = username;
            Clientip = NetworkUtil.GetIP();
            Nasip = NetworkUtil.GetNasip();
            Mac = NetworkUtil.GetMac();
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            Initial();
        }

        public ActiveReq(RequestEntity reqEntity) : this(reqEntity.Username)
        { }

        private void Initial()
        {
            string enStr = Clientip + Nasip + Mac + Timestamp + KEY;
            Console.WriteLine(enStr);
            string md5 = MD5Util.CreateMD5(enStr);
            Authenticator = md5;
        }

        public override string GetRequestUrl()
        {
            string url = "http://enet.10000.gd.cn:8001/hbservice/client/active";
            string param = string.Format("username={0}&clientip={1}&nasip={2}&mac={3}&timestamp={4}&authenticator={5}",
                Username, Clientip, Nasip, Mac, Timestamp, Authenticator);
            return url + "?" + param;
        }
    }
}
