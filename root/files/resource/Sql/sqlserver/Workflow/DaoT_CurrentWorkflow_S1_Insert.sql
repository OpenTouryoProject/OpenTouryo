-- DaoT_CurrentWorkflow_S1_Insert
-- 2014/7/4 日立 太郎
INSERT INTO 
  [T_CurrentWorkflow]
    (
      [WorkflowControlNo],
      [HistoryNo],
      [WfPositionId],
      [FromUserId],
      [FromUserInfo],
      [ActionType],
      [ToUserId],
      [ToUserInfo],
      [NextWfPositionId],
      [ReserveArea],
      [ExclusiveKey],
      [ReplyDeadline],
      [CreatedDate],
      [AcceptanceDate]
    )
VALUES
    (
      @WorkflowControlNo,
      @HistoryNo,
      @WfPositionId,
      @FromUserId,
      @FromUserInfo,
      @ActionType,
      @ToUserId,
      @ToUserInfo,
      @NextWfPositionId,
      @ReserveArea,
      @ExclusiveKey,
      @ReplyDeadline,
      @CreatedDate,
      @AcceptanceDate
    )
