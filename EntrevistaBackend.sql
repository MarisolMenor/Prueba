-- 1. Crear Base de Datos de Prueba
CREATE DATABASE EntrevistaBackend;
GO

USE EntrevistaBackend;
GO

-- 2. Crear Tabla de Productos con Restricción de Stock
CREATE TABLE Productos (
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL CONSTRAINT CK_Productos_Stock_NoNegativo CHECK (Stock >= 0)
);
GO

-- 3. Insertar Datos de Prueba
INSERT INTO Productos (Nombre, Precio, Stock) VALUES 
('Laptop Dell Vostro', 1200.50, 10),
('Monitor ASUS 24"', 250.00, 5),
('Teclado Mecánico Logitech', 85.00, 0);
GO