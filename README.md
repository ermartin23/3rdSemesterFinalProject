README file

# 🐦 Dead Pigeons — Jerne IF Lottery System
Final Project — Programming II · CDS Security · Systems Development

**Team:** CodeBusters

**Team Members**
Emre
Erica
Katja
Laura

---

## Project Overview

Dead Pigeons is a weekly lottery-style game played by supporters of Jerne IF. This project delivers a full digital platform for managing players, transactions, number boards, weekly games, authentication, and admin workflows.

The system is fully distributed: a React frontend communicating with a .NET Web API backend, deployed to the cloud with secure handling of all secrets and user data.

---

## Tech Stack

### Backend
The backend is implemented as a .NETWeb API, which is responsible for business logic, persistence and security.

- .NET 9 Web API
- Entity Framework Core (PostgreSQL)
- Authentication & Authorization (JWT)
- Server-side validation
- Swagger / OpenAPI (NSwag-generated client)
- GUID-based IDs
- GitHub Actions CI (build + test)
- XUnit + XUnit.DependencyInjection
- TestContainers (isolated test database)
- Docker (for development + deployment)

### Frontend
The frontend is a React client that’s responsible for user interaction and presentation logic.

- React + TypeScript
- React Router
- Vite
- Component styling with TailwindCSS/DaisyUI
- API communication via fetch/NSwag client

---

## Infrastructure & Security

- Cloud deployment via Fly.io
- No secrets in git
- Environment-based configuration
- Secure password hashing
- Authorization policies based on roles

---

## Repository Structure

/client → React + TypeScript frontend
/server → .NET Web API backend
/tests  → Automated backend tests
README.md

---

## Deployment (Fly.io)
### Backend

**Fly.io App Monitoring**
https://fly.io/apps/deadpigeonsdev-muddy-flower-4166/monitoring

**Public API URL**
https://deadpigeons-aged-voice-1352.fly.dev

### Frontend
The frontend is deployed separately and configured via environment variables to communicate with the Fly.io backend.

---

## Test Accounts & Password Hashing

### Admin
- **Email:** firstadmin@example.com
- **Password:** MyAdminPassword123!

### Player
- **Email:** secondplayer@example.com
- **Password:** secondplayer123

Passwords are never stored in plain text.
All credentials are hashed before persistence.

---

## Password Hashing Tool

A small utility is included to generate hashed passwords for seeding and testing.

dotnet run --project server/HashTool -- <PASSWORD_YOU_WANT>

Example:
dotnet run --project server/HashTool -- MyAdminPassword123!

---

## Authentication & Authorization

Passwords are hashed using ASP.NET Core Identity PasswordHasher
Login endpoint returns a JWT
JWT includes claims:

- sub = userId
- role = Admin / Player

Authorization rules:
Admin endpoints require Admin role
Player endpoints require Player role
Ownership is enforced using the JWT sub claim

---

## Known Issues / Current Limitations

Cookie-based authentication is not implemented yet
JWT is handled via HTTP headers
No refresh-token mechanism
Intended for academic and demonstration purposes