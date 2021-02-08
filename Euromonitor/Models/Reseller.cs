using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Euromonitor.Models
{
    public class Reseller
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}
