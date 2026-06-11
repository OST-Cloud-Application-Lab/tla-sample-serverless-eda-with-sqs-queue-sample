---
adr_id: "0007"
title: Implement all microservice endpoints as Serverless functions
status: decided
tags: []
links:
    precedes: []
    succeeds: []
comments: []
---

## <a name="question"></a> Question

How should each microservice expose its HTTP endpoints to users?

## <a name="options"></a> Options

1. <a name="option-1"></a> One Lambda per endpoint
2. <a name="option-2"></a> Single Lambda per microservice (fat Lambda)

## <a name="criteria"></a> Criteria

- **Per-endpoint observability**: CloudWatch metrics (invocations, errors, duration, throttles) and log groups must be attributable to individual API operations without request-level log filtering; a latency spike or error surge on one endpoint must be detectable in isolation.
- **Independent scaling**: Endpoints with different load profiles (e.g., read-heavy `GET /tlas` vs. write-heavy `POST /tlas/{groupName}`) must scale concurrently and independently; one endpoint must not monopolize Lambda concurrency reserved for the service.
- **IAM least privilege**: Each Lambda function should carry only the IAM permissions it requires; a read endpoint should not hold write permissions simply because a write endpoint in the same service needs them.
- **Alignment with Serverless Framework conventions**: The team decided to use the Serverless Framework (ADR-0003); the solution must map naturally to the `functions:` block in `serverless.yml` without bespoke runtime adaptors or non-standard deployment packaging.
- **Cold start isolation**: A cold start triggered by traffic on one endpoint must not delay execution of another endpoint; functions must start independently.
- **Operational simplicity**: The deployment unit for a single endpoint change should be scoped to that function; deploying a fix to one handler must not require redeploying every handler in the service.

## <a name="outcome"></a> Outcome

We decided for [Option 1](#option-1).

Each HTTP route and event trigger in the Manager, Resolver, and Reports services is defined as a separate `functions:` entry in `serverless.yml`. The Manager exposes `readAllTlaGroups`, `addNewTlaGroup`, `addNewTla`, `acceptTla`, `createReport`, and `report` as individual HTTP API-backed Lambda functions, plus `updateReport` as an EventBridge-triggered function. The Resolver exposes `readAllTlaGroups`, `readAllTlas`, and `readTlaGroupByName` as HTTP API-backed functions, plus `acceptTla` as an EventBridge-triggered function. The Reports service exposes `generateReport` as an SQS-triggered function. Each function receives its own CloudWatch log group and its own CloudWatch metrics, and the shared IAM role for each service stack is scoped to only the permissions actually required across its functions.

Option 2 produces a fat Lambda: the entire HTTP framework dependency graph is bundled into one deployment artifact; all endpoints share the same timeout, memory allocation, and concurrency limit; and CloudWatch metrics aggregate across all operations, making per-endpoint latency or error analysis impossible without log-level filtering. Under load, a slow write endpoint can exhaust the concurrency budget shared with fast read endpoints. Option 3 avoids some of those constraints by running the real framework, but it still conflates all endpoint metrics and IAM permissions into a single function, and it requires the Lambda Web Adapter layer to be added and maintained as an explicit deployment dependency — a non-trivial operational concern that contradicts the simplicity goal established in ADR-0003.

## <a name="comments"></a> Comments
<a name="comment-1"></a>1. (2026-06-11 10:00:00) : marked decision as decided
