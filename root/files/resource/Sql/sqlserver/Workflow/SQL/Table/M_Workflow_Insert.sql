USE [Workflow]
GO

INSERT INTO [dbo].[M_Workflow](
     [Id], [SubSystemId], [WorkflowName],
     [WfPositionId], [WorkflowNo], [FromUserId],
     [ActionType], [ToUserId], [ToUserPositionTitlesId], [SortIndex], 
     [NextWfPositionId], [NextWorkflowNo], [MailTemplateId], [ReserveArea])
     VALUES
         ( 1,  'OEQT', 'Quote', 100, 1  , 1   , 'Request' , 2   , 1   , 1, 200,  2   , 1, ''),
         ( 2,  'OEQT', 'Quote', 100, 1  , 2   , 'Request' , 2   , 1   , 1, 200,  2   , 1, ''),
         ( 3,  'OEQT', 'Quote', 200, 2  , 2   , 'TurnBack', 1   , 1   , 1, 100,  1   , 1, ''),
         ( 4,  'OEQT', 'Quote', 200, 2  , 2   , 'RFQ'     , 3   , 1   , 1, 300,  3   , 1, ''),
         ( 5,  'OEQT', 'Quote', 300, 3  , 3   , 'TurnBack', 2   , 1   , 1, 200,  2   , 1, ''),
         ( 6,  'OEQT', 'Quote', 300, 3  , 3   , 'RFQ'     , 4999, null, 1, 400,  4   , 1, ''),
         ( 7,  'OEQT', 'Quote', 400, 4  , 4999, 'TurnBack', 3   , 1   , 1, 300,  3   , 1, ''),
         ( 8,  'OEQT', 'Quote', 400, 4  , 4999, 'Reply'   , 5999, null, 1, 500,  5   , 1, ''),
         ( 9,  'OEQT', 'Quote', 500, 5  , 5999, 'TurnBack', 4999, null, 1, 400,  4   , 1, ''),
         ( 10, 'OEQT', 'Quote', 500, 5  , 5999, 'Reply'   , 3   , 1   , 1, 300,  6   , 1, ''),
         ( 11, 'OEQT', 'Quote', 300, 6  , 3   , 'Reply'   , 2   , 1   , 1, 200,  7   , 1, ''),
         ( 12, 'OEQT', 'Quote', 200, 7  , 2   , 'RFQ'     , 6999, null, 1, 600,  801 , 1, ''),
         ( 13, 'OEQT', 'Quote', 600, 801, 6999, 'TurnBack', 2   , 1   , 1, 200,  7   , 1, ''),
         ( 14, 'OEQT', 'Quote', 600, 801, 6999, 'Reply'   , 6998, null, 1, 600,  802 , 1, ''),
         ( 15, 'OEQT', 'Quote', 600, 802, 6998, 'TurnBack', 6999, null, 1, 600,  801 , 1, ''),
         ( 16, 'OEQT', 'Quote', 600, 802, 6998, 'Reply'   , 6997, null, 1, 600,  803 , 1, ''),
         ( 17, 'OEQT', 'Quote', 600, 803, 6997, 'TurnBack', 6999, null, 1, 600,  802 , 1, ''),
         ( 18, 'OEQT', 'Quote', 600, 803, 6997, 'Reply'   , 2999, null, 1, 200,  901 , 1, ''),
         ( 19, 'OEQT', 'Quote', 200, 901, 2999, 'Reply'   , 2998, null, 1, 200,  902 , 1, ''),
         ( 20, 'OEQT', 'Quote', 200, 902, 2998, 'Reply'   , 2997, null, 1, 200,  903 , 1, ''),
         ( 21, 'OEQT', 'Quote', 200, 903, 2997, 'Reply'   , 1   , 1   , 1, 100,  10  , 1, ''),
         ( 22, 'OEQT', 'Quote', 100, 10 , 1   , 'End'     , null, null, 1, null, null, 1, '')

            -- 200, 300 への遷移時は[NextWorkflowNo]が必要になる。 [ToUserId]で一意になれば、不要。

GO


