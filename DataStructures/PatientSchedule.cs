using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.DataStructures
{
    public class PatientSchedule
    {
        public int PatientID { get; set; }
        public string PatientName{ get; set; }
        public List<Hemodialysy> Hemodialysis { get; set; }

    }

    public struct Hemodialysy
    {
        public HemodialysisItem hemodialysisItem { get; set; }
        public DialysisTime dialysisTime { get; set; }

    }

    public struct DialysisTime
    {
        public DateTime dateTime { get; set; }
        public AmOrPm AmPm { get; set; }

    }
    public enum HemodialysisItem
    {
        HDF = 0,
        HD, 
        HDHP
    }

    public enum AmOrPm
    {
        AM = 0, 
        PM
    }
}
