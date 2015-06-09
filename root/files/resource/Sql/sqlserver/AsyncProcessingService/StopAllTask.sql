-- StopAllTask
-- 2015/06/09 Sandeep
UPDATE
    [AsyncProcessingServiceStatusManagementTable]
SET
    [CommandId] = @P2
WHERE
    [StatusId] = @P1