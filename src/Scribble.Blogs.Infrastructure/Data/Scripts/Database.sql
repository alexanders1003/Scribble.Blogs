IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Scribble.Blogs')
    BEGIN

        CREATE DATABASE [Scribble.Blogs]

        USE [Scribble.Blogs]

        CREATE TABLE [dbo].[Blogs]
        (
            [Id]          [UNIQUEIDENTIFIER] NOT NULL PRIMARY KEY DEFAULT NEWID(),
            [AuthorId]    [UNIQUEIDENTIFIER] NOT NULL,
            [Title]       [NVARCHAR](500)    NOT NULL,
            [Description] [NVARCHAR]         NULL,
            [CreatedAt]   [DATETIME]         NOT NULL DEFAULT SYSDATETIME(),
        );

    END