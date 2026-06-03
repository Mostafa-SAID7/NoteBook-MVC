# Security Policy

## Supported Versions

Currently supported versions with security updates:

| Version | Supported          |
| ------- | ------------------ |
| 2.1.x   | :white_check_mark: |
| 2.0.x   | :white_check_mark: |
| < 2.0   | :x:                |

## Reporting a Vulnerability

We take security vulnerabilities seriously. If you discover a security issue, please follow these steps:

### How to Report

1. **Do NOT** open a public GitHub issue
2. Email security details to: [your-email@example.com] (replace with actual email)
3. Include:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fix (if any)

### What to Expect

- **Acknowledgment**: Within 48 hours
- **Initial Assessment**: Within 5 business days
- **Status Updates**: Every 7 days until resolved
- **Fix Timeline**: Depends on severity
  - Critical: 7 days
  - High: 14 days
  - Medium: 30 days
  - Low: 90 days

### Security Best Practices

When deploying NoteBook:

1. **Database Security**
   - Use strong passwords
   - Enable SSL/TLS for connections
   - Create dedicated database user (not postgres)
   - Restrict network access

2. **Application Security**
   - Change default JWT secret
   - Use HTTPS in production
   - Enable rate limiting
   - Keep dependencies updated
   - Review logs regularly

3. **Docker Security**
   - Don't use `latest` tag in production
   - Scan images for vulnerabilities
   - Use non-root user in containers
   - Limit container resources

4. **Environment Variables**
   - Never commit secrets to git
   - Use secret management systems
   - Rotate credentials regularly

### Known Security Features

- ✅ JWT authentication
- ✅ Input validation (FluentValidation)
- ✅ SQL injection protection (Parameterized queries via Dapper)
- ✅ Rate limiting
- ✅ CORS configuration
- ✅ Health checks (no sensitive data exposed)

### Security Updates

Subscribe to GitHub releases to get notified of security updates.

### Disclosure Policy

Once a vulnerability is fixed:
1. Security advisory published on GitHub
2. CVE requested (if applicable)
3. Public disclosure after fix is available

Thank you for helping keep NoteBook secure!
