-- UpdateTaskRetry
-- 2015/04/13 Sandeep
UPDATE
    [AsyncProcessingServiceStatusManagementTable]
SET
    [NumberOfRetries] = @P2
    ,[CompletionDateTime] = @P3
    ,[StatusId] = @P4
	,[ExceptionInfo] = @P5
WHERE
    [Id] = @P1