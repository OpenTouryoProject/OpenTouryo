INSERT INTO
  [dbo].[T_WorkflowHistory]
  (
    [WorkflowControlNo]
    ,[HistoryNo]
    ,[WfPositionId]
    ,[WorkflowNo]
    ,[FromUserId]
    ,[FromUserInfo]
    ,[ActionType]
    ,[ToUserId]
    ,[ToUserInfo]
    ,[ToUserPositionTitlesId]
    ,[NextWfPositionId]
    ,[NextWorkflowNo]
    ,[ReserveArea]
    ,[ReplyDeadline]
    ,[StartDate]
    ,[AcceptanceDate]
    ,[AcceptanceUserId]
    ,[AcceptanceUserInfo]
    ,[EndDate])

  SELECT
     [WorkflowControlNo]
    ,[HistoryNo]
    ,[WfPositionId]
    ,[WorkflowNo]
    ,[FromUserId]
    ,[FromUserInfo]
    ,[ActionType]
    ,[ToUserId]
    ,[ToUserInfo]
    ,[ToUserPositionTitlesId]
    ,[NextWfPositionId]
    ,[NextWorkflowNo]
    ,[ReserveArea]
    ,[ReplyDeadline]
    ,[StartDate]
    ,[AcceptanceDate]
    ,[AcceptanceUserId]
    ,[AcceptanceUserInfo]
    ,NULL 
  FROM
    [dbo].[T_CurrentWorkflow]
  WHERE
    [WorkflowControlNo] = @WorkflowControlNo

