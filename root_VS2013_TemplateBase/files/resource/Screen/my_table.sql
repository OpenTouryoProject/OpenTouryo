

/****** Object:  Table [dbo].[my_table]    Script Date: 03/12/2014 11:37:44 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[my_table]') AND type in (N'U'))
DROP TABLE [dbo].[my_table]


/****** Object:  Table [dbo].[my_table]    Script Date: 03/12/2014 11:37:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[my_table](
	[columna] [int] NOT NULL,
	[columnb] [int] NOT NULL,
	[columnc] [nvarchar](max) NOT NULL,
	[columnd] [timestamp] NULL,
PRIMARY KEY CLUSTERED 
(
	[columna] ASC,
	[columnb] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


