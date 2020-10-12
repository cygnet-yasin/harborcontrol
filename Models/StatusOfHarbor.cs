using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    /// <summary>
    /// Status of harbot
    /// </summary>
    public class StatusOfHarbor
    {
        /// <summary>
        /// Status of harbor constructor
        /// </summary>
        public StatusOfHarbor()
        {
            boatStatus = new List<BoatStatus>();
        }

        /// <summary>
        /// Initial parameter of harbor application
        /// </summary>
        public InitialParameter initialParameter { get; set; }

        /// <summary>
        /// boat status list
        /// </summary>
        public List<BoatStatus> boatStatus { get; set; }
    }
}