USE [AsyncProcessingServiceDB]
GO

/****** Object:  Table [dbo].[AsyncProcessingServiceStatusManagementTable]    Script Date: 01/05/2015 12:10:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AsyncProcessingServiceStatusManagementTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](5) NULL,
	[ProcessName] [nvarchar](50) NULL,
	[Data] [nvarchar](50) NOT NULL,
	[RegistrationDateTime] [datetime] NULL,
	[ExecutionStartDateTime] [datetime] NULL,
	[NumberOfRetries] [int] NULL,
	[CompletionDateTime] [datetime] NULL,
	[ProgressRate] [int] NULL,
	[ReservedArea] [nvarchar](100) NULL,
	[StatusId] [int] NULL,
	[CommandId] [int] NULL,
 CONSTRAINT [PK_AsyncProcessingServiceStatusManagementTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

