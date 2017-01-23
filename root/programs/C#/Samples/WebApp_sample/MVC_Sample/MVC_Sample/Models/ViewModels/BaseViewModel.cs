using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;


namespace MVC_Sample.Models.ViewModels
{
    public class BaseViewModel
    {

        /// <summary>UserName</summary>
        public static string UserName
        {
            get
            {
                MyUserInfo myUserInfo = (MyUserInfo)UserInfoHandle.GetUserInformation();
                if (myUserInfo == null)
                {
                    return "anonymous";
                }
                else
                {
                    return myUserInfo.UserName;
                }
                
            }
        }
    }
}