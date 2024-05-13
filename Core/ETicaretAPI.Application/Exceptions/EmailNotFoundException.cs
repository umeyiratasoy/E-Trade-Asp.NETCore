using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Exceptions
{
    public class EmailNotFoundException : Exception
    {

        public EmailNotFoundException() : base("Böyle bir E-mail bulunamamaktadır.")
        {
        }

        public EmailNotFoundException(string? message) : base(message)
        {
        }

        public EmailNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
