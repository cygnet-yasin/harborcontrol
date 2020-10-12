using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Harbor
{
    public class Response
    {
        public int statusCode { get; set; }
        public object data{ get; set; }
        public string errorType{ get; set; }
        public string errorMSG{ get; set; }
        public object extraData{ get; set; }
    }
}