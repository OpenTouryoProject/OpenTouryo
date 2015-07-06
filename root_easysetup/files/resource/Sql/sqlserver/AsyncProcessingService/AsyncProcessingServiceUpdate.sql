
UPDATE
  [AsyncProcessingServiceStatusManagementTable]
SET
      
      [RegistrationDateTime]=@P3,
      [ExecutionStartDateTime]=@P4,
      [NumberOfRetries]=@P5,
      [CompletionDateTime]=@P6,
      [ProgressRate]=@P7,
      [StatusId]=@P8,
      [CommandId]=@P9

WHERE

  [UserId]= @P2 
