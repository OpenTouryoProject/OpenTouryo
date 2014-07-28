SELECT 
  
  W.[Id],
  W.[SubSystemId],
  W.[WorkflowName],
  W.[WfPositionId],
  W.[WorkflowNo],
  FU.[Section] as FromSection,
  FU.[Name] as FromUser,
  W.[ActionType],
  TU.[Section] as ToSection,
  TU.[Name] as ToUser,
  W.[ToUserPositionTitlesId],
  W.[SortIndex], 
  W.[NextWfPositionId],
  W.[NextWorkflowNo],
  W.[MailTemplateId],
  W.[ReserveArea]

FROM
  [dbo].[M_Workflow] as W
  LEFT OUTER JOIN [dbo].[M_User] as FU ON W.[FromUserId] = FU.[Id]
  LEFT OUTER JOIN [dbo].[M_User] as TU ON W.[ToUserId] = TU.[Id]

ORDER BY W.[Id]