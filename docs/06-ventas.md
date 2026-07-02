[← Volver al índice](README.md)

# 6. Ventas

Esta área tiene dos pantallas: **Nueva Venta** (para cobrar) y **Ventas** (historial).

---

## 6.1 Registrar una nueva venta

**Ruta:** `/nueva-venta` (menú **Nueva Venta** o el botón 🛒 de la barra superior)

### Paso a paso

1. **Agregar productos al carrito:**
   - Elige un producto del desplegable (muestra el stock disponible).
   - Indica la **cantidad**.
   - Presiona el botón **+**.
   - El producto aparece en la tabla del carrito.
2. **Ajustar cantidades:** dentro del carrito puedes usar **−** y **+** para cambiar la cantidad de cada línea. No puedes superar el stock disponible. Para quitar un producto, presiona la **✕**.
3. **Asignar empleado** *(opcional):* en el panel derecho, elige el empleado que atiende la venta (o déjalo en "Sin asignar").
4. **Elegir método de pago:**
   - 💵 **Efectivo**
   - 📱 **SINPE**
   - 💳 **Tarjeta**
   - 📋 **Fiado** (queda pendiente de pago)
5. **Si es efectivo:** escribe el **Monto recibido** y el sistema calcula el **Cambio** automáticamente.
6. Revisa el **Total** (arriba a la derecha) y presiona **Cobrar Venta**.

### Qué ocurre al cobrar
- Se guarda la venta con la fecha y hora.
- El **stock de cada producto disminuye** según lo vendido.
- La venta queda disponible en el **historial** y se le puede imprimir recibo.

> 💡 El botón **Cobrar Venta** permanece deshabilitado hasta que haya al menos un producto en el carrito.

---

## 6.2 Historial de ventas

**Ruta:** `/ventas` (menú **Ventas**)

### Indicadores del día
Cuatro tarjetas muestran, para **hoy**:
- 📈 **Ventas de Hoy** — total vendido.
- 💰 **Cobrado** — lo cobrado (todo menos fiados).
- 🧾 **Fiados** — total en ventas fiadas.
- 🧮 **Transacciones** — número de ventas.

### Buscar y filtrar
- **Buscar:** por número de folio o por nombre de empleado.
- **Rango de fechas:** con el selector de fechas.
- **Filtros rápidos:** **Todas**, **Fiados**, **Hoy**.
- **Método de pago:** desplegable para ver solo Efectivo, SINPE, Tarjeta o Fiado.

### La tabla de ventas
Cada fila muestra: **Folio**, **Fecha/Hora** (la fecha aparece en formato largo, ej. *Lunes 3 Marzo 26*), **Empleado**, **Método** de pago (con etiqueta de color), **Total** y **Estado**.

### Estado de pago
- Una venta puede estar **Pagado** (verde) o **Pendiente**.
- Si está pendiente, verás el botón **Marcar Pagado** para registrar el cobro.

### Acciones por fila
- 👁️ **Ver** el recibo.
- ✏️ **Editar** la venta.
- 🖨️ **Imprimir**.

### Selección múltiple
Marca varias ventas con las casillas para:
- **Marcar pagadas** en lote.
- **Exportar selección** a Excel.
- **Limpiar** la selección.

### Exportar a Excel
El botón **Exportar Excel** genera un archivo con dos hojas:
- **Ventas** — detalle de cada venta.
- **Resumen** — separadas en pendientes y pagadas, con totales.

### Paginación
Abajo puedes cambiar la **cantidad de filas** (10, 25, 50, 100) y navegar entre páginas.

---

## 6.3 Ver e imprimir el recibo

Al presionar 👁️ **Ver** en una venta (`/venta/{número}`), se muestra el **recibo** con el detalle de productos, totales y datos de la venta.

- Presiona **Imprimir** para enviarlo a la impresora.
- El recibo se **adapta al tema** (claro/oscuro) dentro de la aplicación, pero al **imprimir siempre sale en papel blanco con texto negro**, para que se lea bien.

---

## 6.4 Editar una venta

Al presionar ✏️ **Editar** (`/venta/editar/{número}`) puedes corregir los datos de una venta ya registrada. Guarda los cambios para aplicarlos; la modificación queda registrada en los [Logs](09-logs.md).

---

[Siguiente: Empleados →](07-empleados.md)
