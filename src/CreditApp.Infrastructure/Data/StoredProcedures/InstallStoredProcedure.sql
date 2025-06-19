IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'UpdateCreditRequestStatus')
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[UpdateCreditRequestStatus]
        @CreditRequestId UNIQUEIDENTIFIER,
        @NewStatus NVARCHAR(50),
        @RejectionReason NVARCHAR(MAX) = NULL,
        @ApprovedBy NVARCHAR(100) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @CurrentStatus NVARCHAR(50)
        SELECT @CurrentStatus = Status FROM CreditRequests WHERE Id = @CreditRequestId
        
        IF @CurrentStatus IS NULL
            THROW 50000, ''Credit request not found'', 1
            
        IF @CurrentStatus = ''Approved'' OR @CurrentStatus = ''Rejected''
            THROW 50000, ''Cannot change status of an already approved or rejected request'', 1
            
        BEGIN TRANSACTION
            UPDATE CreditRequests
            SET 
                Status = @NewStatus,
                RejectionReason = @RejectionReason,
                ApprovedBy = @ApprovedBy,
                ApprovedAt = CASE 
                    WHEN @NewStatus IN (''Approved'', ''Rejected'') THEN GETUTCDATE()
                    ELSE ApprovedAt
                END,
                UpdatedAt = GETUTCDATE()
            WHERE Id = @CreditRequestId
            
            INSERT INTO AuditLogs (
                EntityName,
                EntityId,
                Action,
                Details,
                PerformedBy
            )
            VALUES (
                ''CreditRequest'',
                @CreditRequestId,
                ''StatusUpdate'',
                ''Status changed from '' + @CurrentStatus + '' to '' + @NewStatus,
                @ApprovedBy
            )
        COMMIT TRANSACTION
    END
    ')
END 