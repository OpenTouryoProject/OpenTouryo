USE [Workflow]
GO

SELECT 
  
  WM.[Id],
  WM.[SubSystemId],
  WM.[WorkflowName],
  WM.[WfPositionId],
  WM.[WorkflowNo],
  FU.[Section] as FromSection,
  FU.[Name] as FromUser,
  WM.[ActionType],
  TU.[Section] as ToSection,
  TU.[Name] as ToUser,
  WM.[ToUserPositionTitlesId], 
  WM.[NextWfPositionId],
  WM.[NextWorkflowNo],
  WM.[CorrespondOfReplyWorkflow],
  WM.[MailTemplateId],
  WM.[ReserveArea]

FROM
  [dbo].[M_Workflow] as WM
  LEFT OUTER JOIN [dbo].[M_User] as FU ON WM.[FromUserId] = FU.[Id]
  LEFT OUTER JOIN [dbo].[M_User] as TU ON WM.[ToUserId] = TU.[Id]

ORDER BY WM.[Id]