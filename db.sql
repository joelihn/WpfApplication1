USE [db]
GO
/****** Object:  Table [dbo].[PatientArea]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatientArea](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Type] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
	[InfectTypeId] [int] NULL,
	[Seq] [int] NULL,
	[Position] [varchar](255) NULL,
 CONSTRAINT [PK__PatientArea__164452B1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__PatientArea__173876EA] UNIQUE NONCLUSTERED 
(
	[Seq] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__PatientArea__182C9B23] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Bed]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Bed](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[TreatTypeId] [int] NULL,
	[IsAvailable] [bit] NULL,
	[IsOccupy] [bit] NULL,
	[Description] [varchar](255) NULL,
	[BedId] [int] NULL,
	[IsTemp] [bit] NULL,
	[PatientAreaId] [int] NULL,
	[MachineTypeId] [int] NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__Bed__014935CB] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__Bed__023D5A04] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InfectType]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InfectType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__InfectType__1DE57479] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__InfectType__1ED998B2] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MachineType]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MachineType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
	[BgColor] [varchar](255) NULL,
 CONSTRAINT [PK__MachineType__0F975522] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MedicalOrderPara]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MedicalOrderPara](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Type] [varchar](255) NULL,
	[Count] [int] NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__MedicalOrderPara__117F9D94] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__MedicalOrderPara__1273C1CD] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MedicalOrder]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MedicalOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PatientId] [int] NULL,
	[Activated] [bit] NULL,
	[Seq] [varchar](255) NULL,
	[Plan] [varchar](255) NULL,
	[MethodId] [int] NULL,
	[Interval] [int] NULL,
	[Times] [int] NULL,
	[Description] [varchar](255) NULL,
	[Reserved1] [varchar](255) NULL,
	[Reserved2] [varchar](255) NULL,
 CONSTRAINT [PK__MedicalOrder__7C8480AE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Patient]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Patient](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PatientId] [varchar](255) NULL,
	[Name] [varchar](100) NOT NULL,
	[DOB] [varchar](100) NOT NULL,
	[Gender] [varchar](100) NOT NULL,
	[Nationality] [varchar](255) NULL,
	[Marriage] [varchar](100) NULL,
	[Height] [varchar](100) NULL,
	[BloodType] [varchar](100) NULL,
	[InfectTypeId] [int] NOT NULL,
	[TreatStatusId] [int] NOT NULL,
	[IsFixedBed] [bit] NOT NULL,
	[IdCode] [varchar](255) NULL,
	[AreaId] [int] NULL,
	[Mobile] [varchar](255) NULL,
	[ZipCode] [varchar](100) NULL,
	[Weixinhao] [varchar](100) NULL,
	[Payment] [varchar](100) NULL,
	[Orders] [varchar](255) NULL,
	[RegisitDate] [varchar](100) NULL,
	[BedId] [int] NULL,
	[IsAssigned] [bit] NULL,
	[Description] [varchar](255) NULL,
	[Reserved1] [varchar](255) NULL,
	[Reserved2] [varchar](255) NULL,
 CONSTRAINT [PK__Patient__2A4B4B5E] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatientDepartment]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatientDepartment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__PatientDepartmen__0425A276] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatientGroup]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatientGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__PatientGroup__1A14E395] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatientGroupPara]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatientGroupPara](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GroupId] [int] NULL,
	[Left] [varchar](100) NULL,
	[Key] [varchar](255) NULL,
	[Symbol] [varchar](100) NULL,
	[Value] [varchar](255) NULL,
	[Right] [varchar](100) NULL,
	[Logic] [varchar](100) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__PatientGroupPara__0DAF0CB0] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PatientRoom]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PatientRoom](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PatientAreaId] [int] NULL,
	[Name] [varchar](255) NULL,
	[InfectTypeId] [int] NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__PatientRoom__0AD2A005] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__PatientRoom__0BC6C43E] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ScheduleTemplate]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ScheduleTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PatientId] [int] NULL,
	[Date] [varchar](255) NULL,
	[AmPmE] [varchar](255) NULL,
	[Method] [varchar](255) NULL,
	[BedId] [int] NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
	[IsTemp] [bit] NULL,
	[IsAuto] [bit] NULL,
 CONSTRAINT [PK__ScheduleTemplate__7E6CC920] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ScheduleType]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ScheduleType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[PatientId] [int] NULL,
	[TimeRange] [varchar](255) NULL,
	[Type] [int] NULL,
	[Color] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__ScheduleType__1BFD2C07] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TreatMethod]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TreatMethod](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[IsAvailable] [bit] NULL,
	[DoublePump] [bit] NULL,
	[SinglePump] [bit] NULL,
	[BgColor] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__TreatMethod__07F6335A] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ__TreatMethod__08EA5793] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TreatStatus]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TreatStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
	[Activated] [bit] NULL,
 CONSTRAINT [PK__TreatStatus__060DEAE8] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TreatTime]    Script Date: 11/08/2015 23:31:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TreatTime](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Activated] [bit] NULL,
	[Name] [varchar](255) NULL,
	[BeginTime] [varchar](255) NULL,
	[EndTime] [varchar](255) NULL,
	[Description] [varchar](255) NULL,
	[Reserved] [varchar](255) NULL,
 CONSTRAINT [PK__TreatTime__145C0A3F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF__Patient__BedId__2B3F6F97]    Script Date: 11/08/2015 23:31:00 ******/
ALTER TABLE [dbo].[Patient] ADD  CONSTRAINT [DF__Patient__BedId__2B3F6F97]  DEFAULT ((-1)) FOR [BedId]
GO
/****** Object:  Default [DF__ScheduleT__BedId__7F60ED59]    Script Date: 11/08/2015 23:31:00 ******/
ALTER TABLE [dbo].[ScheduleTemplate] ADD  CONSTRAINT [DF__ScheduleT__BedId__7F60ED59]  DEFAULT ((-1)) FOR [BedId]
GO
