-- UpdateTaskAbort
-- 2015/04/13 Sandeep
UPDATE
    [AsyncProcessingServiceStatusManagementTable]
SET
    [CommandId] = @P2
WHERE
    [Id] = @P1