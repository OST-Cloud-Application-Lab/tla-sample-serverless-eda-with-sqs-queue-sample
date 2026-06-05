---
adr_id: "0005"
title: Deploy shared infrastructure in a separate Serverless stack
status: decided
tags: []
links:
    precedes: []
    succeeds: []
comments: []
---

## <a name="question"></a> Question

How should shared AWS infrastructure resources (EventBridge event bus, SQS queues) be managed in relation to the individual microservice stacks?

## <a name="options"></a> Options

1. <a name="option-1"></a> Dedicated infrastructure stack — shared resources defined once in a separate `infrastructure/serverless.yml` stack with no Lambda functions, consumed by service stacks via CloudFormation exports
2. <a name="option-2"></a> Owned by one service stack — shared resources defined inside the manager stack as the primary service, referenced by other stacks via CloudFormation exports

## <a name="criteria"></a> Criteria

- **Deployment independence**: Each microservice stack must be deployable and redeployable in isolation without modifying or disrupting shared AWS resources that other services depend on.
- **Single source of truth**: Shared AWS resources (event bus, queues) must be defined in exactly one place to prevent naming conflicts, ARN drift, or accidental deletion caused by an unrelated service deployment.
- **Explicit deployment ordering**: The pipeline must have a clearly defined, dependency-free ordering; shared resources must exist before services that consume them are deployed.
- **Cross-stack references**: Service stacks must resolve ARNs and names of shared resources at deploy time without hardcoding values that could drift between stages.
- **Tooling consistency**: The solution should not require introducing a second IaC toolchain beyond the already-decided Serverless Framework.

## <a name="outcome"></a> Outcome

We decided for [Option 1](#option-1).

A dedicated `infrastructure/serverless.yml` stack defines only the shared AWS resources (the `TLAEventBus` EventBridge event bus and the `tla-report-queue` SQS queue) with no Lambda functions. The stack exports `TLAEventBusName`, `TLAEventBusArn`, `TLAReportQueueArn`, and `TLAReportQueueUrl` as CloudFormation stack outputs. The `manager`, `resolver`, and `reports` service stacks consume these outputs using `${cf:tla-infrastructure-serverless-${sls:stage}.<ExportName>}` references, which CloudFormation resolves at deploy time. The infrastructure stack is deployed first in the CI/CD pipeline and is only redeployed when shared resources change.

Option 2 was ruled out because it creates a hidden lifecycle dependency: if the manager stack is torn down (e.g. for a full redeploy), the event bus and SQS queue are destroyed along with it, immediately breaking the resolver and reports services. Option 3 would either cause naming conflicts when multiple stacks try to create resources with the same name, or produce duplicate isolated resources that diverge over time. Option 4 would require learning and maintaining a second IaC tool, which contradicts the simplicity goal established in ADR-0001.

## <a name="comments"></a> Comments
<a name="comment-1"></a>1. (2026-06-05 09:14:32) : marked decision as decided
