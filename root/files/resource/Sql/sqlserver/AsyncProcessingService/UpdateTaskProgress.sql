-- UpdateTaskProgress
-- 2015/04/13 Sandeep
UPDATE
    [AsyncProcessingServiceStatusManagementTable]
SET
    [ProgressRate] = @P2
WHERE
    [Id] = @P1