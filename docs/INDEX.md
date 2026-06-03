# Documentation Index

Complete guide to NoteBook documentation files. Start here to find what you're looking for.

## 🚀 Quick Start

**New to NoteBook?** Start here:

1. [README.md](../README.md) - Project overview and features
2. [SETUP.md](../SETUP.md) - Get it running in 5 minutes
3. [ARCHITECTURE.md](ARCHITECTURE.md) - Understand the codebase structure

## 📚 Documentation by Role

### For End Users / Testers

- [README.md](../README.md) - What is NoteBook?
- [SETUP.md](../SETUP.md) - How to install locally or with Docker
- [docs/API.md](API.md) - API endpoints and examples
- [docs/TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Fix common issues

### For Developers

- [docs/ARCHITECTURE.md](ARCHITECTURE.md) - Code structure and design patterns
- [docs/DEVELOPMENT.md](DEVELOPMENT.md) - Development workflow and tools
- [docs/CONTRIBUTING.md](CONTRIBUTING.md) - Coding standards and guidelines
- [docs/TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Debug and fix problems
- [API.md](API.md) - Test API endpoints

### For DevOps / Operations

- [SETUP.md](../SETUP.md) - Local installation
- [docs/DEPLOYMENT.md](DEPLOYMENT.md) - Production deployment strategies
- [docker-compose.yml](../docker-compose.yml) - Docker Compose configuration
- [Dockerfile](../Dockerfile) - Docker image definition
- [docs/TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Operations issues

### For Contributors

- [docs/CONTRIBUTING.md](CONTRIBUTING.md) - How to contribute
- [docs/DEVELOPMENT.md](DEVELOPMENT.md) - Development setup and workflow
- [docs/ARCHITECTURE.md](ARCHITECTURE.md) - Code structure to understand before coding
- [SETUP.md](../SETUP.md) - Local development environment

## 📖 Documentation Files

### Root Level

| File | Purpose |
|------|---------|
| [README.md](../README.md) | Project overview & quick links |
| [SETUP.md](../SETUP.md) | Installation (local + Docker) |
| [PROJECT_SUMMARY.md](../PROJECT_SUMMARY.md) | Delivery checklist |

### Docs Folder

| File | Purpose |
|------|---------|
| [INDEX.md](INDEX.md) | Navigation hub |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Code structure & patterns |
| [DOMAIN_LAYER.md](DOMAIN_LAYER.md) | Domain entities & value objects |
| [API.md](API.md) | Endpoint documentation |
| [HEALTH_CHECKS.md](HEALTH_CHECKS.md) | Health check endpoints & monitoring |
| [RATE_LIMITING.md](RATE_LIMITING.md) | Rate limiting configuration & testing |
| [PAGINATION.md](PAGINATION.md) | Pagination & sorting guide |
| [DEVELOPMENT.md](DEVELOPMENT.md) | Development workflow |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Contribution guidelines |
| [DEPLOYMENT.md](DEPLOYMENT.md) | Production deployment |
| [TROUBLESHOOTING.md](TROUBLESHOOTING.md) | Common issues |

### Database

- [db/README.md](../db/README.md) - Database schema & operations
- [db/schema.sql](../db/schema.sql) - Main schema
- [db/migrations/](../db/migrations/) - Schema migrations
- [db/seeds/](../db/seeds/) - Sample data

## 🔍 Common Questions

### "How do I get started?"
→ Read [README.md](../README.md) then [SETUP.md](../SETUP.md)

### "How does the code work?"
→ Read [ARCHITECTURE.md](ARCHITECTURE.md)

### "What is the domain model?"
→ Read [DOMAIN_LAYER.md](DOMAIN_LAYER.md)

### "How do I test the API?"
→ Read [API.md](API.md)

### "How do I monitor the API?"
→ Read [HEALTH_CHECKS.md](HEALTH_CHECKS.md)

### "How do I configure rate limiting?"
→ Read [RATE_LIMITING.md](RATE_LIMITING.md)

### "How do I use pagination?"
→ Read [PAGINATION.md](PAGINATION.md)

### "How do I deploy this?"
→ Read [DEPLOYMENT.md](DEPLOYMENT.md)

### "Something's broken, how do I fix it?"
→ Check [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

### "How can I contribute?"
→ Read [CONTRIBUTING.md](CONTRIBUTING.md)

### "How do I setup the database?"
→ Read [db/README.md](../db/README.md)

## 🏗️ Architecture Quick Reference

```
NoteBook Architecture
├── Domain Layer (NoteBook.Domain)
│   ├── Entities: Business models
│   ├── Exceptions: Domain errors
│   └── Repositories: Interface contracts
├── Application Layer (NoteBook.Application)
│   ├── CQRS: Commands and Queries
│   ├── DTOs: Data transfer objects
│   └── Handlers: Business logic
├── Infrastructure Layer (NoteBook.Infrastructure)
│   ├── Repositories: Data access implementations
│   └── Database: Connection management
└── Web Layer (NoteBook.Web)
    ├── Controllers: API endpoints
    └── Views: User interface
```

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed explanation.

## 🛠️ Common Commands

```bash
# Setup
git clone https://github.com/yourusername/NoteBook.git
cd NoteBook

# Local Development
dotnet build
cd NoteBook.Web && dotnet run

# Docker
docker-compose up

# Database
psql -U postgres -d notebook_db -f database.sql

# Testing
dotnet test
curl http://localhost:5000/api/notes
```

See [DEVELOPMENT.md](DEVELOPMENT.md) for more commands.

## 📝 Documentation Standards

All documentation follows these principles:

- **Clear**: Concise, avoid unnecessary jargon
- **Complete**: Cover main scenarios
- **Accessible**: Different skill levels
- **Maintainable**: Easy to update
- **DRY**: No duplication across files

## 🔗 External Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Dapper GitHub](https://github.com/DapperLib/Dapper)
- [MediatR Documentation](https://jbogard.com/mediatr/)
- [Docker Documentation](https://docs.docker.com/)

## 📋 Document Versions

| Document | Version | Last Updated |
|----------|---------|--------------|
| README.md | 1.0.0 | June 2026 |
| SETUP.md | 1.0.0 | June 2026 |
| ARCHITECTURE.md | 1.0.0 | June 2026 |
| DOMAIN_LAYER.md | 2.0.0 | June 2026 |
| API.md | 2.0.0 | June 2026 |
| HEALTH_CHECKS.md | 1.0.0 | June 2026 |
| PAGINATION.md | 1.0.0 | June 2026 |
| DEVELOPMENT.md | 1.0.0 | June 2026 |
| CONTRIBUTING.md | 1.0.0 | June 2026 |
| DEPLOYMENT.md | 1.0.0 | June 2026 |
| TROUBLESHOOTING.md | 1.0.0 | June 2026 |

## 💡 Tips for Using Documentation

1. **Use search** (Ctrl+F / Cmd+F) to find specific topics
2. **Follow links** to related documents for deeper understanding
3. **Check TROUBLESHOOTING.md** first if you encounter issues
4. **Report gaps** - If something is missing, create an issue
5. **Keep it updated** - Help keep docs current when you learn new things

## 🤝 Contributing to Documentation

Good documentation is crucial. When contributing:

1. Use clear, concise language
2. Include practical examples
3. Link to related sections
4. Update version numbers
5. Test instructions before documenting

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.

## 📞 Getting Help

- **Found a bug?** Create a GitHub Issue
- **Have a question?** Check documentation first, then create Discussion
- **Want to contribute?** See [CONTRIBUTING.md](CONTRIBUTING.md)
- **Need support?** Check [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

---

**Last Updated**: June 2026  
**Documentation Version**: 1.0.0
