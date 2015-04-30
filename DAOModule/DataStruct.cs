#region File Header Text

#endregion

#region Using References

#endregion

using System;

namespace WpfApplication1.DAOModule
{
    /// <summary>
    /// 患者表
    /// </summary>
    public class FmriPatient
    {
        public Int64 PatientId { get; set; } //TODO ?
        public string PatientName { get; set; }
        public string PatientDob { get; set; }
        public string PatientGender { get; set; }
        public string PatientHand { get; set; }
        public string PatientRegesiterDate { get; set; }
        public string PatientStudyDescription { get; set; }
        public string PatientClinicDescription { get; set; }
    }


    public enum SeriesType
    {
        AnatomyImage = 0,
        FuctionImage = 1,
        MergeImage
    }

    /// <summary>
    /// 序列表
    /// </summary>
    public class FMriSeries
    {
        public Int64 SeriesId { get; set; }
        public Int64 SeriesPatientId { get; set; }
        public string SeriesWw { get; set; }
        public string SeriesWl { get; set; }
        public string SeriesProtocolName { get; set; }
        public string SeriesUId { get; set; }
        public string SeriesTrValue { get; set; }
        public string SeriesDescription { get; set; }
        public string SeriesDateTime { get; set; }
        public string SeriesImageDir { get; set; }
        public SeriesType SeriesType { get; set; }
        public Int64 SeriesImageCount { get; set; }
        public bool SeriesProcessed { get; set; }
    }

    /// <summary>
    /// 处理记录表
    /// </summary>
    public class FMriProcess
    {
        public Int64 ProcessId { get; set; }
        public Int64 ProcessSeriesId { get; set; }
        public Int64 ProcessStimulusId { get; set; }
        public string ProcessCorrespondingAnatomicImages { get; set; }
        public Int64 ProcessStimulusTimes { get; set; }
        public string ProcessStimulusName { get; set; }
        public string ProcessAlgorithms { get; set; }
        public string ProcessAlgorithmsParameters { get; set; }
        public string ProcessResultImages { get; set; }
        public string ProcessResultAdjustImages { get; set; }
        public string ProcessHeadMotionFile { get; set; }
        public string ProcessDateTime { get; set; }
    }

    /// <summary>
    /// 刺激记录表
    /// </summary>
    public class FMriStimulus
    {
        public Int64 StimulusId { get; set; }
        public Int64 StimulusPatientId { get; set; }
        public string StimulusName { get; set; }
        public string StimulusDateTime { get; set; }
        public Int64 StimulusTimes { get; set; }
        public string StimulusDescription { get; set; }
    }

    /// <summary>
    /// 报告记录表
    /// </summary>
    public class FMriReport
    {
        public Int64 ReportId { get; set; }
        public Int64 ReportPatientId { get; set; }
        public string ReportDoc { get; set; }
        public string ReportDate { get; set; }
    }
}