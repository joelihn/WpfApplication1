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
        public Int64 PatientRoomId { get; set; }
        public string Name { get; set; }
        public Int64 Type { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOccupy { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class InfectType
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class Interval
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public Int64 Type { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class MedicalOrder
    {
        public Int64 Id { get; set; }
        public Int64 PatientId { get; set; }
        public Int64 TotalQuantity { get; set; }
        public string TotalInterval { get; set; }
        public Int64 HdfQuantity { get; set; }
        public string HdfInterval { get; set; }
        public Int64 HdhpQuantity { get; set; }
        public string HdhpInterval { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
    }

    public class Patient
    {
        public Int64 Id { get; set; } //TODO ?
        public string PatientId { get; set; }
        public string Name { get; set; }
        public string Dob { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public string Order { get; set; }
        public string Status { get; set; }
        public string RegisitDate { get; set; }
        public Int64 InfectType { get; set; }
        public bool IsFixedBed { get; set; }
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
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class PatientDepartment
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
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
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thu { get; set; }
        public string Fri { get; set; }
        public string Sta { get; set; }
        public string Sun { get; set; }
        public string NMon { get; set; }
        public string NTue { get; set; }
        public string NWed { get; set; }
        public string NThu { get; set; }
        public string NFri { get; set; }
        public string NSta { get; set; }
        public string NSun { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
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
        public string Type { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class TreatType
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    public class TreatStatus
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reserved { get; set; }
    }

    //public enum SeriesType
    //{
    //    AnatomyImage = 0,
    //    FuctionImage = 1,
    //    MergeImage
    //}

}