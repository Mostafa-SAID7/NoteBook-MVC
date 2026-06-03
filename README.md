# NoteBook

A production-grade note-taking application with clean architecture, PostgreSQL, and Dapper.

**Status**: ✅ Production-Ready | **Version**: 2.1.0 | **Framework**: ASP.NET Core 9 | **Phase**: 5 (Rate Limiting Recommended)

## 🚀 Quick Start

### Docker (Recommended)
```bash
# Using pre-built image from Docker Hub
docker pull msaid356/notebook:latest
docker-compose up -d
# API: http://localhost:5000

# Or build from source
git clone https://github.com/yourusername/NoteBook.git
cd NoteBook
docker-compose -f docker-compose.build.yml up -d
```

### Local Development
```bash
git clone https://github.com/yourusername/NoteBook.git
cd NoteBook
dotnet restore
dotnet build
cd NoteBook.Web && dotnet run
# API: http://localhost:5000
```

See [SETUP.md](SETUP.md) for detailed setup instructions.

## 📚 Documentation

| Document | Purpose |
|----------|---------|
| [SETUP.md](SETUP.md) | Complete installation & Docker guide |
| [docs/API.md](docs/API.md) | API endpoints & examples |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Code structure & patterns |
| [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) | Development workflow |
| [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) | Production deployment |
| [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md) | Contribution guidelines |
| [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) | Common issues & fixes |
| [docs/INDEX.md](docs/INDEX.md) | Complete documentation index |

## ✨ Features

### Phase 1-4 (Complete)
- ✅ Create, read, update, delete notes (with soft delete)
- ✅ Archive & restore notes
- ✅ Full-text search (title, content, tags)
- ✅ Tag-based categorization
- ✅ Pagination support
- ✅ RESTful API
- ✅ JWT authentication
- ✅ Input validation (FluentValidation)
- ✅ Unit testing (xUnit)
- ✅ Health checks (`/api/health`)

### Phase 5 (Recommended)
- ⏳ Rate limiting (protection against abuse)
- 📚 See [ADVANCED_FEATURES_REVIEW.md](ADVANCED_FEATURES_REVIEW.md)

### Phase 6+ (Optional)
- ⏳ Redis caching (performance)
- 🎯 User collaboration (sharing, comments)
- 🎯 File attachments
- 🎯 Activity logging

## 🛠 Tech Stack

- **Backend**: ASP.NET Core 9.0
- **Database**: PostgreSQL 16
- **Data Access**: Dapper + Npgsql
- **Architecture**: Clean Architecture + CQRS
- **Orchestration**: MediatR
- **Logging**: Serilog
- **Containerization**: Docker
- **Registry**: Docker Hub (msaid356/notebook)

## 📊 Project Structure

```
NoteBook/
├── NoteBook.Domain/              # Core business logic
├── NoteBook.Application/         # CQRS & services
├── NoteBook.Infrastructure/      # Data access
├── NoteBook.Web/                 # API controllers
├── db/                           # Database schemas & migrations
├── docs/                         # Documentation
├── Dockerfile                    # Container image
└── docker-compose.yml            # Local dev stack
```

## 🔌 API Endpoints

```
GET    /api/notes                 # Get all notes
GET    /api/notes/{id}            # Get single note
POST   /api/notes                 # Create note
PUT    /api/notes/{id}            # Update note
DELETE /api/notes/{id}            # Delete note
GET    /api/notes/search?term=... # Search notes
```

Full documentation: [docs/API.md](docs/API.md)

## 🤝 Contributing

1. Read [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md)
2. Follow coding standards in [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md)
3. Create feature branch: `git checkout -b feature/your-feature`
4. Commit with conventional messages: `git commit -m "feat(scope): description"`
5. Push and submit PR

## 📋 Requirements

- .NET 9 SDK
- PostgreSQL 16 (or Docker)
- Docker & Docker Compose (optional, but recommended)

## 🆘 Issues & Support

- **Stuck?** Check [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)
- **Question?** See [docs/INDEX.md](docs/INDEX.md)
- **Bug?** Open GitHub Issue
- **Feature Request?** Open GitHub Discussion

## 📄 License

MIT License - See [LICENSE](LICENSE)

## 🙏 Acknowledgments

- Original project: [Mostafa SAID7/NoteBook](https://github.com/Mostafa-SAID7/NoteBook)
- Built with ❤️ using ASP.NET Core

---

**[Start here →](docs/INDEX.md)** | **[API Docs →](docs/API.md)** | **[Setup Guide →](SETUP.md)**
