-- ============================================
-- ConstruControl - Esquema de Base de Datos
-- SQL Server
-- ============================================

CREATE DATABASE ConstruControlDB;
GO

USE ConstruControlDB;
GO

-- ============================================
-- USUARIOS Y ROLES
-- ============================================
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreCompleto NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(300) NOT NULL,
    Rol NVARCHAR(30) NOT NULL CHECK (Rol IN ('Admin','JefeObra','Bodeguero','Empleado')),
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- ============================================
-- OBRAS
-- ============================================
CREATE TABLE Obras (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Ubicacion NVARCHAR(250) NOT NULL,
    Presupuesto DECIMAL(18,2) NOT NULL,
    FechaInicio DATE NOT NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Planificacion'
        CHECK (Estado IN ('Planificacion','Activa','Pausada','Finalizada')),
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- ============================================
-- MATERIALES
-- ============================================
CREATE TABLE Materiales (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Stock DECIMAL(18,2) NOT NULL DEFAULT 0,
    StockMinimo DECIMAL(18,2) NOT NULL DEFAULT 0,
    Unidad NVARCHAR(20) NOT NULL, -- ej: 'kg', 'saco', 'm3', 'unidad'
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);

-- ============================================
-- PROVEEDORES
-- ============================================
CREATE TABLE Proveedores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Contacto NVARCHAR(150) NULL,
    Telefono NVARCHAR(30) NULL,
    Email NVARCHAR(150) NULL,
    Activo BIT NOT NULL DEFAULT 1
);

-- ============================================
-- COMPRAS (cabecera)
-- ============================================
CREATE TABLE Compras (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProveedorId INT NOT NULL FOREIGN KEY REFERENCES Proveedores(Id),
    ObraId INT NOT NULL FOREIGN KEY REFERENCES Obras(Id),
    UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id), -- quien crea la orden
    Fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
    FechaRecepcion DATETIME2 NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente'
        CHECK (Estado IN ('Pendiente','Recibida','Cancelada')),
    Total DECIMAL(18,2) NOT NULL DEFAULT 0
);

-- ============================================
-- DETALLE DE COMPRA (líneas)
-- ============================================
CREATE TABLE DetalleCompra (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CompraId INT NOT NULL FOREIGN KEY REFERENCES Compras(Id),
    MaterialId INT NOT NULL FOREIGN KEY REFERENCES Materiales(Id),
    Cantidad DECIMAL(18,2) NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL
);

-- ============================================
-- CONSUMO DE MATERIALES
-- ============================================
CREATE TABLE Consumos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MaterialId INT NOT NULL FOREIGN KEY REFERENCES Materiales(Id),
    ObraId INT NOT NULL FOREIGN KEY REFERENCES Obras(Id),
    ResponsableId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    Cantidad DECIMAL(18,2) NOT NULL,
    Fecha DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- ============================================
-- PERSONAL
-- ============================================
CREATE TABLE Empleados (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Cargo NVARCHAR(100) NOT NULL,
    FechaIngreso DATE NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE Asistencias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EmpleadoId INT NOT NULL FOREIGN KEY REFERENCES Empleados(Id),
    ObraId INT NOT NULL FOREIGN KEY REFERENCES Obras(Id),
    Fecha DATE NOT NULL,
    HoraEntrada TIME NULL,
    HoraSalida TIME NULL
);

-- ============================================
-- ADJUNTOS: FACTURAS Y FOTOS
-- ============================================
CREATE TABLE Facturas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CompraId INT NOT NULL FOREIGN KEY REFERENCES Compras(Id),
    RutaArchivo NVARCHAR(400) NOT NULL,
    FechaSubida DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE TABLE FotosObra (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ObraId INT NOT NULL FOREIGN KEY REFERENCES Obras(Id),
    RutaArchivo NVARCHAR(400) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    FechaSubida DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- ============================================
-- NOTIFICACIONES (motor de automatización)
-- ============================================
CREATE TABLE Notificaciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Tipo NVARCHAR(30) NOT NULL CHECK (Tipo IN ('StockBajo','PresupuestoExcedido','CompraSugerida')),
    ObraId INT NULL FOREIGN KEY REFERENCES Obras(Id),
    MaterialId INT NULL FOREIGN KEY REFERENCES Materiales(Id),
    Mensaje NVARCHAR(500) NOT NULL,
    Leida BIT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- ============================================
-- LOGS / AUDITORÍA
-- ============================================
CREATE TABLE Logs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NULL FOREIGN KEY REFERENCES Usuarios(Id),
    Accion NVARCHAR(50) NOT NULL, -- 'Crear','Actualizar','Eliminar'
    Entidad NVARCHAR(50) NOT NULL, -- 'Obra','Material','Compra', etc.
    EntidadId INT NULL,
    Detalle NVARCHAR(MAX) NULL,
    Fecha DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO
