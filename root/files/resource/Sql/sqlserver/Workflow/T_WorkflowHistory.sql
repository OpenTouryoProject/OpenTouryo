USE [Workflow]
GO

/****** Object:  Table [dbo].[T_WorkflowHistory]    Script Date: 2014/07/03 14:08:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[T_WorkflowHistory](
	[WorkflowControlNo] [varchar] (99) NOT NULL ,
	[HistoryNo] [int] NOT NULL ,
	[WfPositionId] [int] NOT NULL ,
	[FromUserId] [decimal] (10,0) NULL ,
	[FromUserInfo] [nvarchar] (30) NULL ,
	[ActionType] [varchar] (30) NOT NULL ,
	[ToUserId] [decimal] (10,0) NULL ,
	[ToUserInfo] [nvarchar] (30) NULL ,
	[NextWfPositionId] [int] NULL ,
	[ReserveArea] [varchar](50) NULL,
	[ReplyDeadline] [datetime] NULL ,
	[StartDate] [datetime] NOT NULL ,
	[AcceptanceDate] [datetime] NULL ,
	[EndDate] [datetime] NOT NULL

 CONSTRAINT [PK_I_WorkflowHistory] PRIMARY KEY CLUSTERED 
(
	[WorkflowControlNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


