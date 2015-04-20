-- UpdateTaskSuccess
-- 2015/04/13 Sandeep
UPDATE
    [AsyncProcessingServiceStatusManagementTable]
SET
    [CompletionDateTime] = @P2
    ,[ProgressRate] = @P3
    ,[StatusId] = @P4
WHERE
    [Id] = @P1