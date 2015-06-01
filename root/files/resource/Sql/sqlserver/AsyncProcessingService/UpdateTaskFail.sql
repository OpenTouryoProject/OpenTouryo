-- UpdateTaskFail
-- 2015/04/13 Sandeep
UPDATE
    [AsyncProcessingServiceStatusManagementTable]
SET
    [CompletionDateTime] = @P2
    ,[StatusId] = @P3
	,[ExceptionInfo] = @P4
WHERE
    [Id] = @P1