Imports System
Imports Microsoft.Data.SqlClient
Imports System.Text.Json
Module Program
    Private connString As String = "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EntrevistaBackend;Integrated Security=True"

    Sub Main()
        ' =========================================================================
        ' PRUEBAS FASE 1: Control de Stock (Ventas)
        ' =========================================================================
        Console.WriteLine("=== FASE 1: PROBANDO VENTAS ===")

        ' Escenario A: Venta exitosa (Comprar 2 Laptops de las 10 disponibles)
        Dim ventaOk As Boolean = ActualizarStockProducto(1, 2)
        Console.WriteLine($"¿Venta de Laptop exitosa?: {ventaOk}") ' Esperado: True

        ' Escenario B: Venta fallida (Intentar comprar 6 Monitores cuando solo hay 5)
        Dim ventaFallida As Boolean = ActualizarStockProducto(2, 6)
        Console.WriteLine($"¿Venta de Monitor exitosa?: {ventaFallida}") ' Esperado: False
        Console.WriteLine()

        ' =========================================================================
        ' PRUEBAS FASE 2: Migración y Consulta Relacional (INNER JOIN)
        ' =========================================================================
        Console.WriteLine("=== FASE 2: MIGRACIÓN Y CONSULTA RELACIONAL ===")

        ' Primero se ejecuta la migración programática (crea tabla proveedores si no existe)
        EjecutarMigracionProveedores()

        ' Consultamos el detalle del producto ID 1 (Debe traer su proveedor 'Dell LATAM')
        MostrarDetalleProducto(1)
        Console.WriteLine()

        ' =========================================================================
        ' PRUEBAS FASE 3: Reabastecimiento desde JSON (Incrementar Stock)
        ' =========================================================================
        Console.WriteLine("=== FASE 3: INCREMENTO DE STOCK DESDE JSON ===")

        ' DECLARACIÓN DEL JSON: Nota cómo se escapan las comillas usando "" en VB.NET
        ' El JSON equivale a: {"Nombre": "Monitor ASUS 24\"", "Cantidad": 10}
        Dim jsonDeEntrada As String = "{ ""Nombre"": ""Monitor ASUS 24\"""", ""Cantidad"": 10 }"

        Console.WriteLine("Procesando JSON de entrada...")
        IncrementarStockDesdeJSON(jsonDeEntrada)

        ' Validación final: Volvemos a consultar el producto para ver si el stock subió
        Console.WriteLine("" & vbCrLf & "Verificando stock actualizado en la BD:")
        MostrarDetalleProducto(2) ' El monitor debería reflejar el cambio si el método de la Fase 2 lee el stock

        Console.WriteLine("" & vbCrLf & "Presiona ENTER para salir...")
        Console.ReadLine()
    End Sub
    Function ActualizarStockProducto(idProducto As Integer, cantidadVendida As Integer) As Boolean
        Dim filasAfectadas As Integer = 0

        Try
            Using conn As New SqlConnection(connString)
                conn.Open()
                Dim query As String = "UPDATE Productos SET Stock = Stock - @Cantidad WHERE IdProducto = @Id AND Stock >= @Cantidad"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Cantidad", cantidadVendida)
                    cmd.Parameters.AddWithValue("@Id", idProducto)
                    filasAfectadas = cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As SqlException
            Console.WriteLine($"Error al actualizar stock: {ex.Message}")
            Return False
        End Try

        Return filasAfectadas > 0
    End Function
    Sub EjecutarMigracionProveedores()
        Try
            Using conn As New SqlConnection(connString)
                conn.Open()

                Dim crearTablaProveedores As String = "
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Proveedores')
            BEGIN
                CREATE TABLE Proveedores (
                    IdProveedor INT IDENTITY(1,1) PRIMARY KEY,
                    Nombre VARCHAR(100) NOT NULL
                )
            END"
                Using cmd As New SqlCommand(crearTablaProveedores, conn)
                    cmd.ExecuteNonQuery()
                End Using

                Dim agregarColumna As String = "
            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = 'Productos' AND COLUMN_NAME = 'IdProveedor'
            )
            BEGIN
                ALTER TABLE Productos ADD IdProveedor INT NULL
                ALTER TABLE Productos ADD CONSTRAINT FK_Productos_Proveedores
                    FOREIGN KEY (IdProveedor) REFERENCES Proveedores(IdProveedor)
            END"
                Using cmd As New SqlCommand(agregarColumna, conn)
                    cmd.ExecuteNonQuery()
                End Using

                Dim seedProveedor As String = "
            IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Dell LATAM')
            BEGIN
                INSERT INTO Proveedores (Nombre) VALUES ('Dell LATAM')
                UPDATE Productos SET IdProveedor = SCOPE_IDENTITY() WHERE IdProducto = 1
            END"
                Using cmd As New SqlCommand(seedProveedor, conn)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            Console.WriteLine("Migración de proveedores ejecutada correctamente.")

        Catch ex As SqlException
            Console.WriteLine($"Error de base de datos durante la migración: {ex.Message}")
        Catch ex As Exception
            Console.WriteLine($"Error inesperado durante la migración: {ex.Message}")
        End Try
    End Sub

    Sub MostrarDetalleProducto(idProducto As Integer)
        Try
            Using conn As New SqlConnection(connString)
                conn.Open()
                Dim query As String = "
            SELECT P.Nombre, P.Stock, P.Precio, ISNULL(S.Nombre, 'Sin proveedor asignado') AS Proveedor
            FROM Productos P
            LEFT JOIN Proveedores S ON P.IdProveedor = S.IdProveedor
            WHERE P.IdProducto = @Id"

                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@Id", idProducto)
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Console.WriteLine($"Producto: {reader("Nombre")} | Stock: {reader("Stock")} | Precio: {reader("Precio"):C} | Proveedor: {reader("Proveedor")}")
                        Else
                            Console.WriteLine($"No se encontró el producto con Id {idProducto}")
                        End If
                    End Using
                End Using
            End Using

        Catch ex As SqlException
            Console.WriteLine($"Error de base de datos al consultar el producto: {ex.Message}")
        Catch ex As Exception
            Console.WriteLine($"Error inesperado al consultar el producto: {ex.Message}")
        End Try
    End Sub
    Sub IncrementarStockDesdeJSON(json As String)
        Try
            Using doc As JsonDocument = JsonDocument.Parse(json)
                Dim root As JsonElement = doc.RootElement
                Dim nombreProducto As String = root.GetProperty("Nombre").GetString()
                Dim cantidad As Integer = root.GetProperty("Cantidad").GetInt32()

                Using conn As New SqlConnection(connString)
                    conn.Open()
                    Dim query As String = "UPDATE Productos SET Stock = Stock + @Cantidad WHERE Nombre = @Nombre"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@Cantidad", cantidad)
                        cmd.Parameters.AddWithValue("@Nombre", nombreProducto)
                        Dim filas As Integer = cmd.ExecuteNonQuery()

                        If filas > 0 Then
                            Console.WriteLine($"Stock incrementado en {cantidad} unidades para '{nombreProducto}'.")
                        Else
                            Console.WriteLine($"No se encontró ningún producto llamado '{nombreProducto}'.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As JsonException
            Console.WriteLine($"Error al parsear el JSON: {ex.Message}")
        Catch ex As SqlException
            Console.WriteLine($"Error de base de datos: {ex.Message}")
        Catch ex As Exception
            Console.WriteLine($"Error inesperado: {ex.Message}")
        End Try
    End Sub
End Module
