[← Volver al índice](README.md)

# 5. Inventario y compras

**Ruta:** `/inventario` (menú **Gestión → Inventario**)

Esta pantalla muestra el historial de **compras a proveedores** y el estado de las existencias.

## 5.1 Indicadores superiores

Tres tarjetas resumen el inventario:

- 📦 **Total Registros** — cantidad de líneas de compra registradas.
- 🟡 **Stock Bajo** — productos en o bajo su mínimo (pero con existencias).
- 🔴 **Agotados** — productos sin existencias (stock en cero o menos).

## 5.2 La tabla de compras

Cada fila corresponde a un producto ingresado en una compra:

| Columna | Significado |
|---------|-------------|
| **Proveedor** | Quién vendió la mercadería. |
| **Fecha** | Fecha de la compra. |
| **Producto** | Nombre y SKU (número interno). |
| **Cantidad** | Unidades ingresadas. |
| **Costo Unit.** | Precio que pagaste por unidad. |
| **Precio Venta** | Precio actual de venta al público. |
| **Ganancia** | Diferencia entre precio de venta y costo. En rojo si es negativa. |

Usa el cuadro **Buscar** para filtrar por nombre de producto, proveedor o SKU.

## 5.3 Registrar una compra

Cuando recibes mercadería de un proveedor:

1. Presiona **Registrar Compra** (arriba a la derecha).
2. En la ventana, completa los **datos generales**:
   - **Proveedor** — elígelo de la lista. *(Si no aparece, agrégalo primero en [Configuración → Proveedores](08-configuracion.md).)*
   - **Fecha** — fecha de la compra.
   - **Notas** — observaciones (opcional).
3. **Agrega los productos** de la compra, uno por uno:
   - Elige el **Producto**.
   - Indica la **Cantidad** y el **Precio** unitario de compra.
   - Presiona **Añadir Producto**. Aparecerá en la tabla con su subtotal.
   - Repite para cada producto. Puedes quitar una línea con el ícono 🗑️.
4. Revisa el **Total de la compra** (se calcula solo).
5. Presiona **Finalizar Compra**.

### Qué ocurre al finalizar
- Se guarda la compra con todos sus productos.
- El **stock de cada producto aumenta** automáticamente según la cantidad comprada.
- Se crea un registro **INSERT** sobre `CompraInventario` en los [Logs](09-logs.md).

> ⚠️ No puedes finalizar si no seleccionaste proveedor o si no agregaste al menos un producto.

## 5.4 Diferencia entre "Registrar Compra" e "Incrementar inventario"

| | Registrar Compra | Incrementar inventario |
|---|---|---|
| Dónde está | Inventario | Productos |
| Aumenta stock | ✅ | ✅ |
| Guarda proveedor | ✅ | ❌ |
| Guarda costo/precio | ✅ | ❌ |
| Ideal para | Ingreso real de mercadería | Ajustes rápidos o conteos |

---

[Siguiente: Ventas →](06-ventas.md)
