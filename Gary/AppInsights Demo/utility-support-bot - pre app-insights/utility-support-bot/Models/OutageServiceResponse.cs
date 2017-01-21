using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnwAssistBot.Models
{

    public class GetOutagesResponse
    {
        public OutageDetail[] Outages { get; set; }
    }

    public class OutageDetail
    {
        public DateTime DataLastUpdatedOriginal { get; set; }
        public string DateLastUpdated { get; set; }
        public DateTime OutageDateOriginal { get; set; }
        public string OutageDate { get; set; }
        public string FullPostcode { get; set; }
        public string StreetName { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string FaultType { get; set; }
        public string PlannedOutageStartDate { get; set; }
        public DateTime EstimatedTimeOfRestorationOriginal { get; set; }
        public string EstimatedTimeOfRestoration { get; set; }
        public DateTime EstimatedTimeOfRestorationMajorityOriginal { get; set; }
        public string EstimatedTimeOfRestorationMajority { get; set; }
        public string CustomerInformation { get; set; }
        public string ActualTimeRestored { get; set; }
        public string PinPath { get; set; }
        public float DistanceFrom { get; set; }
        public string Distance { get; set; }
    }

}