SELECT
  COUNT(*)
FROM
  [dbo].[T_WorkflowHistory]
WHERE
  [WorkflowControlNo] = @WorkflowControlNo
