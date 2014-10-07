USE [Workflow]
GO

/****** Object:  Table [dbo].[T_WorkflowHistory]    Script Date: 2014/07/03 14:08:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

DROP TABLE [dbo].[T_WorkflowHistory]
GO

CREATE TABLE [dbo].[T_WorkflowHistory](
	[WorkflowControlNo] [nvarchar] (50) NOT NULL ,
	[HistoryNo] [int] NOT NULL ,
	[WfPositionId] [nvarchar](10) NOT NULL ,
	[WorkflowNo] [int] NULL ,
	[FromUserId] [decimal] (10,0) NULL ,
	[FromUserInfo] [nvarchar] (30) NULL ,
	[ActionType] [nvarchar] (30) NOT NULL ,
	[ToUserId] [decimal] (10,0) NULL ,
	[ToUserInfo] [nvarchar] (30) NULL ,
	[ToUserPositionTitlesId] [int] NULL ,
	[NextWfPositionId] [nvarchar](10) NULL ,
	[NextWorkflowNo] [int] NULL ,
	[ReserveArea] [nvarchar](50) NULL,
	[ReplyDeadline] [datetime] NULL ,
	[StartDate] [datetime] NOT NULL ,
	[AcceptanceDate] [datetime] NULL ,
	[AcceptanceUserId] [decimal] (10,0) NULL ,
	[AcceptanceUserInfo] [nvarchar] (30) NULL ,
	[EndDate] [datetime] NULL

 CONSTRAINT [PK_I_WorkflowHistory] PRIMARY KEY CLUSTERED 
(
	[WorkflowControlNo] ASC,
	[HistoryNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


