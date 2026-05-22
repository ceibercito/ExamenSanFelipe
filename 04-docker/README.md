# Docker — cómo levantar todo el sistema

Requisito único: **Docker Desktop**. No necesitas instalar .NET, SQL Server ni Node.

## Levantar

```bash
cd 04-docker
docker compose up --build
```

La primera vez tarda varios minutos: descarga SQL Server, compila los 4 microservicios .NET y crea/instala el proyecto Angular.

## Orden de arranque

1. **mssql** — SQL Server 2022 arranca.
2. **db-init** — espera a SQL Server, crea la BD `VentasComprasDB`, las tablas, el usuario `admin` y los productos de ejemplo. Luego termina (es un contenedor de un solo uso).
3. **auth / compras / ventas / movimientos** — los 4 microservicios .NET 8.
4. **frontend** — Angular en modo dev.

## URLs

| Servicio | URL |
| -------- | --- |
| Frontend (app) | http://localhost:4400 |
| Auth.API (Swagger) | http://localhost:5001/swagger |
| Compras.API (Swagger) | http://localhost:5002/swagger |
| Ventas.API (Swagger) | http://localhost:5003/swagger |
| Movimientos.API (Swagger) | http://localhost:5004/swagger |
| SQL Server | localhost:1433 (sa / Tu_Password123) |

Login de prueba: **admin / admin123**

## Comandos útiles

```bash
# En segundo plano
docker compose up -d --build

# Ver logs de un servicio
docker compose logs -f compras

# Re-ejecutar solo la inicialización de BD
docker compose run --rm db-init

# Apagar (conserva datos)
docker compose down

# Apagar y BORRAR la base de datos
docker compose down -v

# Reset total del frontend (si quedó a medias)
docker compose down
rm -rf ../03-frontend-angular/app
docker compose up --build
```

## Troubleshooting

**Los microservicios no conectan a la BD al inicio:** EF Core tiene `EnableRetryOnFailure`, así que reintenta solo mientras SQL Server termina de arrancar. Si persiste, revisa que el contenedor `db-init` haya terminado con "Base de datos inicializada correctamente".

**Error CORS en el navegador:** las APIs solo permiten el origen `http://localhost:4400`. Asegúrate de abrir el frontend exactamente en esa URL.

**El puerto está ocupado:** cambia el mapeo en `docker-compose.yml` (ej. `"5101:8080"`) y, si cambias el puerto de una API, actualiza también `03-frontend-angular/overlay/src/environments/environment.ts`.

**db-init falla con "sqlcmd not found":** el script detecta automáticamente la ruta. Si tu imagen no incluye las herramientas, ejecuta los scripts de `01-base-de-datos/` manualmente con Azure Data Studio o DBeaver conectándote a localhost:1433.
