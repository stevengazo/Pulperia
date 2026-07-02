[← Volver al índice](README.md)

# 7. Empleados

**Ruta:** `/empleados` (menú **Gestión → Empleados**)

Esta pantalla lista los empleados **activos** y permite consultar el desempeño de cada uno.

## 7.1 Lista de empleados

La tabla muestra cada empleado con su **foto** (o un ícono si no tiene), **nombre**, **apellido** y un identificador **EMP-#**.

- **Buscar:** escribe nombre o apellido en el cuadro de búsqueda.
- El contador de la derecha indica cuántos empleados coinciden.
- Presiona el ícono 👁️ para ver el **detalle** del empleado.

## 7.2 Detalle e historial de un empleado

**Ruta:** `/empleado/{id}`

Al abrir un empleado verás:

### Encabezado
Su foto, nombre completo e identificador.

### Estadísticas
- **Ventas** — cantidad de ventas asociadas al empleado.
- **Total Vendido** — monto total (₡).

Ambas cifras se ajustan al **filtro de fechas** que elijas.

### Filtro por fechas y exportación
- Usa el **selector de fechas** para acotar el período.
- Presiona el botón **CSV** (📄) para **exportar** las ventas del período a un archivo `.csv` (útil para abrir en Excel).

### Tabla de ventas
Lista cada venta del empleado con su **número**, **fecha y hora**, **total** y acciones:
- Generar/descargar el **ticket** de la venta.
- **Ver recibo →** para abrir el detalle completo.

> ℹ️ Los empleados se administran desde el sistema de cuentas. Si necesitas **agregar o dar de baja** empleados, contacta al administrador.

---

[Siguiente: Configuración →](08-configuracion.md)
