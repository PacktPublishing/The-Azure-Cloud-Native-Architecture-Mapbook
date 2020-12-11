using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class OrderEvent
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public enum EventType { Created, Deleted, Updated}
        public EventType type { get; set; }
    }
   
}
