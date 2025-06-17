SELECT 
    SCHEMA_NAME(schema_id) as SchemaName,
    name as ProcedureName,
    create_date as CreatedDate,
    modify_date as ModifiedDate
FROM sys.procedures
WHERE name = 'UpdateCreditRequestStatus'; 