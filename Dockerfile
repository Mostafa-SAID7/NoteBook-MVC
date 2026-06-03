# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["NoteBook.Web/NoteBook.Web.csproj", "NoteBook.Web/"]
COPY ["NoteBook.Application/NoteBook.Application.csproj", "NoteBook.Application/"]
COPY ["NoteBook.Infrastructure/NoteBook.Infrastructure.csproj", "NoteBook.Infrastructure/"]
COPY ["NoteBook.Domain/NoteBook.Domain.csproj", "NoteBook.Domain/"]

# Restore dependencies
RUN dotnet restore "NoteBook.Web/NoteBook.Web.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/NoteBook.Web"
RUN dotnet build "NoteBook.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "NoteBook.Web.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80 443

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p logs

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "NoteBook.Web.dll"]
