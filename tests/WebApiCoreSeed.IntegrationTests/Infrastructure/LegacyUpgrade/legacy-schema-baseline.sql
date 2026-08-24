CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles] ([NormalizedName])
WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [dbo].[AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers] ([NormalizedUserName])
WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE TABLE [dbo].[Atendentes] (
    [Id] uniqueidentifier NOT NULL,
    [Nome] varchar(100) NOT NULL,
    [TipoAtendente] int NOT NULL,
    CONSTRAINT [PK_Atendentes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[Loggin] (
    [Id] uniqueidentifier NOT NULL,
    [EventId] int NULL,
    [Escopo] varchar(100) NULL,
    [LogLevel] int NOT NULL,
    [Message] varchar(6000) NOT NULL,
    [CreatedTime] datetime NULL,
    CONSTRAINT [PK_Loggin] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[Mesas] (
    [Id] uniqueidentifier NOT NULL,
    [Numero] varchar(50) NOT NULL,
    [Lugares] int NOT NULL CONSTRAINT [DF_Mesas_Lugares] DEFAULT (4),
    [Ativo] bit NOT NULL CONSTRAINT [DF_Mesas_Ativo] DEFAULT (1),
    [LocalizacaoMesa] int NOT NULL,
    CONSTRAINT [PK_Mesas] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[Pratos] (
    [Id] uniqueidentifier NOT NULL,
    [Titulo] varchar(200) NOT NULL,
    [Descricao] varchar(800) NOT NULL,
    [Foto] varchar(200) NOT NULL,
    [Preco] float NOT NULL,
    [Ativo] bit NOT NULL CONSTRAINT [DF_Pratos_Ativo] DEFAULT (1),
    [TipoPrato] int NOT NULL,
    CONSTRAINT [PK_Pratos] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[Pedidos] (
    [Id] uniqueidentifier NOT NULL,
    [AtendenteId] uniqueidentifier NOT NULL,
    [MesaId] uniqueidentifier NOT NULL,
    [Numero] varchar(50) NOT NULL,
    [DataHoraCadastro] datetime NOT NULL CONSTRAINT [DF_Pedidos_DataHoraCadastro] DEFAULT (getdate()),
    [DataHoraEncerrado] datetime NULL,
    CONSTRAINT [PK_Pedidos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Pedidos_Atendentes] FOREIGN KEY ([AtendenteId])
        REFERENCES [dbo].[Atendentes] ([Id]),
    CONSTRAINT [FK_Pedidos_Mesas] FOREIGN KEY ([MesaId])
        REFERENCES [dbo].[Mesas] ([Id])
);
GO

CREATE TABLE [dbo].[PedidoPrato] (
    [Id] uniqueidentifier NOT NULL,
    [PedidoId] uniqueidentifier NOT NULL,
    [PratoId] uniqueidentifier NOT NULL,
    [StatusProducao] int NOT NULL,
    [Observacao] varchar(1000) NULL,
    CONSTRAINT [PK_PedidoPrato] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PedidoPrato_Pedidos] FOREIGN KEY ([PedidoId])
        REFERENCES [dbo].[Pedidos] ([Id]),
    CONSTRAINT [FK_PedidoPrato_Pratos] FOREIGN KEY ([PratoId])
        REFERENCES [dbo].[Pratos] ([Id])
);
GO

CREATE INDEX [IX_PedidoPrato_PedidoId] ON [dbo].[PedidoPrato] ([PedidoId]);
GO

CREATE INDEX [IX_PedidoPrato_PratoId] ON [dbo].[PedidoPrato] ([PratoId]);
GO

CREATE INDEX [IX_Pedidos_AtendenteId] ON [dbo].[Pedidos] ([AtendenteId]);
GO

CREATE INDEX [IX_Pedidos_MesaId] ON [dbo].[Pedidos] ([MesaId]);
GO

INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES
    (N'20200817223121_InitialCreate', N'3.1.2'),
    (N'20200817223231_InitialCreate', N'3.1.2');
GO
