# NoteBook

A production-grade note-taking application with clean architecture, PostgreSQL, and Dapper.

**Status**: ✅ Production-Ready | **Version**: 2.1.0 | **Framework**: ASP.NET Core 9 | **Phase**: 5 (Rate Limiting Recommended)

## 🚀 Quick Start

### Docker (Recommended)
```bash
git clone https://github.com/yourusername/NoteBook.git
cd NoteBook
docker-compose up
# API: http://localhost:5000
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
| [SETUP.md](SETUP.md) | Installation & setup guide |
| [ADVANCED_FEATURES_REVIEW.md](ADVANCED_FEATURES_REVIEW.md) | Rate limiting & caching review |
| [ADVANCED_FEATURES_STATUS.md](ADVANCED_FEATURES_STATUS.md) | Current feature status |
| [DOMAIN_ENHANCEMENTS.md](DOMAIN_ENHANCEMENTS.md) | Domain layer improvements |
| [docs/INDEX.md](docs/INDEX.md) | Documentation navigation |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Code structure & patterns |
| [docs/DOMAIN_LAYER.md](docs/DOMAIN_LAYER.md) | Domain entities & patterns |
| [docs/API.md](docs/API.md) | API endpoints & examples |
| [docs/HEALTH_CHECKS.md](docs/HEALTH_CHECKS.md) | Health check endpoints |
| [docs/PAGINATION.md](docs/PAGINATION.md) | Pagination guide |
| [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) | Development workflow |
| [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md) | Contribution guidelines |
| [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) | Production deployment |
| [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md) | Common issues & fixes |

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
- Docker & Docker Compose (optional)

## 🆘 Issues & Support

- **Stuck?** Check [docs/TROUBLESHOOTING.md](docs/TROUBLESHOOTING.md)
- **Question?** See [docs/INDEX.md](docs/INDEX.md) for navigation
- **Bug?** Open GitHub Issue (use template)
- **Feature Request?** Open GitHub Discussion

## 📄 License

[To be determined]

## 🙏 Acknowledgments

- Original project: [Mostafa SAID7/NoteBook](https://github.com/Mostafa-SAID7/NoteBook)
- Built with ❤️ using ASP.NET Core

---

**[Start here →](docs/INDEX.md)** | **[API Docs →](docs/API.md)** | **[Setup Guide →](SETUP.md)**
