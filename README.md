# Prueba Tecnica
# 💻 Prueba Técnica Backend: VB.NET & SQL Server

¡Bienvenido/a al proceso de selección! Esta prueba técnica está diseñada para evaluar tus habilidades de desarrollo backend en entornos corporativos utilizando **.NET Framework 4.8** y **SQL Server**. 

La evaluación se realizará con una duración estimada de **45 a 60 minutos**.

---

## 🛠️ Requisitos Previos (Preparación del Entorno)
Para optimizar el tiempo de la sesión, hemos realizado los siguientes pasos antes de iniciar la prueba:

1. Tener acceso a una instancia local de **SQL Server** y tener instalado **SQL Server Management Studio (SSMS)**.
2. Entorno de desarrollo preferido (**Visual Studio 2019/2022**) e instalado el SDK de **.NET Framework 4.8**.
3. Script de base de datos que se detalla a continuación.

### 🗄️ Script de Base de Datos (SQL Server)
Ejecuta este código en tu servidor local para crear la base de datos de prueba y la tabla con la que trabajaremos:

```sql
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
```

---

## 📋 El Desafío (Live Coding)

Durante la prueba, deberás crear una **Aplicación de Consola en VB.NET (.NET Framework 4.8)** e implementar la lógica de negocio para el siguiente escenario:

### Escenario: Control de Stock e Integración con BD
Necesitamos construir un método llamado `ActualizarStockProducto` que se encargue de procesar una venta en el sistema restando las existencias de la base de datos de forma segura.

### Requerimientos Técnicos:
1. **Firma del método:** El método debe recibir `idProducto (Integer)` y `cantidadVendida (Integer)`, devolviendo un valor booleano (`True` si la venta se procesó, `False` si falló).
2. **Persistencia Real:** Debe conectarse a la base de datos local `EntrevistaBackend`.
3. **Regla de Negocio:** La venta no puede realizarse si la cantidad solicitada es mayor al stock disponible en la base de datos.
4. **Validación en Consola:** En el método `Main()`, deberás invocar la función simulando dos escenarios:
   - Una venta exitosa (ej: restar 2 unidades al producto con ID 1).
   - Una venta fallida por falta de stock (ej: intentar restar 6 unidades al producto con ID 2).

---

## 🎯 ¿Qué evaluaremos?

En esta prueba no buscamos una solución compleja, sino la aplicación de buenas prácticas de ingeniería de software para un perfil Semi-Senior:

* **Manejo Eficiente de Recursos:** Uso correcto de la liberación de conexiones a la base de datos (ciclo de vida de objetos en .NET).
* **Seguridad:** Prevención estricta contra vulnerabilidades comunes (Inyección SQL).
* **Control de Excepciones:** Robustez del código ante fallos de conexión o errores en tiempo de ejecución.
* **Manejo de Concurrencia básica:** Estrategia elegida para evitar condiciones de carrera (*Race Conditions*) al descontar stock.

---
*¡Mucho éxito en la prueba!*
