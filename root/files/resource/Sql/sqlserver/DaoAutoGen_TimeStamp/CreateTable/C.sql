USE [Northwind]
GO

/****** Object:  Table [dbo].[ts_test_tableC]    Script Date: 03/19/2010 19:19:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ts_test_tableC](
	[ts] [timestamp] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[val] [nvarchar](50) NULL,
 CONSTRAINT [PK_ts_test_tableC] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

