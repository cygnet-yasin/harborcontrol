using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// Response of api
    /// </summary>
    public class Response
    {
        /// <summary>
        /// status code
        /// </summary>
        public int statusCode { get; set; }

        /// <summary>
        /// data
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// error type
        /// </summary>
        public string errorType { get; set; }

        /// <summary>
        /// error message
        /// </summary>
        public string errorMSG { get; set; }

        /// <summary>
        /// error data
        /// </summary>
        public object extraData { get; set; }
    }
}