USE [WinServiceDB]
GO

/****** Object:  Table [dbo].[SampleWinServices]    Script Date: 11/25/2014 19:16:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SampleWinServices](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](5) NULL,
	[ProcessName] [nvarchar](50) NULL,
	[Data] [nvarchar](50) NOT NULL,
	[RegistrationDateTime] [datetime] NULL,
	[ExecutionStartDateTime] [datetime] NULL,
	[NumberOfRetries] [int] NULL,
	[CompletionDateTime] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[ProgressRate] [int] NULL,
	[Command] [nvarchar](50) NULL,
	[ReservedArea] [nvarchar](100) NULL,
 CONSTRAINT [PK_SampleWinServices] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

