#!/bin/bash
set -a
source .env
set +a

# dotnet tool install -g dotnet-ef
dotnet ef dbcontext scaffold "$CONN_STR" Npgsql.EntityFrameworkCore.PostgreSQL \
    --output-dir ./Entities \
    --context-dir . \
    --context MyDbContext \
    --no-onconfiguring \
    --namespace dataaccess.Entities \
    --context-namespace Infrastructure.Postgres.Scaffolding \
    --schema deadpigeonsdb \
    --force