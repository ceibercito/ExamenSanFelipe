# Examen Técnico IT — Sistema de Compras y Ventas

Solución completa al examen técnico: **.NET 8 (microservicios) + Angular + SQL Server + Docker**.

> Autor: Ceiber Conrado Garibay Choque

---

## Estructura del proyecto

```
examen-tecnico-it/
├── 01-base-de-datos/      # Scripts SQL Server (tablas, datos, CRUD, stock/kardex)
├── 02-backend-dotnet/     # Microservicios .NET 8 (Auth, Compras, Ventas, Movimientos)
├── 03-frontend-angular/   # App Angular (compra, venta, kardex) con interceptor JWT
├── 04-docker/             # docker-compose: SQL Server + microservicios + frontend
└── 05-entregables/        # Teoría (30%), guión del video, diagrama, Postman
```

---

## Roadmap por fases

| Fase | Contenido | Estado |
| ---- | --------- | ------ |
| **1** | Base de datos SQL Server (modelo, datos, scripts CRUD, lógica de stock) | ✅ |
| **2** | Backend .NET 8 — microservicios, JWT, EF Core, Swagger, CORS, SOLID, Facade/Decorator | ✅ |
| **3** | Frontend Angular — login, interceptor JWT, vistas Compra/Venta/Kardex, Bootstrap | ✅ |
| **4** | Docker — orquestación de todo el stack | ✅ |
| **5** | Entregables — teoría 30%, guión video, diagrama arquitectura, Postman | ✅ |

---

## Arquitectura (resumen)

Microservicios .NET 8 que comparten una base de datos SQL Server (patrón *shared-database microservices*), cada uno con su propia API, Swagger y CORS:

- **Auth.API** — autenticación y emisión de JWT (30 min de duración).
- **Compras.API** — gestión de productos + registro/listado de compras. Al comprar: actualiza costo y precio de venta del producto (PrecioVenta = Costo × 1.35) e inserta un Movimiento de tipo **Entrada**.
- **Ventas.API** — registro/listado de ventas. Valida stock disponible (leído de Movimientos) e inserta un Movimiento de tipo **Salida**.
- **Movimientos.API** — Kardex: stock actual por producto y detalle de movimientos.

El frontend Angular consume estos servicios usando un **interceptor** que agrega el token JWT en cada petición.

---

## Cómo ejecutar (resumen — detalle en cada fase)

```bash
cd examen-tecnico-it/04-docker
docker compose up --build
```

Servicios (puertos):
- Frontend Angular: http://localhost:4400
- Auth.API / Swagger: http://localhost:5001/swagger
- Compras.API / Swagger: http://localhost:5002/swagger
- Ventas.API / Swagger: http://localhost:5003/swagger
- Movimientos.API / Swagger: http://localhost:5004/swagger
- SQL Server: localhost:1433 (sa / Tu_Password123)

Usuario de prueba: **admin / admin123**

---

## Lógica de negocio clave

**Stock de un producto** = suma de entradas − suma de salidas (calculado desde la tabla Movimiento):

```
stock = Σ(MovimientoDet.Cantidad donde TipoMovimiento = 1 Entrada)
      − Σ(MovimientoDet.Cantidad donde TipoMovimiento = 2 Salida)
```

**Al registrar una Compra:**
1. Inserta CompraCab + CompraDet.
2. Actualiza el producto: Costo = precio de compra, PrecioVenta = Costo × 1.35.
3. Inserta MovimientoCab (Entrada) + MovimientoDet.

**Al registrar una Venta:**
1. Valida que la cantidad no supere el stock disponible.
2. Calcula SubTotal = Cantidad × PrecioVenta; Igv = SubTotal × 0.18; Total = SubTotal + Igv.
3. Inserta VentaCab + VentaDet.
4. Inserta MovimientoCab (Salida) + MovimientoDet.

> Nota sobre el IGV: el examen escribe "Igv (cantidad * Precio Venta * 1.18)". Esa fórmula da el total con IGV incluido, no el IGV solo. En la solución se interpreta de la forma contablemente correcta: IGV = base × 0.18. Esto se explica en el documento de teoría y en el guión del video para que puedas justificarlo.
```
