-- DaoM_Workflow_S1_Insert
-- 2014/7/18 日立 太郎
INSERT INTO 
  [M_Workflow]
    (
      [Id],
      [SubSystemId],
      [WorkflowName],
      [WfPositionId],
      [WorkflowNo],
      [FromUserId],
      [ActionType],
      [ToUserId],
      [ToUserPositionTitlesId],
      [NextWfPositionId],
      [NextWorkflowNo],
      [CorrespondOfReplyWorkflow],
      [MailTemplateId],
      [ReserveArea]
    )
VALUES
    (
      @Id,
      @SubSystemId,
      @WorkflowName,
      @WfPositionId,
      @WorkflowNo,
      @FromUserId,
      @ActionType,
      @ToUserId,
      @ToUserPositionTitlesId,
      @NextWfPositionId,
      @NextWorkflowNo,
      @CorrespondOfReplyWorkflow,
      @MailTemplateId,
      @ReserveArea
    )
