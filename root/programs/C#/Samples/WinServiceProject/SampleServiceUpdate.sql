
UPDATE
  [SampleWinServices]
SET
      
      [RegistrationDateTime]=@P3,
      [ExecutionStartDateTime]=@P4,
      [NumberOfRetries]=@P5,
      [CompletionDateTime]=@P6,
[ProgressRate]=@P7,
      [Status]=@P8,
[Command]=@P9,

WHERE

  UserId= @P2 
