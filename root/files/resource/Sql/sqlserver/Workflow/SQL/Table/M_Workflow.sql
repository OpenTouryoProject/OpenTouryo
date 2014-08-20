USE [Workflow]
GO

/****** Object:  Table [dbo].[M_Workflow]    Script Date: 2014/07/03 14:08:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

DROP TABLE [dbo].[M_Workflow]
GO

CREATE TABLE [dbo].[M_Workflow](
	[Id] [decimal](10, 0) IDENTITY (1, 1) NOT NULL,
	[SubSystemId] [char](4) NOT NULL,
	[WorkflowName] [nvarchar](10) NOT NULL,
	[WfPositionId] [nvarchar](10) NOT NULL,
	[WorkflowNo] [int] NULL ,
	[FromUserId] [decimal](10, 0) NULL,
	[ActionType] [nvarchar](30) NOT NULL,
	[ToUserId] [decimal](10, 0) NULL,
	[ToUserPositionTitlesId] [int] NULL,
	[NextWfPositionId] [nvarchar](10) NULL,
	[NextWorkflowNo] [int] NULL ,
	[CorrespondOfReplyWorkflow] [int] NULL ,
	[MailTemplateId] [int] NULL,
	[ReserveArea] [nvarchar](50) NULL,
 CONSTRAINT [PK_M_Workflow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

USE [Workflow]
GO

/****** Object:  Index [IX_M_Workflow]    Script Date: 2014/07/03 14:08:46 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_M_Workflow] ON [dbo].[M_Workflow]
(
	[SubSystemId] ASC,
	[WorkflowName] ASC,
	[WfPositionId] ASC,
	[FromUserId] ASC,
	[ActionType] ASC,
	[ToUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


