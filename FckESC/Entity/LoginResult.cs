using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FckESC.Entity
{
    class LoginResult
    {
        public LoginResult(Type type)
        {
            ResultType = type;
        }

        public string ReusltInfo
        {
            get
            {
                switch (ResultType)
                {
                    case Type.Error:
                        return "登录失败，未知错误";
                    case Type.AccountIncorrect:
                        return "登录失败，账号错误";
                    case Type.PasswordIncorrect:
                        return "登录失败，密码错误";
                    case Type.ParamError:
                        return "登录失败，参数错误";
                    case Type.Suspend:
                        return "登录失败，服务已暂停，可能是账号欠费";
                    case Type.CodeError:
                        return "验证码处理失败";
                    case Type.Ok:
                        return "登录成功";
                }
                return "返回类型无效";
            }
        }

        public Type ResultType { get; set; }

        public enum Type
        {
            Ok,
            Error,
            AccountIncorrect,
            PasswordIncorrect,
            ParamError,
            Suspend,
            CodeError
        }
    }


}
