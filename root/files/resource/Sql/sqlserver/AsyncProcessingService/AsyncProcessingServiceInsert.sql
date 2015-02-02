-- AsyncProcessingServiceInsert
-- 2014/11/22 “ú—§ ‘¾˜Y
INSERT INTO 
  [AsyncProcessingServiceStatusManagementTable]
    (
      
      [UserId],
      [ProcessName],
      [Data],
      [RegistrationDateTime],
      [ExecutionStartDateTime],
      [NumberOfRetries],
      [CompletionDateTime],      
      [StatusId], 
      [ProgressRate],     
      [CommandId],
      [ReservedArea]
      
    )
VALUES
    (
      @P2,
      @P3,
      @P4,
      @P5,
      @P6,
      @P7,
      @P8,
      @P9,
      @P10,
      @P11,
      @P12
    )
SELECT SCOPE_IDENTITY();
