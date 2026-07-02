[← Volver al índice](README.md)

# 9. Logs del sistema

**Ruta:** `/logs` (menú **Sistema → Logs**)

Los **Logs** son el registro de auditoría del sistema: guardan automáticamente **quién hizo qué, cuándo y qué cambió**. Sirven para dar seguimiento a las operaciones y detectar errores o cambios indebidos.

## 9.1 Qué se registra

Cada vez que ocurre una operación importante, se guarda una fila con:

| Columna | Significado |
|---------|-------------|
| **Fecha** | Fecha y hora del cambio. |
| **Usuario** | Quién lo hizo (o "Sistema" si fue automático). |
| **Acción** | Tipo de operación (ver colores abajo). |
| **Tabla** | A qué parte del sistema afectó (Producto, Venta, Categoría…). |
| **Registro** | Identificador del elemento afectado. |
| **Datos Anteriores** | Cómo estaba **antes** del cambio (formato JSON). |
| **Datos Nuevos** | Cómo quedó **después** (formato JSON). |

## 9.2 Colores de las acciones

Para leer más rápido, cada acción tiene un color:

- 🟢 **Verde** — creaciones e ingresos: `INSERT`, `INCREMENTO_INVENTARIO`.
- 🔵 **Azul** — modificaciones: `UPDATE`.
- 🔴 **Rojo** — eliminaciones y salidas: `DELETE`, `DEVOLUCION_PROVEEDOR`.
- ⚪ **Neutro** — otras acciones (por ejemplo, exportaciones).

## 9.3 Buscar y navegar

- **Buscar:** escribe un usuario, acción o tabla en el cuadro de búsqueda para filtrar. La búsqueda se aplica sola tras un instante.
- **Paginación:** usa **← Anterior** y **Siguiente →** para recorrer los registros. Arriba a la derecha se indica el total de registros.

## 9.4 Ejemplos de uso

- *"¿Quién bajó el stock de este producto?"* → busca por el nombre del producto o por `DEVOLUCION` / `INCREMENTO`.
- *"¿Qué se eliminó ayer?"* → busca `DELETE` y revisa las fechas.
- *"¿Qué cambió en una venta?"* → busca `Venta` y compara **Datos Anteriores** vs **Datos Nuevos**.

> 💡 Los cuadros de **Datos Anteriores/Nuevos** muestran el contenido en formato JSON, con su propia barra de desplazamiento si el texto es largo.

---

[Siguiente: Preguntas frecuentes →](10-preguntas-frecuentes.md)
