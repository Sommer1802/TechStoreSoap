-- Base de datos del Taller SOAP - TechStore S.A.S
-- Se ejecuta automáticamente al iniciar el servicio (Data/InicializadorBD.cs),
-- pero también se puede correr a mano en SQL Server Management Studio.

IF DB_ID('TechStoreSOAP') IS NULL
    CREATE DATABASE TechStoreSOAP;
GO

USE TechStoreSOAP;
GO

IF OBJECT_ID('dbo.Producto') IS NULL
BEGIN
    CREATE TABLE Producto (
        IdProducto  INT IDENTITY(1,1) PRIMARY KEY,
        Codigo      VARCHAR(20)    NOT NULL UNIQUE,
        Nombre      VARCHAR(100)   NOT NULL,
        Descripcion VARCHAR(250)   NOT NULL DEFAULT '',
        Categoria   VARCHAR(50)    NOT NULL,
        Precio      DECIMAL(12,2)  NOT NULL CHECK (Precio > 0),
        Stock       INT            NOT NULL CHECK (Stock >= 0),
        Sucursal    VARCHAR(50)    NOT NULL
    );

    INSERT INTO Producto (Codigo, Nombre, Descripcion, Categoria, Precio, Stock, Sucursal) VALUES
    ('PC-001',  'Portátil 15 pulgadas',     'Core i5, 8 GB RAM, SSD 512 GB',          'Computadores', 2400000, 10, 'Barranquilla Centro'),
    ('IMP-001', 'Impresora multifuncional', 'Impresión, copia y escaneo con wifi',     'Impresoras',    650000, 15, 'Barranquilla Norte'),
    ('DD-001',  'Disco duro externo 1 TB',  'USB 3.0, portátil',                       'Almacenamiento',280000, 30, 'Barranquilla Centro'),
    ('CAM-001', 'Cámara IP HD',             'Cámara de vigilancia con visión nocturna','Seguridad',     180000, 25, 'Soledad'),
    ('MON-001', 'Monitor 24 pulgadas',      'Full HD, panel IPS',                      'Monitores',     720000,  8, 'Barranquilla Norte');
END
GO
