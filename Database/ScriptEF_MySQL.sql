-- Script MySQL generado a partir de la migración del modelo
-- Recomendación: hacer backup antes de ejecutar en una BD con datos

SET FOREIGN_KEY_CHECKS = 0;

-- Tabla Especialidades
CREATE TABLE IF NOT EXISTS `Especialidades` (
  `IdEspecialidad` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `Descripcion` varchar(500) DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdEspecialidad`),
  UNIQUE KEY `IX_Especialidades_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla GrupoSanguineo
CREATE TABLE IF NOT EXISTS `GrupoSanguineo` (
  `IdGrupo` int NOT NULL AUTO_INCREMENT,
  `Codigo` varchar(5) NOT NULL,
  `Nombre` varchar(50) DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdGrupo`),
  UNIQUE KEY `IX_GrupoSanguineo_Codigo` (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla Doctores
CREATE TABLE IF NOT EXISTS `Doctores` (
  `IdDoctor` int NOT NULL AUTO_INCREMENT,
  `Nombres` varchar(100) NOT NULL,
  `Apellidos` varchar(100) NOT NULL,
  `DNI` varchar(8) NOT NULL,
  `IdEspecialidad` int NOT NULL,
  `Telefono` varchar(15) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `CMP` varchar(50) DEFAULT NULL,
  `RNE` varchar(50) DEFAULT NULL,
  `Direccion` varchar(200) DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT 1,
  `FechaRegistro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdDoctor`),
  UNIQUE KEY `IX_Doctores_DNI` (`DNI`),
  KEY `IX_Doctores_IdEspecialidad` (`IdEspecialidad`),
  CONSTRAINT `FK_Doctores_Especialidades_IdEspecialidad` FOREIGN KEY (`IdEspecialidad`) REFERENCES `Especialidades` (`IdEspecialidad`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla Pacientes
CREATE TABLE IF NOT EXISTS `Pacientes` (
  `IdPaciente` int NOT NULL AUTO_INCREMENT,
  `Nombres` varchar(100) NOT NULL,
  `Apellidos` varchar(100) NOT NULL,
  `DNI` varchar(8) NOT NULL,
  `FechaNacimiento` date NOT NULL,
  `Sexo` varchar(1) DEFAULT NULL,
  `Direccion` varchar(200) DEFAULT NULL,
  `Telefono` varchar(15) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `GrupoSanguineo` varchar(5) DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT 1,
  `FechaRegistro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdPaciente`),
  UNIQUE KEY `IX_Pacientes_DNI` (`DNI`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla Horarios
CREATE TABLE IF NOT EXISTS `Horarios` (
  `IdHorario` int NOT NULL AUTO_INCREMENT,
  `IdDoctor` int NOT NULL,
  `DiaSemana` varchar(10) NOT NULL,
  `HoraInicio` time NOT NULL,
  `HoraFin` time NOT NULL,
  `DuracionCitaMinutos` int NOT NULL DEFAULT 30,
  `Activo` tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`IdHorario`),
  KEY `IX_Horarios_IdDoctor` (`IdDoctor`),
  CONSTRAINT `FK_Horarios_Doctores_IdDoctor` FOREIGN KEY (`IdDoctor`) REFERENCES `Doctores` (`IdDoctor`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla Citas
CREATE TABLE IF NOT EXISTS `Citas` (
  `IdCita` int NOT NULL AUTO_INCREMENT,
  `IdPaciente` int NOT NULL,
  `IdDoctor` int NOT NULL,
  `FechaCita` date NOT NULL,
  `HoraCita` time NOT NULL,
  `Estado` varchar(20) NOT NULL DEFAULT 'Pendiente',
  `Motivo` varchar(500) DEFAULT NULL,
  `Observaciones` varchar(1000) DEFAULT NULL,
  `MontoPagado` decimal(10,2) DEFAULT NULL,
  `FechaRegistro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdCita`),
  KEY `IX_Citas_IdPaciente` (`IdPaciente`),
  KEY `IX_Citas_IdDoctor` (`IdDoctor`),
  CONSTRAINT `FK_Citas_Pacientes_IdPaciente` FOREIGN KEY (`IdPaciente`) REFERENCES `Pacientes` (`IdPaciente`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `FK_Citas_Doctores_IdDoctor` FOREIGN KEY (`IdDoctor`) REFERENCES `Doctores` (`IdDoctor`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabla Usuarios
CREATE TABLE IF NOT EXISTS `Usuarios` (
  `IdUsuario` int NOT NULL AUTO_INCREMENT,
  `NombreUsuario` varchar(50) NOT NULL,
  `Contraseña` varchar(255) NOT NULL,
  `TipoUsuario` varchar(20) NOT NULL,
  `IdRelacionado` int DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT 1,
  `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`IdUsuario`),
  UNIQUE KEY `IX_Usuarios_NombreUsuario` (`NombreUsuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Indexes adicionales
CREATE INDEX `IX_Doctores_IdEspecialidad` ON `Doctores` (`IdEspecialidad`);
CREATE INDEX `IX_Horarios_IdDoctor` ON `Horarios` (`IdDoctor`);
CREATE INDEX `IX_Citas_IdPaciente` ON `Citas` (`IdPaciente`);
CREATE INDEX `IX_Citas_IdDoctor` ON `Citas` (`IdDoctor`);

SET FOREIGN_KEY_CHECKS = 1;
