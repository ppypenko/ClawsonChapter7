using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities
{
    class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        public string UserPassword { get; set; }
    }
}
