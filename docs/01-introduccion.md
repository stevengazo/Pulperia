[← Volver al índice](README.md)

# 1. Introducción

## ¿Qué es PulpePOS?

PulpePOS es un **sistema de punto de venta y administración** pensado para pulperías y pequeños comercios. Con él puedes:

- Registrar **ventas** y cobrar por distintos métodos de pago (efectivo, SINPE, tarjeta, fiado).
- Llevar el **inventario** al día: cada compra a proveedor suma stock y cada venta lo resta automáticamente.
- Administrar el **catálogo de productos**, sus precios de costo y de venta.
- Registrar **devoluciones a proveedor** e **ingresos manuales de inventario**.
- Consultar **estadísticas** de ventas, ganancias y desempeño por empleado.
- Mantener un **registro de auditoría** (logs) de todos los cambios importantes.

## Conceptos básicos

| Término | Significado |
|---------|-------------|
| **Producto** | Un artículo que vendes (nombre, presentación, precio de costo, precio de venta, stock). |
| **Stock** | Cantidad de unidades disponibles de un producto. |
| **Stock mínimo** | Nivel de alerta: si el stock baja de aquí, el producto se marca como "bajo stock". |
| **Compra** | Ingreso de mercadería desde un proveedor. Aumenta el stock. |
| **Venta** | Salida de mercadería a un cliente. Disminuye el stock. |
| **Fiado** | Venta a crédito, pendiente de pago. |
| **Auditoría / Log** | Registro automático de quién hizo cada cambio y cuándo. |

## Requisitos para usarlo

- Un **navegador web** moderno (Chrome, Edge o Firefox actualizados).
- Una **cuenta de usuario** (correo y contraseña) proporcionada por el administrador.
- Conexión al servidor donde está instalado el sistema.

No necesitas instalar nada en tu computadora: PulpePOS funciona desde el navegador.

## Cómo está organizado el sistema

El sistema se recorre desde el **menú lateral** izquierdo, dividido en tres grupos:

- **PRINCIPAL:** Dashboard · Ventas · Nueva Venta
- **GESTIÓN:** Productos · Inventario · Empleados
- **SISTEMA:** Configuración · Logs

Cada una de estas pantallas se explica en detalle en las siguientes secciones.

---

[Siguiente: Acceso y navegación →](02-acceso-y-navegacion.md)
