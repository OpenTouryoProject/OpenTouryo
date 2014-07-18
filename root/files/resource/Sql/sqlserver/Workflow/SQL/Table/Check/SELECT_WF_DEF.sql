select 
  
  W.[Id], W.[SubSystemId], W.[WorkflowName],
  W.[WfPositionId], W.[WorkflowNo], FU.[Section] as FromSection, FU.[Name] as FromUser,
  W.[ActionType], TU.[Section] as ToSection, TU.[Name] as ToUser, W.[ToUserPositionTitlesId], W.[SortIndex], 
  W.[NextWfPositionId], W.[NextWorkflowNo], W.[MailTemplateId], W.[ReserveArea]

from
  [dbo].[M_Workflow] as W, [dbo].[M_User] as FU, [dbo].[M_User] as TU

where
  W.[FromUserId] = FU.[Id] and W.[ToUserId] = TU.[Id]
  or W.[FromUserId] = null or W.[ToUserId] = null

order by W.[Id]