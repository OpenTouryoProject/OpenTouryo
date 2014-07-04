-- DaoT_Workflow_S1_Insert
-- 2014/7/4 日立 太郎
INSERT INTO 
  [T_Workflow]
    (
      [WorkflowControlNo],
      [SubSystemId],
      [WorkflowName],
      [UserId],
      [UserInfo],
      [ReserveArea],
      [CreatedDate],
      [EndDate]
    )
VALUES
    (
      @WorkflowControlNo,
      @SubSystemId,
      @WorkflowName,
      @UserId,
      @UserInfo,
      @ReserveArea,
      @CreatedDate,
      @EndDate
    )
