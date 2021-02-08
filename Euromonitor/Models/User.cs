using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Euromonitor.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
