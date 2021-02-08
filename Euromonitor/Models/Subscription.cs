using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Euromonitor.Models
{
    public class Subscription
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int BookID { get; set; }
        public bool Active { get; set; }
    }
}
