# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

COPY . .

RUN dotnet build 

# Production stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS prod

COPY --from=builder /bin/Debug/net8.0 /app

CMD ["/app/cc-ras"]