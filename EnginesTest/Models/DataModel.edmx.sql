
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 04/07/2015 10:54:00
-- Generated from EDMX file: D:\Users\aapopov\Documents\Visual Studio 2012\Projects\EnginesTest\EnginesTest\Models\DataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [EnginesTest];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_TestMeasurement]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Measurements] DROP CONSTRAINT [FK_TestMeasurement];
GO
IF OBJECT_ID(N'[dbo].[FK_EngineTest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tests] DROP CONSTRAINT [FK_EngineTest];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tests] DROP CONSTRAINT [FK_UserTest];
GO
IF OBJECT_ID(N'[dbo].[FK_JobTitleUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_JobTitleUser];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Measurements]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Measurements];
GO
IF OBJECT_ID(N'[dbo].[Tests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tests];
GO
IF OBJECT_ID(N'[dbo].[Engines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Engines];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[JobTitles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JobTitles];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Measurements'
CREATE TABLE [dbo].[Measurements] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TestId] int  NOT NULL,
    [Torque] int  NOT NULL,
    [RPM] int  NOT NULL,
    [FuelConsumption] float  NOT NULL,
    [TCoolant] float  NOT NULL,
    [TOil] float  NOT NULL,
    [TFuel] float  NOT NULL,
    [TExhaustGas] float  NOT NULL,
    [POil] float  NOT NULL,
    [PExhaustGas] float  NOT NULL,
    [PowerHP] float  NOT NULL,
    [PowerKWh] float  NOT NULL
);
GO

-- Creating table 'Tests'
CREATE TABLE [dbo].[Tests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UID] nvarchar(32)  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [EngineId] int  NOT NULL,
    [UserId] int  NOT NULL,
    [TIncomingAir] float  NOT NULL,
    [PBarometric] float  NOT NULL
);
GO

-- Creating table 'Engines'
CREATE TABLE [dbo].[Engines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UID] nvarchar(32)  NOT NULL,
    [Model] nvarchar(11)  NOT NULL,
    [SerialNumber] nvarchar(10)  NOT NULL,
    [Configuration] nvarchar(1)  NOT NULL,
    [Cylinders] int  NOT NULL,
    [EngineCapacity] float  NOT NULL,
    [ValversPerCylinder] int  NOT NULL,
    [FuelType] nvarchar(1)  NOT NULL,
    [RatedTorque] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(50)  NOT NULL,
    [LastName] nvarchar(100)  NOT NULL,
    [MiddleName] nvarchar(50)  NULL,
    [Phone] nvarchar(20)  NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [JobTitleId] int  NOT NULL,
    [ProfileImage] nvarchar(50)  NULL,
    [SecurityUM] bit  NOT NULL,
    [SecurityTV] bit  NOT NULL,
    [SecurityTM] bit  NOT NULL,
    [Login] nvarchar(50)  NOT NULL,
    [Password] nvarchar(32)  NOT NULL,
    [Deleted] bit  NOT NULL
);
GO

-- Creating table 'JobTitles'
CREATE TABLE [dbo].[JobTitles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(100)  NOT NULL,
    [Deleted] bit  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Measurements'
ALTER TABLE [dbo].[Measurements]
ADD CONSTRAINT [PK_Measurements]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tests'
ALTER TABLE [dbo].[Tests]
ADD CONSTRAINT [PK_Tests]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Engines'
ALTER TABLE [dbo].[Engines]
ADD CONSTRAINT [PK_Engines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'JobTitles'
ALTER TABLE [dbo].[JobTitles]
ADD CONSTRAINT [PK_JobTitles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TestId] in table 'Measurements'
ALTER TABLE [dbo].[Measurements]
ADD CONSTRAINT [FK_TestMeasurement]
    FOREIGN KEY ([TestId])
    REFERENCES [dbo].[Tests]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TestMeasurement'
CREATE INDEX [IX_FK_TestMeasurement]
ON [dbo].[Measurements]
    ([TestId]);
GO

-- Creating foreign key on [EngineId] in table 'Tests'
ALTER TABLE [dbo].[Tests]
ADD CONSTRAINT [FK_EngineTest]
    FOREIGN KEY ([EngineId])
    REFERENCES [dbo].[Engines]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EngineTest'
CREATE INDEX [IX_FK_EngineTest]
ON [dbo].[Tests]
    ([EngineId]);
GO

-- Creating foreign key on [UserId] in table 'Tests'
ALTER TABLE [dbo].[Tests]
ADD CONSTRAINT [FK_UserTest]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTest'
CREATE INDEX [IX_FK_UserTest]
ON [dbo].[Tests]
    ([UserId]);
GO

-- Creating foreign key on [JobTitleId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_JobTitleUser]
    FOREIGN KEY ([JobTitleId])
    REFERENCES [dbo].[JobTitles]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JobTitleUser'
CREATE INDEX [IX_FK_JobTitleUser]
ON [dbo].[Users]
    ([JobTitleId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------