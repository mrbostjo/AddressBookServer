using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AddressBookServer.Models
{
    public enum ContactProperties
    {
        FirstName, LastName, Address, Phone
    }

    [DebuggerDisplay("id:{id}, {firstName}\t{lastName}\t{address}\t{phone}")]
    public class Contact
    {
        public long id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
    }
}
