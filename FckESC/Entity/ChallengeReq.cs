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
    /// 验证码请求的实体
    /// </summary>
    class ChallengeReq : RequestEntity
    {
        public ChallengeReq(string username)
        {
            Version = "214";
            Username = username;
            Clientip = NetworkUtil.GetIP();
            Nasip = NetworkUtil.GetNasip();
            Mac = NetworkUtil.GetMac();
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            Initial();
        }

        private void Initial()
        {
            string enStr = Version + Clientip + Nasip + Mac + Timestamp + KEY;
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
            return "http://enet.10000.gd.cn:10001/client/vchallenge";
        }

    }
}
