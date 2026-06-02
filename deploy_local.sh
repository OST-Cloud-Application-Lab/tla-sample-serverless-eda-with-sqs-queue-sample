#!/usr/bin/env bash
set -euo pipefail

IMAGE_NAME="tla-sample-app"

: "${TLA_SAMPLE_APP_SERVERLESS_ORG:?ERROR: TLA_SAMPLE_APP_SERVERLESS_ORG is not set}"
: "${SERVERLESS_ACCESS_KEY:?ERROR: SERVERLESS_ACCESS_KEY is not set}"

echo "Building image: ${IMAGE_NAME}"
docker build -t "${IMAGE_NAME}" .

echo "Running deployment container"
docker run --rm \
  -e TLA_SAMPLE_APP_SERVERLESS_ORG="${TLA_SAMPLE_APP_SERVERLESS_ORG}" \
  -e SERVERLESS_ACCESS_KEY="${SERVERLESS_ACCESS_KEY}" \
  "${IMAGE_NAME}"
