# Troubleshooting Guide

Solutions to common issues when running NoteBook.

## Installation Issues

### Issue: "dotnet command not found"

**Cause**: .NET SDK not installed or not in PATH

**Solution**:
1. Install .NET 9 SDK from https://dotnet.microsoft.com/download/dotnet/9.0
2. Verify installation:
   ```bash
   dotnet --version
   ```
3. Restart terminal/IDE

---

### Issue: "Cannot find package X"

**Cause**: NuGet packages not restored or network issue

**Solution**:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# If still failing, try specific package source
dotnet restore --source https://api.nuget.org/v3/index.json
```

---

## Database Connection Issues

### Issue: "Connection refused" (PostgreSQL)

**Cause**: PostgreSQL service not running or wrong connection string

**Solution**:

1. **Check PostgreSQL is running**:
   ```bash
   # Windows
   Get-Service | Where-Object {$_.Name -like "*postgre*"}
   
   # Mac
   brew services list | grep postgresql
   
   # Linux
   sudo systemctl status postgresql
   ```

2. **Test connection**:
   ```bash
   psql -U postgres -h localhost
   ```

3. **Update connection string** in `NoteBook.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=notebook_db;Username=postgres;Password=postgres"
   }
   ```

4. **Restart application**

---

### Issue: "Database notebook_db doesn't exist"

**Cause**: Database not created

**Solution**:
```bash
# Create database
psql -U postgres -c "CREATE DATABASE notebook_db;"

# Initialize schema
psql -U postgres -d notebook_db -f database.sql

# Verify
psql -U postgres -d notebook_db -c "\dt"
```

---

### Issue: "FATAL: password authentication failed for user"

**Cause**: Wrong password in connection string

**Solution**:
1. Reset PostgreSQL password:
   ```bash
   psql -U postgres -c "ALTER USER postgres PASSWORD 'newpassword';"
   ```

2. Update connection string with new password

3. Test connection:
   ```bash
   psql -U postgres -d notebook_db
   ```

---

### Issue: "SSL/TLS connection required"

**Cause**: PostgreSQL requires SSL but connection string doesn't specify it

**Solution**:
```json
"DefaultConnection": "Host=db.example.com;Port=5432;Database=notebook_db;Username=user;Password=pass;SslMode=Require"
```

---

## Build Issues

### Issue: "Build failed with X errors"

**Solution**:
```bash
# Clean build
dotnet clean

# Restore dependencies
dotnet restore

# Rebuild
dotnet build
```

---

### Issue: "namespace NoteBook not found"

**Cause**: Project reference missing or namespace incorrect

**Solution**:
1. Verify project references in `.csproj`:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\NoteBook.Application\NoteBook.Application.csproj" />
   </ItemGroup>
   ```

2. Check namespace declaration at top of files

3. Rebuild solution

---

## Runtime Issues

### Issue: Application won't start

**Solution**:
1. **Check logs**:
   ```bash
   tail -f logs/app-*.txt
   ```

2. **Verify database connection**:
   ```bash
   psql -U postgres -d notebook_db -c "SELECT 1"
   ```

3. **Check port availability**:
   ```bash
   # Windows
   netstat -ano | findstr :5000
   
   # Mac/Linux
   lsof -i :5000
   ```

4. **Use different port**:
   ```bash
   export ASPNETCORE_URLS="http://+:5001"
   dotnet run
   ```

---

### Issue: "Port 5000 already in use"

**Solution**:

