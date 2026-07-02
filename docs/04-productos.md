[← Volver al índice](README.md)

# 4. Productos

**Ruta:** `/productos` (menú **Gestión → Productos**)

El catálogo muestra todos los productos y sus indicadores comerciales.

## 4.1 La tabla de productos

Cada fila muestra:

| Columna | Significado |
|---------|-------------|
| **Producto** | Nombre y presentación (ej. "Arroz — 1 kg"). |
| **Categoría** | Categoría a la que pertenece. |
| **Unidades Compradas** | Total de unidades ingresadas por compras. |
| **Costo Total** | Cuánto se ha gastado comprando ese producto (₡). |
| **Unidades Vendidas** | Total de unidades vendidas. |
| **Ventas Total** | Cuánto se ha vendido de ese producto (₡). |
| **Stock** | Existencias actuales. 🟢 verde = suficiente, 🔴 rojo = en o bajo el mínimo. |
| **Acciones** | Editar (✏️) o eliminar (🗑️) el producto. |

### Buscar y filtrar
- **Buscar:** escribe nombre o presentación en el cuadro de búsqueda.
- **Categoría:** usa el desplegable para mostrar solo una categoría.

## 4.2 Crear un producto nuevo

1. Presiona **Nuevo producto** (botón naranja, arriba a la derecha).
2. Completa el formulario:
   - **Nombre** y **Presentación** (obligatorios).
   - **Costo** (lo que te cuesta) y **Venta** (precio al público).
   - **Stock actual** y **Stock mínimo**.
   - **Categoría** (elige una existente o crea una nueva con el botón **+**).
3. Presiona **Guardar**.

> 💡 **Crear categoría al vuelo:** en el campo Categoría, presiona **+**, escribe el nombre y confirma con ✓. La nueva categoría queda seleccionada de una vez.

## 4.3 Editar o eliminar un producto

- **Editar:** en la fila del producto, presiona el ícono ✏️. Se abre la pantalla de edición donde puedes cambiar cualquier dato y guardar.
- **Eliminar:** presiona 🗑️. El producto se elimina y la acción queda registrada en los [Logs](09-logs.md).

> ⚠️ Eliminar un producto es permanente. Hazlo solo si estás seguro.

## 4.4 Devolución a proveedor

Usa esta opción cuando **devuelves mercadería** al proveedor (producto dañado, vencido, etc.). **Resta** unidades del stock y deja registro de auditoría.

1. Presiona **Devolución a proveedor** (botón rojo).
2. En la ventana:
   - **Producto** — elige el producto (verás su stock actual).
   - **Cantidad** — cuántas unidades devuelves.
   - **Stock resultante** — el sistema muestra cómo quedará (`actual → nuevo`). Si quedaría negativo, se marca en rojo y no permite continuar.
   - **Motivo** — describe por qué (opcional pero recomendado).
3. Presiona **Registrar devolución**.

El stock disminuye y se crea un registro con la acción **DEVOLUCION_PROVEEDOR** en los Logs.

## 4.5 Incrementar inventario

Usa esta opción para **sumar unidades** al stock de forma manual (ajuste, conteo físico, ingreso puntual) sin registrar una compra completa.

1. Presiona **Incrementar inventario** (botón verde).
2. Elige el **Producto**, la **Cantidad** a sumar y un **Motivo**.
3. Presiona **Incrementar stock**.

El stock aumenta y se registra la acción **INCREMENTO_INVENTARIO** en los Logs.

> ℹ️ Para ingresos grandes de mercadería con proveedor y precios, usa mejor **[Registrar Compra](05-inventario-y-compras.md)**, que además guarda el costo y el proveedor.

### Validaciones de ambas ventanas
- Debes seleccionar un producto.
- La cantidad debe ser mayor que cero.
- En **devolución**, no puedes devolver más unidades de las que hay en stock.

---

[Siguiente: Inventario y compras →](05-inventario-y-compras.md)
