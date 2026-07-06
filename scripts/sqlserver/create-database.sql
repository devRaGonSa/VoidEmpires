/*
Ayudante manual de SSMS para el primer destino SQL Server controlado de VoidEmpires.

Base recomendada para la primera validacion: [VoidEmpires_Dev].

Usage:
1. Open this file in SQL Server Management Studio.
2. Connect with operator credentials that can create databases.
3. Confirm the active database context is [master].
4. Review the database name, file locations, sizing, and growth settings for your infrastructure.
5. If an existing database with this name matters, take a backup before making any manual changes.
6. Execute manually only after review.

This script does not create logins, users, or passwords.
This script does not contain connection strings or credentials.
This script does not apply EF Core migrations.
This script does not drop, truncate, reset, or delete data.
*/

USE [master];
GO

IF DB_ID(N'VoidEmpires_Dev') IS NULL
BEGIN
    CREATE DATABASE [VoidEmpires_Dev];
END;
GO

IF DB_ID(N'VoidEmpires_Dev') IS NOT NULL
BEGIN
    PRINT N'Database [VoidEmpires_Dev] is present.';
END;
GO
