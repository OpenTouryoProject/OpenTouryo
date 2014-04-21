USE [Northwind]
GO

/****** Object:  Table [dbo].[ts_test_tableB]    Script Date: 03/19/2010 19:19:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ts_test_tableB](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ts] [timestamp] NOT NULL,
	[val] [nvarchar](50) NULL,
 CONSTRAINT [PK_ts_test_tableB] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

