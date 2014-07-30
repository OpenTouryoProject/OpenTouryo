SELECT
  [T_WorkflowHistory].[WorkflowNo]
FROM
  [T_WorkflowHistory]
WHERE
  [T_WorkflowHistory].[WorkflowControlNo] = @WorkflowControlNo
  AND [T_WorkflowHistory].[HistoryNo] = (
    SELECT
      MAX([T_WorkflowHistory].[HistoryNo])
    FROM
      [T_WorkflowHistory]
    WHERE
      [T_WorkflowHistory].[WorkflowControlNo] = @WorkflowControlNo
  )