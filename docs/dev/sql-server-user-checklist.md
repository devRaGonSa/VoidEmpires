# Checklist SQL Server controlado

Usa esta lista para validar manualmente `VoidEmpires_Dev`. No pegues usuarios, passwords ni cadenas reales en commits, chats, tickets o documentos.

- [ ] Crear `VoidEmpires_Dev` manualmente en SSMS usando `scripts/sqlserver/create-database.sql` despues de revisarlo.
- [ ] Configurar la cadena local fuera del repo con `User Id=<USER>` y `Password=<PASSWORD>`.
- [ ] Ejecutar el smoke de conexion `SELECT 1` con `scripts/sqlserver-connection-smoke.ps1`.
- [ ] Confirmar que `dotnet test --no-build` normal no requiere SQL Server.
- [ ] Generar el script de migracion solo cuando exista `SqlServerInitialBaseline`.
- [ ] Revisar el script SQL generado antes de abrirlo en SSMS.
- [ ] Aplicar el script manualmente solo si el operador lo aprueba.
- [ ] Ejecutar de nuevo el smoke SQL Server opcional contra `VoidEmpires_Dev`.
- [ ] Ejecutar la app local con `VoidEmpires__Persistence__Provider="SqlServer"` solo despues de que el esquema exista.
- [ ] Ejecutar el dry-run de catalogos finales sin `-Apply` y confirmar que no borra datos de gameplay.
- [ ] Limpiar variables locales: `VoidEmpires__Persistence__Provider`, `ConnectionStrings__DefaultConnection`, `VOIDEMPIRES_CONNECTION_STRING`, `VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED`, `VOIDEMPIRES_SQLSERVER_SMOKE_CONNECTION_STRING`.

Referencia detallada: `docs/dev/sql-server-runbook.md`.
