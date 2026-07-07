# Checklist SQL Server controlado

Usa esta lista para validar manualmente `VoidEmpires_Dev`. No pegues usuarios, passwords ni cadenas reales en commits, chats, tickets o documentos.

Estado Block 41: la limpieza de UI producto no cambia este flujo. SQL Server sigue siendo una validacion manual/controlada; no se debe asumir que `VoidEmpires_Dev` existe, no se debe aplicar el script generado automaticamente y no se deben commitear cadenas resueltas ni passwords reales. Este repositorio no registra como completados el apply manual del esquema ni una aceptacion de `cockpit-validation` contra SQL Server; tratalos como pendientes salvo evidencia privada del operador fuera del repo.

- [ ] Crear `VoidEmpires_Dev` manualmente en SSMS usando `scripts/sqlserver/create-database.sql` despues de revisarlo.
- [ ] Configurar la cadena local fuera del repo con `User Id=<USER>` y `Password=<PASSWORD>`.
- [ ] Ejecutar el smoke de conexion `SELECT 1` con `scripts/sqlserver-connection-smoke.ps1`.
- [ ] Confirmar que `dotnet test --no-build` normal no requiere SQL Server.
- [ ] Generar el script de migracion solo cuando exista `SqlServerInitialBaseline`.
- [ ] Ejecutar `scripts/check-sqlserver-generated-script-safety.ps1` contra el script generado.
- [ ] Hacer backup manual de `VoidEmpires_Dev` en SSMS antes de aplicar cambios de esquema.
- [ ] Abrir `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql` en SSMS y revisar todo el contenido antes de ejecutarlo.
- [ ] Verificar manualmente que la conexion y la base activa apuntan a `VoidEmpires_Dev`.
- [ ] Aplicar el script manualmente solo si el operador lo aprueba; los scripts del repositorio no aplican la migracion automaticamente.
- [ ] Inspeccionar tablas creadas y consultar `dbo.__EFMigrationsHistory`.
- [ ] Ejecutar de nuevo el smoke SQL Server opcional contra `VoidEmpires_Dev`.
- [ ] Ejecutar la app local con `VoidEmpires__Persistence__Provider="SqlServer"` solo despues de que el esquema exista.
- [ ] Verificar `GET http://localhost:5142/health`: `persistence.configured=true` y `persistence.provider=Microsoft.EntityFrameworkCore.SqlServer`.
- [ ] Antes de seed, asumir que las tablas de gameplay pueden estar vacias y que algunas vistas de catalogo/readiness pueden no tener opciones.
- [ ] Despues de un seed aprobado solo para una base disposable, verificar que los endpoints dev reflejan el escenario sembrado.
- [ ] Confirmar que las herramientas internas/operador siguen ocultas por defecto en la UI producto y que cualquier diagnostico tecnico se revisa como soporte, no como gameplay final.
- [ ] Ejecutar el dry-run de catalogos finales sin `-Apply` y confirmar que no borra datos de gameplay.
- [ ] Limpiar variables locales: `ASPNETCORE_ENVIRONMENT`, `VoidEmpires__Persistence__Provider`, `ConnectionStrings__DefaultConnection`, `VOIDEMPIRES_CONNECTION_STRING`, `VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED`, `VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING`.

Referencia detallada: `docs/dev/sql-server-runbook.md`.
