-- DaoT_WorkflowHistory_S1_Insert
-- 2014/7/4 日立 太郎
INSERT INTO 
  [T_WorkflowHistory]
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
      [ReplyDeadline],
      [StartDate],
      [AcceptanceDate],
      [EndDate]
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
      @ReplyDeadline,
      @StartDate,
      @AcceptanceDate,
      @EndDate
    )
