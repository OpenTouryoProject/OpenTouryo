USE [Workflow]
GO

/****** Object:  Table [dbo].[T_Workflow]    Script Date: 2014/07/03 14:08:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[T_Workflow](
	[WorkflowControlNo] [varchar](50) NOT NULL,
	[SubSystemId] [char](4) NOT NULL,
	[WorkflowName] [varchar](50) NOT NULL,
	[UserId] [decimal](10, 0) NULL,
	[UserInfo] [nvarchar](50) NOT NULL,
	[ReserveArea] [varchar](50) NULL,
	[CreatedDate] [datetime] NOT NULL ,
	[EndDate] [datetime] NULL

 CONSTRAINT [PK_I_Workflow] PRIMARY KEY CLUSTERED 
(
	[WorkflowControlNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


