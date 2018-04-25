SELECT
 [T_WorkflowHistory].[FromUserId],  
  [T_WorkflowHistory].[WorkflowNo],
  [T_WorkflowHistory].[WfPositionId],
  [T_WorkflowHistory].[NextWfPositionId],
  [T_WorkflowHistory].[ReserveArea],
  [T_WorkflowHistory].[ToUserPositionTitlesId],
  [T_WorkflowHistory].[NextWorkflowNo]
FROM
  [T_WorkflowHistory]
WHERE
  [T_WorkflowHistory].[WorkflowControlNo] = @WorkflowControlNo
  AND [T_WorkflowHistory].[ActionType] != @ActionType
  AND [T_WorkflowHistory].[HistoryNo] = 1