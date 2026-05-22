/* =====================================================================
   EXAMEN TÉCNICO IT — Base de datos del sistema de Compras y Ventas
   Motor: SQL Server 2022
   Archivo 01: creación de la base de datos y las tablas
   ===================================================================== */

IF DB_ID('VentasComprasDB') IS NULL
    CREATE DATABASE VentasComprasDB;
GO

USE VentasComprasDB;
GO

/* ---------------------------------------------------------------------
   Usuarios — para autenticación JWT
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
GO
CREATE TABLE dbo.Usuarios (
    Id_Usuario     INT IDENTITY(1,1) PRIMARY KEY,
    Username       NVARCHAR(50)  NOT NULL UNIQUE,
    PasswordHash   NVARCHAR(255) NOT NULL,
    Nombre         NVARCHAR(100) NOT NULL,
    Activo         BIT           NOT NULL DEFAULT(1),
    Fec_registro   DATETIME      NOT NULL DEFAULT(GETDATE())
);
GO

/* ---------------------------------------------------------------------
   Productos
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Productos', 'U') IS NOT NULL DROP TABLE dbo.Productos;
GO
CREATE TABLE dbo.Productos (
    Id_producto      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre_producto  NVARCHAR(150)  NOT NULL,
    NroLote          NVARCHAR(50)   NULL,
    Fec_registro     DATETIME       NOT NULL DEFAULT(GETDATE()),
    Costo            DECIMAL(18,2)  NOT NULL DEFAULT(0),
    PrecioVenta      DECIMAL(18,2)  NOT NULL DEFAULT(0)
);
GO
CREATE INDEX IX_Productos_Nombre ON dbo.Productos(Nombre_producto);
GO

/* ---------------------------------------------------------------------
   Compras (cabecera y detalle)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.CompraDet', 'U') IS NOT NULL DROP TABLE dbo.CompraDet;
IF OBJECT_ID('dbo.CompraCab', 'U') IS NOT NULL DROP TABLE dbo.CompraCab;
GO
CREATE TABLE dbo.CompraCab (
    Id_CompraCab  INT IDENTITY(1,1) PRIMARY KEY,
    FecRegistro   DATETIME      NOT NULL DEFAULT(GETDATE()),
    SubTotal      DECIMAL(18,2) NOT NULL DEFAULT(0),
    Igv           DECIMAL(18,2) NOT NULL DEFAULT(0),
    Total         DECIMAL(18,2) NOT NULL DEFAULT(0)
);
GO
CREATE TABLE dbo.CompraDet (
    Id_CompraDet  INT IDENTITY(1,1) PRIMARY KEY,
    Id_CompraCab  INT NOT NULL FOREIGN KEY REFERENCES dbo.CompraCab(Id_CompraCab),
    Id_producto   INT NOT NULL FOREIGN KEY REFERENCES dbo.Productos(Id_producto),
    Cantidad      INT           NOT NULL,
    Precio        DECIMAL(18,2) NOT NULL,
    Sub_Total     DECIMAL(18,2) NOT NULL,
    Igv           DECIMAL(18,2) NOT NULL,
    Total         DECIMAL(18,2) NOT NULL
);
GO
CREATE INDEX IX_CompraDet_Cab ON dbo.CompraDet(Id_CompraCab);
GO

/* ---------------------------------------------------------------------
   Ventas (cabecera y detalle)
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.VentaDet', 'U') IS NOT NULL DROP TABLE dbo.VentaDet;
IF OBJECT_ID('dbo.VentaCab', 'U') IS NOT NULL DROP TABLE dbo.VentaCab;
GO
CREATE TABLE dbo.VentaCab (
    Id_VentaCab   INT IDENTITY(1,1) PRIMARY KEY,
    fecRegistro   DATETIME      NOT NULL DEFAULT(GETDATE()),
    SubTotal      DECIMAL(18,2) NOT NULL DEFAULT(0),
    Igv           DECIMAL(18,2) NOT NULL DEFAULT(0),
    Total         DECIMAL(18,2) NOT NULL DEFAULT(0)
);
GO
CREATE TABLE dbo.VentaDet (
    Id_VentaDet   INT IDENTITY(1,1) PRIMARY KEY,
    Id_VentaCab   INT NOT NULL FOREIGN KEY REFERENCES dbo.VentaCab(Id_VentaCab),
    Id_producto   INT NOT NULL FOREIGN KEY REFERENCES dbo.Productos(Id_producto),
    Cantidad      INT           NOT NULL,
    Precio        DECIMAL(18,2) NOT NULL,
    Sub_Total     DECIMAL(18,2) NOT NULL,
    Igv           DECIMAL(18,2) NOT NULL,
    Total         DECIMAL(18,2) NOT NULL
);
GO
CREATE INDEX IX_VentaDet_Cab ON dbo.VentaDet(Id_VentaCab);
GO

/* ---------------------------------------------------------------------
   Movimientos (Kardex) — cabecera y detalle
   Id_TipoMovimiento: 1 = Entrada (compra), 2 = Salida (venta)
   Id_DocumentoOrigen: Id_CompraCab o Id_VentaCab segun el tipo
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.MovimientoDet', 'U') IS NOT NULL DROP TABLE dbo.MovimientoDet;
IF OBJECT_ID('dbo.MovimientoCab', 'U') IS NOT NULL DROP TABLE dbo.MovimientoCab;
GO
CREATE TABLE dbo.MovimientoCab (
    Id_MovimientoCab   INT IDENTITY(1,1) PRIMARY KEY,
    Fec_registro       DATETIME NOT NULL DEFAULT(GETDATE()),
    Id_TipoMovimiento  INT      NOT NULL,   -- 1 Entrada, 2 Salida
    Id_DocumentoOrigen INT      NOT NULL    -- Id_CompraCab o Id_VentaCab
);
GO
CREATE TABLE dbo.MovimientoDet (
    Id_MovimientoDet   INT IDENTITY(1,1) PRIMARY KEY,
    Id_movimientocab   INT NOT NULL FOREIGN KEY REFERENCES dbo.MovimientoCab(Id_MovimientoCab),
    Id_Producto        INT NOT NULL FOREIGN KEY REFERENCES dbo.Productos(Id_producto),
    Cantidad           INT NOT NULL
);
GO
CREATE INDEX IX_MovimientoDet_Producto ON dbo.MovimientoDet(Id_Producto);
CREATE INDEX IX_MovimientoCab_Tipo ON dbo.MovimientoCab(Id_TipoMovimiento);
GO

PRINT 'Tablas creadas correctamente.';
GO
