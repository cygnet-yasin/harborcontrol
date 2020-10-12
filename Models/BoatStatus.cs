using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// boat status
    /// </summary>
    public class BoatStatus
    {
        /// <summary>
        /// boat name
        /// </summary>
        public Boats boats { get; set; }

        /// <summary>
        /// boat position
        /// </summary>
        public BoatPosition boatPosition { get; set; }

        /// <summary>
        /// estimate time to reach next position of this boat
        /// </summary>
        public DateTime? estimateToReachNextPosition { get; set; }
    }
}