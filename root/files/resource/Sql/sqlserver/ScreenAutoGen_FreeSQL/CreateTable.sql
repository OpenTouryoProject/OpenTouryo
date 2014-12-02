/*********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TABL1]') AND type in (N'U'))
DROP TABLE [dbo].[TABL1]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TABL2]') AND type in (N'U'))
DROP TABLE [dbo].[TABL2]

/*********************************************************************************/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

/*********************************************************************************/
CREATE TABLE [dbo].[TABL2](
	[A] [int] NOT NULL,
	[Y] [varchar](50) NULL,
	[Z] [varchar](50) NULL,
 CONSTRAINT [PK_TABL2] PRIMARY KEY CLUSTERED 
(
	[A] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[TABL1](
	[A] [int] NOT NULL,
	[B] [varchar](50) NULL,
	[C] [varchar](50) NULL,
	[AX] [int] NOT NULL,
 CONSTRAINT [PK_TABL1] PRIMARY KEY CLUSTERED 
(
	[A] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/*********************************************************************************/
SET ANSI_PADDING OFF
GO

/*********************************************************************************/

ALTER TABLE [dbo].[TABL1]  WITH CHECK ADD  CONSTRAINT [FK_TABL1_TABL2] FOREIGN KEY([AX])
REFERENCES [dbo].[TABL2] ([A])
GO

ALTER TABLE [dbo].[TABL1] CHECK CONSTRAINT [FK_TABL1_TABL2]
GO

/*********************************************************************************/

INSERT INTO TABL2 values(1,'Insert_Y','Insert_Z')
INSERT INTO TABL2 values(2,'Insert_Y2','Insert_Z2')
INSERT INTO TABL2 values(3,'Insert_Y3','Insert_Z3')
INSERT INTO TABL2 values(4,'Insert_Y4','Insert_Z4')

INSERT INTO TABL1 values(1,'Insert_B','Insert_C',1)
INSERT INTO TABL1 values(2,'Insert_B2','Insert_C2',2)
INSERT INTO TABL1 values(3,'Insert_B3','Insert_C3',3)
INSERT INTO TABL1 values(4,'Insert_B4','Insert_C4',4)