[← Volver al índice](README.md)

# 10. Preguntas frecuentes y solución de problemas

## Uso general

**¿Cómo cambio entre modo claro y oscuro?**
Con el botón de **sol/luna** en la barra superior. Tu elección se recuerda automáticamente. Ver [Acceso y navegación](02-acceso-y-navegacion.md#25-modo-diurno-y-nocturno).

**¿Olvidé mi contraseña, qué hago?**
Las cuentas las gestiona el administrador del sistema. Contáctalo para restablecerla.

**¿Los montos en qué moneda están?**
En **colones costarricenses (₡)**.

## Ventas e inventario

**Vendí un producto pero el stock no bajó.**
El stock disminuye al presionar **Cobrar Venta**. Si no cambió, verifica en [Logs](09-logs.md) que la venta se haya registrado.

**¿Cómo corrijo el stock de un producto?**
- Para **sumar** unidades rápido: **Productos → Incrementar inventario**.
- Para **restar** unidades (devolución): **Productos → Devolución a proveedor**.
- Para un ingreso formal con proveedor y costo: **Inventario → Registrar Compra**.
Ver [Productos](04-productos.md) e [Inventario](05-inventario-y-compras.md).

**No aparece mi proveedor al registrar una compra.**
Primero regístralo en **Configuración → Proveedores**. Ver [Configuración](08-configuracion.md).

**¿Cómo marco como pagada una venta fiada?**
En **Ventas**, busca la venta y presiona **Marcar Pagado**. También puedes seleccionar varias y usar **Marcar pagadas** en lote.

**¿Puedo exportar mis ventas?**
Sí, con **Exportar Excel** en la pantalla de Ventas, o el botón **CSV** en el detalle de un empleado.

## Dashboard

**El total de ventas del dashboard no coincide con lo que esperaba.**
Revisa el **filtro por fechas**: las tarjetas muestran solo el período seleccionado. Elige **Todos los períodos** para ver el histórico completo.

**Las fechas cercanas a la medianoche parecen caer en otro día.**
Las fechas se comparan por día. Si notas desfases, coméntalo al administrador para ajustar la zona horaria del sistema.

## Problemas técnicos

**Al iniciar la aplicación aparece: *"Una directiva de Control de aplicaciones bloqueó este archivo (0x800711C7)"*.**
Esto **no es un problema del sistema**, sino de **Windows**, que está bloqueando la aplicación por seguridad (**Smart App Control** o una política **WDAC/AppLocker**). Soluciones:
1. Ve a **Seguridad de Windows → Control de aplicaciones y explorador → Configuración de Control inteligente de aplicaciones** y cámbialo a **Desactivado**.
   *(Nota: una vez desactivado no se puede volver a activar sin reinstalar Windows.)*
2. Si es un equipo de la empresa y la opción está bloqueada, pide a **IT** que agregue una excepción.

**Aparece un error en una pantalla ("Algo salió mal").**
El sistema muestra una pantalla amigable con pasos: presiona **Reintentar**, luego recarga (F5), y si persiste cierra sesión y vuelve a entrar. Si el error continúa, contacta a soporte indicando la hora en que ocurrió.

**La página parece "congelada" tras iniciar.**
Recarga con **F5**. Si no responde, verifica tu conexión al servidor.

## Contacto y soporte

Ante cualquier problema que no puedas resolver con esta guía, contacta al **administrador del sistema** o a **soporte técnico**, indicando:
- Qué estabas haciendo.
- La **hora** en que ocurrió.
- El mensaje de error exacto (si apareció alguno).

---

[Siguiente: Glosario →](11-glosario.md)
