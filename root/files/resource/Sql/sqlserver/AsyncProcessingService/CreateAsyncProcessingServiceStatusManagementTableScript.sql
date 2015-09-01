USE [AsyncProcessingServiceDB]
GO

/****** Object:  Table [dbo].[AsyncProcessingServiceStatusManagementTable]    Script Date: 01/05/2015 12:10:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AsyncProcessingServiceStatusManagementTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](50) NULL,
	[ProcessName] [nvarchar](50) NULL,
	[Data] [nvarchar](max) NULL,
	[RegistrationDateTime] [datetime] NULL,
	[ExecutionStartDateTime] [datetime] NULL,
	[NumberOfRetries] [int] NULL,
	[CompletionDateTime] [datetime] NULL,
	[ProgressRate] [decimal](6, 2) NULL,
	[ReservedArea] [nvarchar](512) NULL,
	[StatusId] [int] NULL,
	[CommandId] [int] NULL,
	[ExceptionInfo] [nvarchar](512) NULL,
 CONSTRAINT [PK_AsyncProcessingServiceStatusManagementTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

