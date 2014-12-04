/*********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[my_table]') AND type in (N'U'))
DROP TABLE [dbo].[my_table]

/*********************************************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[my_table](
	[ID] [int] NOT NULL,
	[ID2] [int] NOT NULL,
	[Val] [nvarchar](max) NOT NULL,
	[Timestamp] [timestamp] NULL,
CONSTRAINT [PK_my_table] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[ID2] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/*********************************************************************************/
INSERT INTO [dbo].[my_table] ([ID], [ID2], [Val]) values(1, 1, 'AAAA');
INSERT INTO [dbo].[my_table] ([ID], [ID2], [Val]) values(1, 2, 'BBBB');
INSERT INTO [dbo].[my_table] ([ID], [ID2], [Val]) values(2, 1, 'CCCC');
INSERT INTO [dbo].[my_table] ([ID], [ID2], [Val]) values(2, 2, 'DDDD');
