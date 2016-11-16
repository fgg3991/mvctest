using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFirstMvc.Models
{
    
    public class RegisterModel
    {
        public string UserName { get; set; }
        public string UserId { get; set; }

        
        public string UserPassWord { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0} ,ID：{1}",UserName,UserId);
        }
    }

}