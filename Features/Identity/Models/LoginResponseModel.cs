using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Features.Identity.Models
{
    public class LoginResponseModel
    {
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