Option 1: Kill process on port
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Mac/Linux
lsof -i :5000
kill -9 <PID>
```

Option 2: Use different port
```bash
export ASPNETCORE_URLS="http://+:5001"
dotnet run
```

---

### Issue: "Exception: Unable to resolve service"

**Cause**: Dependency injection not configured

**Solution**:
1. Check `Program.cs` for service registration:
   ```csharp
   services.AddApplicationServices();
   services.AddInfrastructureServices(connectionString);
   ```

2. Verify dependencies are registered:
   ```csharp
   services.AddScoped<INoteRepository, NoteRepository>();
   ```

3. Restart application

---

## API Issues

### Issue: "404 Not Found" on API endpoint

**Cause**: Endpoint not mapped or wrong URL

**Solution**:
1. Check endpoint URL matches controller route:
   ```csharp
   [Route("api/[controller]")]
   public class NotesController : ControllerBase { }
   ```

2. Verify HTTP method (GET, POST, etc.):
   ```bash
   curl -X GET http://localhost:5000/api/notes
   ```

3. Check for typos in URL

---

### Issue: "400 Bad Request" on POST

**Cause**: Invalid request body or validation failure

**Solution**:
1. Verify request body format:
   ```json
   {
     "title": "My Note",
     "content": "Content here",
     "tags": "tag1,tag2"
   }
   ```

2. Check required fields are provided

3. Verify JSON is valid (use JSON validator)

4. Check request Content-Type header:
   ```bash
   curl -H "Content-Type: application/json" -X POST ...
   ```

---

### Issue: "401 Unauthorized" / "403 Forbidden"

**Cause**: Authentication/authorization not implemented or token invalid

**Solution**:
1. In development, check default user ID:
   ```csharp
   private static readonly Guid DefaultUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
   ```

2. For production, implement proper authentication

---

## Docker Issues

### Issue: "Cannot connect to Docker daemon"

**Cause**: Docker not running or not installed

**Solution**:
1. Install Docker from https://www.docker.com/products/docker-desktop
2. Start Docker daemon:
   ```bash
   # Mac/Windows: Open Docker Desktop app
   # Linux
   sudo systemctl start docker
   ```
3. Verify:
   ```bash
   docker --version
   docker ps
   ```

---

### Issue: "Docker container exits immediately"

**Cause**: Application crash inside container

**Solution**:
1. Check container logs:
   ```bash
   docker logs <container_id>
   ```

2. Run container interactively to see errors:
   ```bash
   docker run -it --rm notebook:latest
   ```

3. Check environment variables:
   ```bash
   docker run -e ASPNETCORE_ENVIRONMENT=Development notebook:latest
   ```

---

### Issue: "Cannot access application in container"

**Cause**: Port not exposed or service not listening

**Solution**:
1. Verify port mapping:
   ```bash
   docker run -p 5000:5000 notebook:latest
   ```

2. Check application is listening:
   ```bash
   docker exec <container> curl http://localhost:5000/health
   ```

3. Check firewall allows port 5000

---

### Issue: "docker-compose up" fails

**Solution**:
1. Check syntax:
   ```bash
   docker-compose config
   ```

2. View detailed error:
   ```bash
   docker-compose up --verbose
   ```

3. Check service dependencies:
   ```bash
   docker-compose logs
   ```

4. Rebuild from scratch:
   ```bash
   docker-compose down -v
   docker-compose up --build
   ```

---

## Performance Issues

### Issue: Application slow/unresponsive

**Cause**: High CPU, memory, or database load

**Solution**:
1. **Check resource usage**:
   ```bash
   # Windows
   Get-Process dotnet | Select-Object ProcessName, CPU, Memory
   
   # Mac/Linux
   top
   ```

2. **Check database**:
   ```bash
   psql -U postgres -d notebook_db -c "SELECT COUNT(*) FROM notes;"
   ```

3. **Check logs for errors**:
   ```bash
   tail -f logs/app-*.txt
   ```

4. **Monitor connections**:
   ```bash
   psql -U postgres -d notebook_db -c "SELECT COUNT(*) FROM pg_stat_activity;"
   ```

---

### Issue: "Database connection pool exhausted"

**Cause**: Too many open connections

**Solution**:
1. Check connection string includes pool settings:
   ```
   Max Pool Size=20;Pooling=true
   ```

2. Restart application to reset connections

3. Kill idle connections in PostgreSQL:
   ```sql
   SELECT pg_terminate_backend(pid) FROM pg_stat_activity 
   WHERE state = 'idle' AND query_start < NOW() - INTERVAL '10 minutes';
   ```

---

## Logging Issues

### Issue: No logs being written

**Cause**: Log directory doesn't exist or permissions issue

**Solution**:
1. Create logs directory:
   ```bash
   mkdir -p logs
   chmod 755 logs
   ```

2. Verify Serilog configuration in `Program.cs`

3. Check file permissions:
   ```bash
   ls -la logs/
   ```

---

## Debugging

### Enable Debug Logging

```bash
# Set environment variable
export ASPNETCORE_ENVIRONMENT=Development
export Logging__LogLevel__Default=Debug

# Run application
dotnet run
```

### Debug in Visual Studio

1. Open `NoteBook.sln` in Visual Studio
2. Set breakpoints in code
3. Press F5 or Debug → Start Debugging
4. Application pauses at breakpoints

### Debug in VS Code

1. Install C# extension
2. Create `.vscode/launch.json`:
   ```json
   {
     "version": "0.2.0",
     "configurations": [
       {
         "name": ".NET Core Launch (web)",
         "type": "coreclr",
         "request": "launch",
         "preLaunchTask": "build",
         "program": "${workspaceFolder}/NoteBook.Web/bin/Debug/net10.0/NoteBook.Web.dll",
         "args": [],
         "cwd": "${workspaceFolder}",
         "stopAtEntry": false,
         "serverReadyAction": {
           "pattern": "\\bNow listening on:\\s+(https?://\\S+)",
           "uriFormat": "{0}",
           "action": "openExternally"
         }
       }
     ]
   }
   ```
3. Press F5 to debug

---

## Getting Help

1. **Check logs**: `logs/app-*.txt`
2. **Search GitHub Issues**: https://github.com/yourusername/NoteBook/issues
3. **Read documentation**: Check SETUP.md, README.md
4. **Create issue**: Include error message, steps to reproduce, environment details

---

**Version**: 1.0.0  
**Last Updated**: June 2026
