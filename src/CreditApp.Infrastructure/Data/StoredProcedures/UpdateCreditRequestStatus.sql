CREATE OR ALTER PROCEDURE [dbo].[UpdateCreditRequestStatus]
    @CreditRequestId UNIQUEIDENTIFIER,
    @NewStatus NVARCHAR(20),
    @RejectionReason NVARCHAR(500) = NULL,
    @ApprovedBy NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @OldStatus NVARCHAR(20);
    DECLARE @UserId UNIQUEIDENTIFIER;
    
    -- Get current status and user ID
    SELECT @OldStatus = Status, @UserId = UserId
    FROM CreditRequests
    WHERE Id = @CreditRequestId;
    
    -- Update credit request
    UPDATE CreditRequests
    SET 
        Status = @NewStatus,
        UpdatedAt = GETUTCDATE(),
        RejectionReason = CASE 
            WHEN @NewStatus = 'Rejected' THEN @RejectionReason 
            ELSE RejectionReason 
        END,
        ApprovedBy = CASE 
            WHEN @NewStatus = 'Approved' THEN @ApprovedBy 
            ELSE ApprovedBy 
        END,
        ApprovedAt = CASE 
            WHEN @NewStatus = 'Approved' THEN GETUTCDATE() 
            ELSE ApprovedAt 
        END
    WHERE Id = @CreditRequestId;
    
    -- Log the change
    INSERT INTO AuditLogs (
        Id,
        EntityName,
        EntityId,
        Action,
        Details,
        PerformedBy,
        Timestamp
    )
    VALUES (
        NEWID(),
        'CreditRequest',
        @CreditRequestId,
        'StatusUpdate',
        'Status changed from ' + @OldStatus + ' to ' + @NewStatus,
        @ApprovedBy,
        GETUTCDATE()
    );
END 