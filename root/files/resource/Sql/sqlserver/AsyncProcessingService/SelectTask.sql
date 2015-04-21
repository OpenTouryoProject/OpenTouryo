-- SelectTask
-- 2015/04/09 Sandeep
SELECT
TOP(1)
    [Id]
    ,[UserId]
    ,[ProcessName]
    ,[Data]
    ,[NumberOfRetries]
    ,[ReservedArea]
FROM
    [AsyncProcessingServiceStatusManagementTable]
Where
    ([RegistrationDateTime] >= @P1 OR [CompletionDateTime] >= @P7)
    AND [NumberOfRetries]  <  @P2
    AND ([StatusId] = @P3 OR [StatusId] = @P4)
    AND [CommandId] <> @P5 
	AND [CommandId] <> @P6
