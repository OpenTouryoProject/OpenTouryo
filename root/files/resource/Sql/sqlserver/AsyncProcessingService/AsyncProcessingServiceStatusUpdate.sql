-- AsyncProcessingServiceStatusUpdate
-- 2015/02/25 
UPDATE
  [AsyncProcessingServiceStatusManagementTable]
SET   
      [ProgressRate]=@P2,
      [StatusId]=@P3,
      [NumberOfRetries]=@p4

WHERE

      [UserId]= @P1 
