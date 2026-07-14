-- 1. Crear Base de Datos de Prueba
IF  DB_ID('EntrevistaBackend') IS NULL
    BEGIN
        CREATE DATABASE EntrevistaBackend;
    END
GO

USE EntrevistaBackend;
GO

-- 2. Crear Tabla de Productos con Restricción de Stock
IF  OBJECT_ID(N'Productos', N'U') IS NULL
    BEGIN
        CREATE TABLE Productos (
            IdProducto INT IDENTITY(1,1) PRIMARY KEY,
            Nombre VARCHAR(100) NOT NULL,
            Precio DECIMAL(18,2) NOT NULL,
            Stock INT NOT NULL CONSTRAINT CK_Productos_Stock_NoNegativo CHECK (Stock >= 0)
        );
    END
GO

-- 3. Insertar Datos de Prueba
IF  NOT EXISTS(
        SELECT  1
        FROM    Productos
    )
    BEGIN
        INSERT INTO Productos (Nombre, Precio, Stock) VALUES 
        ('Laptop Dell Vostro', 1200.50, 10),
        ('Monitor ASUS 24"', 250.00, 5),
        ('Teclado Mecánico Logitech', 85.00, 0);
    END
GO

-- 4. Crear Tabla de Proveedores
IF  OBJECT_ID(N'Proveedores', N'U') IS NULL
    BEGIN
        CREATE TABLE Proveedores (
            IdProveedor INT IDENTITY(1,1) PRIMARY KEY,
            Nombre VARCHAR(150) NOT NULL,
            Telefono CHAR(10) NOT NULL
        );
    END
GO

-- 5. Crear Proveedores de Ejemplo
IF  NOT EXISTS(
        SELECT  1
        FROM    Proveedores
    )
    BEGIN
        INSERT INTO Proveedores
        (
            Nombre,
            Telefono
        )
        VALUES
        ('DELL', '657814962'),
        ('ASUS', '637845126'),
        ('LOGITECH', '836985214')
    END
GO

-- 5. Añadir campo IdProveedor
IF  NOT EXISTS(
        SELECT  1
        FROM    INFORMATION_SCHEMA.COLUMNS
        WHERE   TABLE_SCHEMA = 'dbo'
                AND TABLE_NAME = 'Productos'
                AND COLUMN_NAME = 'IdProveedor'
    )
    BEGIN
        ALTER TABLE Productos
            ADD IdProveedor INT NULL
    END
GO

IF  EXISTS(
        SELECT  1
        FROM    Productos
        WHERE   IdProveedor IS NULL
    )
    BEGIN
        UPDATE  prod
        SET     IdProveedor = prov.IdProveedor
        FROM    Productos AS prod
                INNER JOIN Proveedores AS prov
                    ON  prod.Nombre LIKE CONCAT('%', prov.Nombre, '%')
    
        ALTER TABLE Productos
            ALTER COLUMN IdProveedor INT NOT NULL
    END
GO

-- 6. Crear FK
IF  OBJECT_ID(N'FK_Productos_Proveedores', 'F') IS NULL
    BEGIN
        ALTER TABLE Productos
            ADD CONSTRAINT [FK_Productos_Proveedores] FOREIGN KEY(IdProveedor)
                REFERENCES Proveedores (IdProveedor)
    END
GO

-- SP: Verificar disponibilidad
CREATE OR ALTER PROCEDURE sp_VentaProductos
    @IdProducto  INT,
    @Cantidad    INT
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @MensajeError VARCHAR(2000)
    
    BEGIN TRY

        -- Validar sì el producto existe
        IF  NOT EXISTS(
                SELECT  1
                FROM    Productos
                WHERE   IdProducto = @IdProducto
            )
            BEGIN
                -- Validar Existencias del Producto
                SET @MensajeError = CONCAT('El producto ', @IdProducto, ' no existe. Por favor verifique')
                ;THROW 50001, @MensajeError, 1
            END
        
        -- Validar Existencias del Producto
        IF  EXISTS(
                SELECT  1
                FROM    Productos
                WHERE   IdProducto  =   @IdProducto
                        AND Stock   <   @Cantidad
            )
            BEGIN
                -- Validar Existencias del Producto
                SET @MensajeError = 'La cantidad pedida supera las existencias del producto. La venta no puede ser completada'
                ;THROW 50002, @MensajeError, 1
            END

        -- Efectuar la disminuciòn de existencias
        UPDATE  Productos
        SET     Stock = Stock - @Cantidad
        WHERE   IdProducto = @IdProducto
    END TRY
    BEGIN CATCH
        ;THROW;
    END CATCH
END
GO

-- Consulta validación
SELECT  *
FROM    Productos AS prod
        INNER JOIN Proveedores AS prov
            ON  prod.IdProveedor = prov.IdProveedor