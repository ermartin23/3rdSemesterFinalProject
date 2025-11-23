🐦 Dead Pigeons — Jerne IF Lottery System
Final Project — Programming II · CDS Security · Systems Development
👨‍💻 Team CodeBusters
👥 Team Members

Erica

Emre

Katja

Laura

📌 Project Overview

Dead Pigeons is a weekly lottery-style game played by supporters of Jerne IF.
This project delivers a full digital platform for managing players, transactions, number boards, weekly games, authentication, and admin workflows.

The system is fully distributed: a React frontend communicating with a .NET Web API backend, deployed to the cloud with secure handling of all secrets and user data.

🧱 Tech Stack
🔧 Backend — .NET Web API

.NET 9 Web API

Entity Framework Core (PostgreSQL)

Authentication & Authorization (JWT or similar)

Server-side validation

Swagger / OpenAPI (NSwag-generated client)

GUID-based IDs

GitHub Actions CI (build + test)

XUnit + XUnit.DependencyInjection

TestContainers (isolated test database)

Docker (for development + deployment)

🎨 Frontend — React Client

React

TypeScript

React Router

Vite

Component styling (TailwindCSS / DaisyUI planned)

API client (NSwag or fetch wrappers)

☁️ Infrastructure & Security

Cloud deployment (Fly.io or equivalent)

No secrets in git

Environment-based configuration

Secure password hashing

Authorization policies based on roles

📂 Planned Repository Structure
/client      → React + TypeScript frontend
/server      → .NET Web API backend
/tests       → Automated backend tests
README.md

🚧 Project Status (Initial Phase)

We are currently setting up:

Solution structure

Required dependencies

Database connection (PostgreSQL)

Testing setup (XUnit + TestContainers)

CI workflow (GitHub Actions)

API skeleton + first endpoints

Frontend project initialization

📜 Future README Sections (Placeholders)

These will be completed as the project grows:

🔐 Security & Authorization Policies

Explain which roles exist, who can access what, and why.

🌍 Environment & Configuration

Required environment variables

Local development setup

Cloud deployment configuration

Secret handling strategy

🚀 Deployment Guide

Steps to deploy backend

Steps to deploy frontend

Docker images

CI/CD explanation

🧪 Testing Documentation

How to run tests

TestContainers setup

Happy/unhappy path coverage

📦 API Documentation

OpenAPI / Swagger URL
NSwag client setup instructions

🪲 Known Issues / Current Limitations

(To be filled during development)