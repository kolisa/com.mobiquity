
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobiquity.exception;
using System;
public class APIException : Exception
{
    
    public APIException(string message) : base(message)
    {
        
    }
}
