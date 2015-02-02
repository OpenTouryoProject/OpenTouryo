USE [AsyncProcessingServiceDB]
GO

/****** Object:  Table [dbo].[EnumAsyncCommandTable]    Script Date: 12/04/2014 15:40:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EnumAsyncCommandTable](
	[Id] [int] NOT NULL,
	[Category] [nvarchar](50) NULL,
	[String] [nvarchar](50) NULL,
 CONSTRAINT [PK_EnumAsyncCommandTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

