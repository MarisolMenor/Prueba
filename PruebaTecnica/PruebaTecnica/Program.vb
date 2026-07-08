Imports System

Module Program
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
        Dim jsonDeEntrada As String = "{ ""Nombre"": ""Monitor ASUS 24"""""", ""Cantidad"": 10 }"

        Console.WriteLine("Procesando JSON de entrada...")
        IncrementarStockDesdeJSON(jsonDeEntrada)

        ' Validación final: Volvemos a consultar el producto para ver si el stock subió
        Console.WriteLine("" & vbCrLf & "Verificando stock actualizado en la BD:")
        MostrarDetalleProducto(2) ' El monitor debería reflejar el cambio si el método de la Fase 2 lee el stock

        Console.WriteLine("" & vbCrLf & "Presiona ENTER para salir...")
        Console.ReadLine()
    End Sub

End Module
