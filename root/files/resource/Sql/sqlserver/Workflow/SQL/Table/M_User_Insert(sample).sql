USE [Workflow]
GO

INSERT INTO [dbo].[M_User]([Id], [Section], [Name], [PositionTitlesId])
    VALUES
	(1   , 'AAAA' , 'A�䒆'   , 1 ),
	(1000, 'AAAA' , 'AAAA0'   , 1 ),
	(2   , 'BBBB' , 'B�䒆'   , 1 ),
	(2000, 'BBBB' , 'BBBB0'   , 1 ),
	(2999, 'BBBB' , 'BBBB1'   , 1 ),
	(2998, 'BBBB' , 'BBBB2'   , 1 ),
	(2997, 'BBBB' , 'BBBB3'   , 1 ),
	(3   , 'CCCC' , 'C�䒆'   , 1 ),
	(3000, 'CCCC' , 'CCCC0'   , 1 ),
	(4   , 'DDDD' , 'D�䒆'   , 1 ),
	(4000, 'DDDD' , 'DDDD0'   , 1 ),
	(4999, 'DDDD' , 'DDDD1'   , 1 ),
	(5   , 'EEEE' , 'E�䒆'   , 1 ),
	(5000, 'EEEE' , 'EEEE0'   , 1 ),
	(5999, 'EEEE' , 'EEEE1'   , 1 ),
	(6   , 'FFFF' , 'F�䒆'   , 1 ),
	(6000, 'FFFF' , 'FFFF0'   , 1 ),
	(6999, 'FFFF' , 'FFFF1'   , 1 ),
	(6998, 'FFFF' , 'FFFF2'   , 1 ),
	(6997, 'FFFF' , 'FFFF3'   , 1 )
GO
