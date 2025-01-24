# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

ARG Nuget_CustomFeedUserName
ARG Nuget_CustomFeedPassword

# ARG HEC_NEXUS_READ_UID
# ARG HEC_NEXUS_READ_PASSWORD

COPY . .

# RUN dotnet nuget update source ras-nuget-public --username $HEC_NEXUS_READ_UID --password "$HEC_NEXUS_READ_PASSWORD" --store-password-in-clear-text

# RUN dotnet build 

# # Production stage
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS prod

# COPY --from=builder /bin/Debug/net8.0 /app

# CMD ["/app/cc-ras"]