using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HypeStock.Data.Models
{
    public class User: IdentityUser
    {
        public IEnumerable<UserBrandLikes> BrandLikes { get; set; }
        public IEnumerable<UserProductLikes> ProductLikes { get; set; }
    }
}
