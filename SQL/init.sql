CREATE DATABASE coin;
GO
USE coin;
GO
CREATE TABLE [dbo].[CoinLang] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Code] NVARCHAR (8)  NOT NULL,
    [Name] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK_CoinLang] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IX_CoinLang_Code] ON [dbo].[CoinLang] ([Code] ASC);