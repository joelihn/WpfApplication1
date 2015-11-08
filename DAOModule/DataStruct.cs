#region File Header Text

#endregion

#region Using References

#endregion

using System;

namespace WpfApplication1.DAOModule
{

    public class Bed
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public Int64 TreatTypeId { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOccupy { get; set; }
        public string Description { get; set; }
        public bool IsTemp { get; set; }
        public Int64 PatientAreaId { get; set; }
        public Int64 MachineTypeId { get; set; }
        public Int64 InfectTypeId { get; set; }//Joe
        public string Reserved { get; set; }
    }

    public class InfectType
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class MachineType
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string BgColor { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class MedicalOrderPara
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Int64 Count { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class MedicalOrder
    {
        public Int64 Id { get; set; }
        public Int64 PatientId { get; set; }
        public bool Activated { get; set; }
        public string Seq { get; set; }
        public string Plan { get; set; }
        public Int64 MethodId { get; set; }
        public Int64 Interval { get; set; }
        public Int64 Times { get; set; }
        public string Description { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
    }

    public class Patient
    {
        public Int64 Id { get; set; } //TODO ?
        public string PatientId { get; set; }
        public string Name { get; set; }
        public string Dob { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Marriage { get; set; }
        public string Height { get; set; }
        public string BloodType { get; set; }
        public Int64 InfectTypeId { get; set; }
        public Int64 TreatStatusId { get; set; }
        public bool IsFixedBed { get; set; }
        public string IdCode { get; set; }
        public Int64 AreaId { get; set; }
        public string Mobile { get; set; }
        public string ZipCode { get; set; }
        public string WeiXinHao { get; set; }
        public string Payment { get; set; }
        public string Orders { get; set; }
        public string RegisitDate { get; set; }
        public Int64 BedId { get; set; }
        public bool IsAssigned { get; set; }
        public string Description { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
    }

    public class PatientArea
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Int64 InfectTypeId { get; set; }
        public string Description { get; set; }
        public Int64 Seq { get; set; }
        public string Position { get; set; }
        public string Reserved { get; set; }
    }

    public class PatientDepartment
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class PatientGroup
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class PatientGroupPara
    {
        public Int64 Id { get; set; }
        public Int64 GroupId { get; set; }
        public string Left { get; set; }
        public string Key { get; set; }
        public string Symbol { get; set; }
        public string Value { get; set; }
        public string Right { get; set; }
        public string Logic { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class PatientRoom
    {
        public Int64 Id { get; set; }
        public Int64 PatientAreaId { get; set; }
        public string Name { get; set; }
        public Int64 InfectTypeId { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class ScheduleTemplate
    {
        public Int64 Id { get; set; }
        public Int64 PatientId { get; set; }
        public string Date { get; set; }
        public string Method { get; set; }
        public string AmPmE { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
        public Int64 BedId { get; set; }
        public bool IsTemp { get; set; }
        public bool IsAuto { get; set; }
    }

    public class ScheduleType
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public Int64 PatientId { get; set; }
        public string TimeRange { get; set; }
        public Int64 Type { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class TreatMethod
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public bool SinglePump { get; set; }
        public bool DoublePump { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
        public string BgColor { get; set; }
        public bool IsAvailable { get; set; }
        public Int64 TreatTypeId { get; set; }//Need Delete
    }

    public class TreatType
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
        public string BgColor { get; set; }
    }

    public class TreatStatus
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public bool Activated { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class TreatTime
    {
        public Int64 Id { get; set; }
        public bool Activated { get; set; }
        public string Name { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class BedDetails
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    //public enum SeriesType
    //{
    //    AnatomyImage = 0,
    //    FuctionImage = 1,
    //    MergeImage
    //}

}