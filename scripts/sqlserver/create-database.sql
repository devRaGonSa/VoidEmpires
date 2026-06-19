/*
Manual SSMS helper for the VoidEmpires SQL Server target.

Usage:
1. Open this file in SQL Server Management Studio.
2. Connect with operator credentials that can create databases.
3. Confirm the active database context is [master].
4. Review file locations, sizing, and growth settings for your infrastructure.
5. Execute manually.

This script does not create logins, users, or passwords.
This script does not apply EF Core migrations.
*/

USE [master];
GO

IF DB_ID(N'VoidEmpires') IS NULL
BEGIN
    CREATE DATABASE [VoidEmpires];
END;
GO

IF DB_ID(N'VoidEmpires') IS NOT NULL
BEGIN
    PRINT N'Database [VoidEmpires] is present.';
END;
GO
