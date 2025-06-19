IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'creditapp')
BEGIN
    CREATE DATABASE [creditapp];
END