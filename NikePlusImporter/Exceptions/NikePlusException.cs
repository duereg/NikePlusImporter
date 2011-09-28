using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Import.NikePlus.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NikePlusException : ApplicationException 
    {
        public NikePlusException() : base("An error has occurred with Nike+") { }
        public NikePlusException(string message) : base(message) { }
        public NikePlusException(string message, Exception ex) : base(message, ex) { }
    }
}
