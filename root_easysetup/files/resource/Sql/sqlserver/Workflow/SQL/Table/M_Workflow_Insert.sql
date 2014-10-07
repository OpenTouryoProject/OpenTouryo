USE [Workflow]
GO

INSERT INTO [dbo].[M_Workflow](
     [SubSystemId], [WorkflowName],
     [WfPositionId], [WorkflowNo], [FromUserId], [ActionType], [ToUserId], [ToUserPositionTitlesId],
     [NextWfPositionId], [NextWorkflowNo], [CorrespondOfReplyWorkflow], [MailTemplateId], [ReserveArea])
     VALUES
     
         ( 'TEST', 'Normal',  100, 1   , 1   , 'Start'     , 2    , 1    , 200  , 2    , null , 1, ''),
         ( 'TEST', 'Normal',  200, 2   , 2   , 'TurnBack'  , 1    , 1    , 100  , 1    , null , 1, ''),
         ( 'TEST', 'Normal',  200, 2   , 2   , 'Request'   , 3    , 1    , 300  , 3    , null , 1, ''),
         ( 'TEST', 'Normal',  300, 3   , 3   , 'Response'  , 2    , 1    , 200  , 4    , null , 1, ''),
         ( 'TEST', 'Normal',  200, 4   , 2   , 'Reply'     , 1    , 1    , 100  , 5    , 1    , 1, ''),
         ( 'TEST', 'Normal',  100, 5   , 1   , 'End'       , null , null , null , null , null , 1, ''),
         
         ( 'TEST', 'Branch1', 100, 1   , 1    , 'Start'    , 2    , 1    , 200  , 2    , null , 1, ''),
         ( 'TEST', 'Branch1', 200, 2   , 2    , 'Request'  , 3    , 1    , 300  , 3    , null , 1, ''),
         ( 'TEST', 'Branch1', 200, 2   , 2    , 'Request'  , 4    , 1    , 400  , 4    , null , 1, ''),
         ( 'TEST', 'Branch1', 300, 3   , 3    , 'Request'  , 5    , 1    , 500  , 5    , null , 1, ''),
         ( 'TEST', 'Branch1', 400, 4   , 4    , 'Request'  , 5    , 1    , 500  , 6    , null , 1, ''),
         ( 'TEST', 'Branch1', 500, 5   , 5    , 'Reply'    , 3    , 1    , 300  , 7    , 3    , 1, ''),
         ( 'TEST', 'Branch1', 500, 6   , 5    , 'Reply'    , 4    , 1    , 400  , 8    , 4    , 1, ''),
         ( 'TEST', 'Branch1', 300, 7   , 3    , 'Reply'    , 2    , 1    , 200  , 9    , 2    , 1, ''),
         ( 'TEST', 'Branch1', 400, 8   , 4    , 'Reply'    , 2    , 1    , 200  , 9    , 2    , 1, ''),
         ( 'TEST', 'Branch1', 200, 9   , 2    , 'Reply'    , 1    , 1    , 100  , 10   , 1    , 1, ''),
         ( 'TEST', 'Branch1', 100, 10  , 1    , 'End'      , null , null , null , null , null , 1, ''),
         
         ( 'TEST', 'Branch2', 100, 1   , 1    , 'Start'    , 2    , 1    , 200  , 2    , null , 1, ''),
         ( 'TEST', 'Branch2', 200, 2   , 2    , 'Request'  , 3    , 1    , 300  , 3    , null , 1, ''),
         ( 'TEST', 'Branch2', 200, 2   , 2    , 'Request'  , 4    , 1    , 400  , 4    , null , 1, ''),
         ( 'TEST', 'Branch2', 300, 3   , 3    , 'Request'  , 5    , 1    , 500  , 5    , null , 1, ''),
         ( 'TEST', 'Branch2', 400, 4   , 4    , 'Request'  , 5    , 1    , 500  , 5    , null , 1, ''),
         ( 'TEST', 'Branch2', 500, 5   , 5    , 'TurnBack' , 3    , 1    , 300  , 3    , null , 1, ''),
         ( 'TEST', 'Branch2', 500, 5   , 5    , 'TurnBack' , 4    , 1    , 400  , 4    , null , 1, ''),
         ( 'TEST', 'Branch2', 500, 5   , 5    , 'Request'  , 6    , 1    , 600  , 6    , null , 1, ''),
         ( 'TEST', 'Branch2', 600, 6   , 6    , 'Reply'    , 5    , 1    , 500  , 7    , 5    , 1, ''),
         ( 'TEST', 'Branch2', 500, 7   , 5    , 'Reply'    , 3    , 1    , 300  , 8    , 3    , 1, ''),
         ( 'TEST', 'Branch2', 500, 7   , 5    , 'Reply'    , 4    , 1    , 400  , 9    , 4    , 1, ''),
         ( 'TEST', 'Branch2', 300, 8   , 3    , 'Reply'    , 2    , 1    , 200  , 10   , 2    , 1, ''),
         ( 'TEST', 'Branch2', 400, 9   , 4    , 'Reply'    , 2    , 1    , 200  , 10   , 2    , 1, ''),
         ( 'TEST', 'Branch2', 200, 10  , 2    , 'Reply'    , 1    , 1    , 100  , 11   , 1    , 1, ''),
         ( 'TEST', 'Branch2', 100, 11  , 1    , 'End'      , null , null , null , null , null , 1, ''),
         
         ( 'OEQT', 'Quote'  , 100, 1   , 1    , 'Start'    , 2    , 1    , 300  , 3    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 200, 2   , 2    , 'Start'    , 2    , 1    , 300  , 3    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 300, 3   , 2    , 'TurnBack' , 1    , 1    , 100  , 1    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 300, 3   , 2    , 'TurnBack' , 2    , 1    , 200  , 2    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 300, 3   , 2    , 'RFQ'      , 3    , 1    , 400  , 4    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 400, 4   , 3    , 'TurnBack' , 2    , 1    , 300  , 3    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 400, 4   , 3    , 'RFQ'      , 4999 , null , 500  , 5    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 500, 5   , 4999 , 'TurnBack' , 3    , 1    , 400  , 4    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 500, 5   , 4999 , 'RFQ'      , 5999 , null , 600  , 6    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 600, 6   , 5999 , 'TurnBack' , 4999 , null , 500  , 5    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 600, 6   , 5999 , 'RFQ'      , 3    , 1    , 400  , 7    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 400, 7   , 3    , 'Reply'    , 2    , 1    , 300  , 8    , 3    , 1, ''),
         ( 'OEQT', 'Quote'  , 300, 8   , 2    , 'RFQ'      , 6999 , null , 700  , 91   , null , 1, ''),
         ( 'OEQT', 'Quote'  , 700, 91  , 6999 , 'TurnBack' , 2    , 1    , 300  , 8    , null , 1, ''),
         ( 'OEQT', 'Quote'  , 700, 91  , 6999  , 'RFQ'     , 6998 , null , 700  , 92   , null , 1, ''),
         ( 'OEQT', 'Quote'  , 700, 92  , 6998 , 'TurnBack' , 6999 , null , 700  , 91   , null , 1, ''),
         ( 'OEQT', 'Quote'  , 700, 92  , 6998 , 'RFQ'      , 6997 , null , 700  , 10   , null , 1, ''),
         ( 'OEQT', 'Quote'  , 700, 10  , 6997 , 'TurnBack' , 6999 , null , 700  , 92   , null , 1, ''),
         ( 'OEQT', 'Quote'  , 700, 10  , 6997 , 'Reply'    , 2    , 1    , 300  , 111  , 8    , 1, ''),
         ( 'OEQT', 'Quote'  , 300, 111 , 2    , 'RFQ'      , 2999 , null , 300  , 112  , null , 1, ''),
         ( 'OEQT', 'Quote'  , 300, 112 , 2999 , 'RFQ'      , 2998 , null , 300  , 12   , null , 1, ''),
         ( 'OEQT', 'Quote'  , 300, 12  , 2998 , 'Reply'    , 1    , 1    , 100  , 131  , 1    , 1, ''),
         ( 'OEQT', 'Quote'  , 300, 12  , 2998 , 'Reply'    , 2    , 1    , 200  , 132  , 2    , 1, ''),
         ( 'OEQT', 'Quote'  , 100, 131 , 1    , 'End'      , null , null , null , null , null , 1, ''),
         ( 'OEQT', 'Quote'  , 100, 132 , 2    , 'End'      , null , null , null , null , null , 1, '')
         
GO

