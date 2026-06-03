# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Docker Hub integration (msaid356/notebook)
- Automated Docker builds via GitHub Actions
- Multi-architecture Docker images (amd64, arm64)

### Changed
- Upgraded from .NET 9.0 to .NET 10.0
- Updated Docker configuration for .NET 10 defaults (ports 8080/8081)
- Improved GitHub workflows with caching

### Fixed
- Docker health checks now work correctly (added curl)
- Database initialization path in CI/CD
- Port configurations across all Docker files

## [2.1.0] - 2026-06-03

### Added
- Rate limiting with AspNetCoreRateLimit
- Configurable rate limits per endpoint
- Health check endpoints
- Comprehensive documentation

### Changed
- Improved error handling
- Enhanced logging with Serilog
- Better validation messages

### Fixed
- Pagination edge cases
- Search query optimization

## [2.0.0] - 2026-05-15

### Added
- Clean Architecture implementation
- CQRS pattern with MediatR
- Domain-driven design (DDD) patterns
- Full-text search capability
- Tag-based categorization
- Archive/restore functionality
- Soft delete support
- Input validation with FluentValidation
- JWT authentication

### Changed
- Complete architectural refactoring
- Migrated to Dapper for data access
- PostgreSQL as primary database

### Breaking Changes
- API endpoint structure changed
- Database schema redesigned
- Authentication system updated

## [1.0.0] - 2026-04-01

### Added
- Initial release
- Basic CRUD operations for notes
- PostgreSQL database integration
- Docker support
- Basic API documentation

---

## Version Guidelines

### Types of Changes
- **Added** - New features
- **Changed** - Changes in existing functionality
- **Deprecated** - Soon-to-be removed features
- **Removed** - Removed features
- **Fixed** - Bug fixes
- **Security** - Security vulnerability fixes

### Version Numbers
- **Major** (X.0.0) - Breaking changes
- **Minor** (0.X.0) - New features, backward compatible
- **Patch** (0.0.X) - Bug fixes, backward compatible
