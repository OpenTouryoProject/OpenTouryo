SELECT
  [FromUserId]
FROM
  [dbo].[T_WorkflowHistory]
WHERE
  [WorkflowControlNo] = @WorkflowControlNo
  AND [HistoryNo] = (
    SELECT
      MAX(HistoryNo)
    FROM
      [dbo].[T_WorkflowHistory]
    WHERE
      [WorkflowControlNo] = @WorkflowControlNo
      AND [ActionType] != @ActionType
      AND [NextWorkflowNo] = @NextWorkflowNo
  )