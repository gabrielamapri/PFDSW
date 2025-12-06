-- Script generado a partir de la migración `MigracionInicial`
-- Recomendación: Hacer backup de la base de datos antes de ejecutar.
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;

CREATE TABLE [Especialidades] (
    [IdEspecialidad] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Nombre] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(500) NULL,
    [Activo] bit NOT NULL DEFAULT 1
);
GO

CREATE TABLE [GrupoSanguineo] (
    [IdGrupo] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Codigo] nvarchar(5) NOT NULL,
    [Nombre] nvarchar(50) NULL,
    [Activo] bit NOT NULL DEFAULT 1
);
GO

CREATE TABLE [Doctores] (
    [IdDoctor] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Nombres] nvarchar(100) NOT NULL,
    [Apellidos] nvarchar(100) NOT NULL,
    [DNI] nvarchar(8) NOT NULL,
    [IdEspecialidad] int NOT NULL,
    [Telefono] nvarchar(15) NULL,
    [Email] nvarchar(100) NULL,
    [CMP] nvarchar(50) NULL,
    [RNE] nvarchar(50) NULL,
    [Direccion] nvarchar(200) NULL,
    [Activo] bit NOT NULL DEFAULT 1,
    [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [FK_Doctores_Especialidades_IdEspecialidad] FOREIGN KEY ([IdEspecialidad]) REFERENCES [Especialidades]([IdEspecialidad]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Pacientes] (
    [IdPaciente] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Nombres] nvarchar(100) NOT NULL,
    [Apellidos] nvarchar(100) NOT NULL,
    [DNI] nvarchar(8) NOT NULL,
    [FechaNacimiento] date NOT NULL,
    [Sexo] nvarchar(1) NULL,
    [Direccion] nvarchar(200) NULL,
    [Telefono] nvarchar(15) NULL,
    [Email] nvarchar(100) NULL,
    [GrupoSanguineo] nvarchar(5) NULL,
    [Activo] bit NOT NULL DEFAULT 1,
    [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE())
);
GO

CREATE TABLE [Horarios] (
    [IdHorario] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [IdDoctor] int NOT NULL,
    [DiaSemana] nvarchar(10) NOT NULL,
    [HoraInicio] time NOT NULL,
    [HoraFin] time NOT NULL,
    [DuracionCitaMinutos] int NOT NULL DEFAULT 30,
    [Activo] bit NOT NULL DEFAULT 1,
    CONSTRAINT [FK_Horarios_Doctores_IdDoctor] FOREIGN KEY ([IdDoctor]) REFERENCES [Doctores]([IdDoctor]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Citas] (
    [IdCita] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [IdPaciente] int NOT NULL,
    [IdDoctor] int NOT NULL,
    [FechaCita] date NOT NULL,
    [HoraCita] time NOT NULL,
    [Estado] nvarchar(20) NOT NULL DEFAULT 'Pendiente',
    [Motivo] nvarchar(500) NULL,
    [Observaciones] nvarchar(1000) NULL,
    [MontoPagado] decimal(10,2) NULL,
    [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [FK_Citas_Pacientes_IdPaciente] FOREIGN KEY ([IdPaciente]) REFERENCES [Pacientes]([IdPaciente]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Citas_Doctores_IdDoctor] FOREIGN KEY ([IdDoctor]) REFERENCES [Doctores]([IdDoctor]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Usuarios] (
    [IdUsuario] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NombreUsuario] nvarchar(50) NOT NULL,
    [Contraseña] nvarchar(255) NOT NULL,
    [TipoUsuario] nvarchar(20) NOT NULL,
    [IdRelacionado] int NULL,
    [Activo] bit NOT NULL DEFAULT 1,
    [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE())
);
GO

-- Indexes and unique constraints
CREATE UNIQUE INDEX IX_Especialidades_Nombre ON [Especialidades]([Nombre]);
GO

CREATE UNIQUE INDEX IX_Doctores_DNI ON [Doctores]([DNI]);
GO

CREATE UNIQUE INDEX IX_Pacientes_DNI ON [Pacientes]([DNI]);
GO

CREATE UNIQUE INDEX IX_GrupoSanguineo_Codigo ON [GrupoSanguineo]([Codigo]);
GO

CREATE UNIQUE INDEX IX_Usuarios_NombreUsuario ON [Usuarios]([NombreUsuario]);
GO

CREATE INDEX IX_Doctores_IdEspecialidad ON [Doctores]([IdEspecialidad]);
GO

CREATE INDEX IX_Horarios_IdDoctor ON [Horarios]([IdDoctor]);
GO

CREATE INDEX IX_Citas_IdPaciente ON [Citas]([IdPaciente]);
GO

CREATE INDEX IX_Citas_IdDoctor ON [Citas]([IdDoctor]);
GO

-- Registrar la migración en __EFMigrationsHistory
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20251206130000_MigracionInicial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20251206130000_MigracionInicial', '8.0.13');
END;
GO

COMMIT;
GO
