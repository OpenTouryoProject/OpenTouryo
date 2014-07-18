--PreStart
select
  *
from
  [dbo].[M_Workflow]
Where 
  SubSystemId = 'OEQT'
  and WorkflowName = 'Quote'
  and WorkflowNo = 1
  and FromUserId IN (1999, 1)