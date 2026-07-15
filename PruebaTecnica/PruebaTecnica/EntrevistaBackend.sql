-- =========================================================================
-- Script de creación completa: Base de datos EntrevistaBackend
-- Ejecutar de principio a fin sobre una instancia limpia de SQL Server / LocalDB
-- =========================================================================

-- =========================================================================
-- 1. Creación de la base de datos
-- =========================================================================
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'EntrevistaBackend')
BEGIN
    CREATE DATABASE EntrevistaBackend;
END
GO

USE EntrevistaBackend;
GO

-- =========================================================================
-- 2. Tabla Productos (con restricción de stock no negativo)
-- =========================================================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Productos')
BEGIN
    CREATE TABLE Productos (
        IdProducto INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL,
        Precio DECIMAL(18,2) NOT NULL,
        Stock INT NOT NULL CONSTRAINT CK_Productos_Stock_NoNegativo CHECK (Stock >= 0)
    );
END
GO

-- =========================================================================
-- 3. Datos de prueba en Productos (solo si la tabla está vacía)
-- =========================================================================
IF NOT EXISTS (SELECT 1 FROM Productos)
BEGIN
    INSERT INTO Productos (Nombre, Precio, Stock) VALUES 
        ('Laptop Dell Vostro', 1200.50, 10),
        ('Monitor ASUS 24"', 250.00, 5),
        ('Teclado Mecánico Logitech', 85.00, 0);
END
GO

-- =========================================================================
-- 4. Tabla Proveedores (introducida en la Fase 2)
-- =========================================================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Proveedores')
BEGIN
    CREATE TABLE Proveedores (
        IdProveedor INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(100) NOT NULL
    );
END
GO

-- =========================================================================
-- 5. Relación Productos -> Proveedores (columna + clave foránea)
-- =========================================================================
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'IdProveedor'
)
BEGIN
    ALTER TABLE Productos ADD IdProveedor INT NULL;

    ALTER TABLE Productos ADD CONSTRAINT FK_Productos_Proveedores
        FOREIGN KEY (IdProveedor) REFERENCES Proveedores(IdProveedor);
END
GO

-- =========================================================================
-- 6. Datos de prueba en Proveedores + asignación al producto 1
-- =========================================================================
IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Dell LATAM')
BEGIN
    INSERT INTO Proveedores (Nombre) VALUES ('Dell LATAM');

    UPDATE Productos 
    SET IdProveedor = SCOPE_IDENTITY() 
    WHERE IdProducto = 1;
END
GO

-- =========================================================================
-- Fin del script. La base queda lista con:
--   - Productos: Laptop Dell Vostro, Monitor ASUS 24", Teclado Mecánico Logitech
--   - Proveedores: Dell LATAM (asignado a la Laptop, IdProducto = 1)
-- =========================================================================