[← Volver al índice](README.md)

# 3. Dashboard

**Ruta:** `/` (pantalla de inicio, opción **Dashboard** del menú)

El Dashboard es la pantalla principal: resume el estado del negocio y te deja **filtrar la información por fechas**.

## 3.1 Encabezado y contexto

Arriba a la izquierda verás el título **Dashboard** y dos "chips" con totales generales del negocio (no dependen de la fecha):

- 📦 **Productos** — cantidad total de productos registrados.
- 🏷️ **Categorías** — cantidad total de categorías.

Arriba a la derecha está el **filtro por fechas** y, debajo, el período que estás viendo actualmente.

## 3.2 Filtro por fechas

El selector de la derecha permite elegir el período que quieres analizar:

| Opción | Muestra |
|--------|---------|
| **Todos los períodos** | Toda la información histórica |
| **Hoy** | Solo el día actual |
| **Ayer** | El día anterior |
| **Últimos 15 días** | Los últimos 15 días |
| **Esta semana / Semana anterior** | La semana en curso o la pasada |
| **Este mes / Mes anterior** | El mes en curso o el pasado |
| **Este año** | Desde el 1 de enero |
| **Personalizado** | Un rango entre dos fechas que tú eliges |

También puedes escribir directamente las **dos fechas** (inicio y fin) en los campos de calendario.

> Todo lo que aparece marcado como **"del período"** cambia según el filtro que elijas.

## 3.3 Tarjetas de estadísticas (del período)

Cuatro tarjetas resumen el desempeño del período seleccionado:

- 🟢 **Ventas del período** — total vendido (₡).
- 🔴 **Compras del período** — total gastado en compras a proveedores (₡).
- 🟡 **Ganancia del período** — ventas menos compras. Se muestra en rojo si es negativa.
- 🟣 **N.º de ventas** — cantidad de ventas registradas.

## 3.4 Tablas de resumen

### Productos con bajo stock *(estado actual)*
Lista los productos cuyo stock está en o por debajo del mínimo. Esta tabla siempre muestra el **estado actual**, sin importar el filtro de fechas, porque el stock es un dato del momento.

### Productos más vendidos *(del período)*
Ranking de los 5 productos con más unidades vendidas en el período elegido.

### Ventas por empleado *(del período)*
Barras que comparan cuánto vendió cada empleado en el período. La barra más larga corresponde al empleado con mayor monto.

> 💡 Las tarjetas y tablas tienen **altura fija**; si una lista es larga, aparece una **barra de desplazamiento amarilla** dentro de la tarjeta y el encabezado de la tabla permanece visible mientras te desplazas.

---

[Siguiente: Productos →](04-productos.md)
