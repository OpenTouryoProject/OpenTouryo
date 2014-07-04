-- DaoM_Workflow_S1_Insert
-- 2014/7/4 日立 太郎
INSERT INTO 
  [M_Workflow]
    (
      [Id],
      [SubSystemId],
      [WorkflowName],
      [WfPositionId],
      [FromUserId],
      [ActionType],
      [ToUserId],
      [ToUserPositionTitlesId],
      [SortIndex],
      [NextWfPositionId],
      [MailTemplateId],
      [ReserveArea]
    )
VALUES
    (
      @Id,
      @SubSystemId,
      @WorkflowName,
      @WfPositionId,
      @FromUserId,
      @ActionType,
      @ToUserId,
      @ToUserPositionTitlesId,
      @SortIndex,
      @NextWfPositionId,
      @MailTemplateId,
      @ReserveArea
    )
