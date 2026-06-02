FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-manager

WORKDIR /app/manager
RUN apt-get update \
    && apt-get install -y --no-install-recommends zip \
    && rm -rf /var/lib/apt/lists/*
COPY manager/ ./
RUN bash ./.build.sh


FROM eclipse-temurin:21-jdk-noble AS build-resolver

WORKDIR /app/resolver
COPY resolver/ ./
RUN chmod +x ./mvnw && ./mvnw clean package -q


FROM python:3.14-alpine

RUN apk add --no-cache bash nodejs npm \
    && npm install -g serverless


WORKDIR /app
COPY . .

COPY --from=build-manager  /app/manager/bin/release/net10.0/deploy-package.zip  manager/bin/release/net10.0/deploy-package.zip
COPY --from=build-resolver /app/resolver/target/                                resolver/target/


RUN cat <<'EOF' > /entrypoint.sh
#!/usr/bin/env bash
set -euo pipefail
 
echo "Deploy infrastructure"
cd /app/infrastructure
sls deploy
 
echo "Deploy manager"
cd /app/manager 
sls deploy
sls invoke --function seedDatabase --data 'unused'
 
echo "Deploy resolver"
cd /app/resolver
sls deploy
sls invoke --function seedDatabase --data 'unused'
 
echo "Deploy reports"
cd /app/reports
sls deploy
 
echo ""
echo "=============================================="
echo " All services deployed successfully!"
echo "=============================================="
EOF
RUN sed -i 's/\r$//' /entrypoint.sh && chmod +x /entrypoint.sh

ENTRYPOINT ["bash", "/entrypoint.sh"]
