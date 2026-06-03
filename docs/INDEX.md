# Documentation Index

Complete guide to NoteBook - streamlined and essential.

## 🚀 Quick Start

1. **[README.md](../README.md)** - What is NoteBook?
2. **[SETUP.md](../SETUP.md)** - Install and run (Docker or local)
3. **[API.md](API.md)** - Test the API endpoints

## 📚 Essential Documentation

### Core Docs

| File | Purpose | Audience |
|------|---------|----------|
| [API.md](API.md) | API endpoints, examples, testing | Developers, DevOps |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Code structure, patterns | Developers |
| [DEPLOYMENT.md](DEPLOYMENT.md) | Production deployment | DevOps |
| [DEVELOPMENT.md](DEVELOPMENT.md) | Development workflow | Developers |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Contribution guidelines | Contributors |
| [TROUBLESHOOTING.md](TROUBLESHOOTING.md) | Common issues & fixes | Everyone |
| [PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md) | File organization | Everyone |

### Root Files

| File | Purpose |
|------|---------|
| [README.md](../README.md) | Project overview |
| [SETUP.md](../SETUP.md) | Complete setup guide |
| [CHANGELOG.md](../CHANGELOG.md) | Version history |
| [LICENSE](../LICENSE) | MIT License |

## 🎯 By Use Case

### "I want to deploy this"
→ [SETUP.md](../SETUP.md) → [DEPLOYMENT.md](DEPLOYMENT.md)

### "I want to develop features"
→ [ARCHITECTURE.md](ARCHITECTURE.md) → [DEVELOPMENT.md](DEVELOPMENT.md)

### "I want to test the API"
→ [API.md](API.md)

### "Something is broken"
→ [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

### "I want to contribute"
→ [CONTRIBUTING.md](CONTRIBUTING.md)

## 🏗️ Architecture Overview

```
NoteBook (Clean Architecture)
├── Domain Layer        → Business entities & rules
├── Application Layer   → CQRS commands & queries
├── Infrastructure      → Database (Dapper + PostgreSQL)
└── Web Layer          → ASP.NET Core API
```

See [ARCHITECTURE.md](ARCHITECTURE.md) for details.

## 🐳 Docker Quick Reference

```bash
# Pull and run from Docker Hub
docker pull msaid356/notebook:latest
docker-compose up -d

# Build from source
docker-compose -f docker-compose.build.yml up -d

# Push to Docker Hub
docker build -t msaid356/notebook:latest .
docker push msaid356/notebook:latest
```

See [SETUP.md](../SETUP.md) for credentials and details.

## 🛠️ Common Commands

```bash
# Local development
dotnet run --project NoteBook.Web

# Docker
docker-compose up -d
docker-compose logs -f

# Database
psql -U postgres -d notebook_db -f db/schema.sql

# Testing
curl http://localhost:5000/health
curl http://localhost:5000/api/notes
```

## 📝 Documentation Philosophy

This documentation is **streamlined** - only essential files:
- ✅ Clear and focused
- ✅ No duplication
- ✅ Easy to navigate
- ✅ Practical examples

## 🤝 Need Help?

1. Check [TROUBLESHOOTING.md](TROUBLESHOOTING.md) first
2. Review relevant documentation above
3. Create GitHub issue if problem persists

---

**Tech Stack:** ASP.NET Core 10, PostgreSQL 16, Docker  
**Image:** msaid356/notebook  
**Last Updated:** June 2026
