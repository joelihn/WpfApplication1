using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.DataStructures
{
    public class PatientSchedule
    {
        public long PatientID { get; set; }
        //public string PatientName{ get; set; }
        public List<Hemodialysy> Hemodialysis { get; set; }

        public PatientSchedule( long patientID )
        {
            PatientID = patientID;
            //PatientName = "";
            Hemodialysis = new List<Hemodialysy>();
        }

    }

    public class Hemodialysy
    {
        public string hemodialysisItem { get; set; }
        public DialysisTime dialysisTime { get; set; }

        public Hemodialysy()
        {
            hemodialysisItem = "";
            dialysisTime = new DialysisTime();
        }

        public Hemodialysy( string treatMent, DateTime dateTime, string ampme )
        {
            hemodialysisItem = treatMent;
            dialysisTime = new DialysisTime(dateTime, ampme);
        }

    }

    public class DialysisTime
    {
        public DateTime dateTime { get; set; }
        public string AmPmE { get; set; }

        public DialysisTime()
        {
            dateTime = new DateTime();
            AmPmE = "";
        }

        public DialysisTime(DateTime _dateTime, string ampme)
        {
            dateTime = _dateTime;
            AmPmE = ampme;
        }

    }


}
