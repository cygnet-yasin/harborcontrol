using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// initial parameter of application
    /// </summary>
    public class InitialParameter
    {
        /// <summary>
        /// initial parameter constructor
        /// </summary>
        public InitialParameter()
        {
            autoGenerateBoatTime = 1;
            oneHourPerSecond = 10;
            perimeterLineDistance = 10;
        }

        /// <summary>
        /// boat count
        /// </summary>
        public int boatCount { get; set; }

        /// <summary>
        /// anchor size on harbor
        /// </summary>
        public int anchorSize { get; set; }

        /// <summary>
        /// single boat anchor time 
        /// </summary>
        public int anchorTime { get; set; }

        /// <summary>
        /// wind speed at harbor
        /// </summary>
        public decimal windSpeed { get; set; }

        /// <summary>
        /// auto generate boat time interval
        /// </summary>
        public int autoGenerateBoatTime { get; set; }

        /// <summary>
        /// one hour per seconds
        /// </summary>
        public int oneHourPerSecond { get; set; }

        /// <summary>
        /// perimeter line distance from dock
        /// </summary>
        public int perimeterLineDistance { get; set; }

        /// <summary>
        /// next auto generate boat time
        /// </summary>
        public DateTime nextAuotGeneratedBoatTime { get; set; }

        /// <summary>
        /// token of application
        /// </summary>
        public string token { get; set; }
    }
}