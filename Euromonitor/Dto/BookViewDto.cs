using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euromonitor.Dto
{
    public class BookViewDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public double? Price { get; set; }
        public bool Subscribed { get; set; }
    }
}
