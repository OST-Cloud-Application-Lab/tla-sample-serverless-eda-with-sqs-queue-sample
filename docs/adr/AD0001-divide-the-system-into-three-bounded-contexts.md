---
adr_id: "0001"
title: Divide the system into three Bounded Contexts
status: decided
tags: []
links:
    precedes: []
    succeeds: []
comments: []
---

## <a name="question"></a> Question

The original TLA application was a single Java monolith: one Serverless service handling both command operations (proposing and accepting TLAs) and query operations (reading accepted TLAs), backed by a single DynamoDB table. How should this system be decomposed into services?

## <a name="options"></a> Options

1. <a name="option-1"></a> Three Bounded Contexts — TLA Manager (commands and approval), TLA Resolver (public read model), TLA Reports (async PDF generation) — each with its own codebase, data store, and independent deployment
2. <a name="option-2"></a> Single monolithic service — all functionality in one deployable unit with a shared domain model and a shared data store
3. <a name="option-3"></a> Two services — TLA Manager and TLA Resolver as separate backends; report generation embedded inside the Manager

## <a name="criteria"></a> Criteria

- **Independent scalability of reads and writes**: The public query API is subject to significantly higher load than the approval commands; only very few users are involved in the approval process, while many users query TLAs via the API. Queries and commands must be scalable independently of each other.
- **Team autonomy**: The development team has grown to a size where parallel work on a single codebase causes coordination overhead. The command side and the query side must be ownable by separate teams that can develop, deploy, and release independently.
- **Isolation of the approval workflow**: A more complex approval process with user roles is planned for the command side. This functionality must be developable in isolation without impacting the read-side or other services.
- **Controlled concurrency for long-running background tasks**: PDF report generation is a slow, compute-intensive operation. Under high load, Lambda must not auto-scale uncontrollably; a backpressure mechanism must allow the team to configure how many reports are generated concurrently.
- **Data isolation**: Each service must own its own data store. No service may access another service's database directly, so each context can evolve its data model independently.

## <a name="outcome"></a> Outcome

We decided for [Option 1](#option-1).

The **Manager/Resolver split** directly addresses the independent scalability and team autonomy criteria. Applying the CQRS pattern, the Manager owns the write model — it stores all TLAs in all lifecycle states and hosts the accept workflow — while the Resolver owns an independent read model containing only accepted, public TLAs. Each service has its own DynamoDB table, its own deployment pipeline, and can be scaled and released by a separate team. The Manager is implemented in C#/.NET and the Resolver in Java/Spring, reflecting the respective teams' technology choices.

**TLA Reports** is introduced as a third Bounded Context to address the controlled-concurrency criterion. Report requests are placed on an SQS queue, which gives the team explicit control over the maximum number of Lambda instances processing reports concurrently. This backpressure mechanism is not achievable with EventBridge alone, which delivers events in real time and would trigger unbounded Lambda scaling under load. The Reports service stores generated PDFs in its own S3 bucket and has no access to the Manager's or Resolver's DynamoDB tables.

Option 2 fails the scalability criterion: a shared data store and a single deployment unit make it impossible to scale reads independently from writes. Option 3 fails the controlled-concurrency criterion: embedding asynchronous, slow PDF generation inside the Manager would couple a compute-intensive background process to a latency-sensitive request-handling service, and it would require the Manager to own S3 storage and PDF generation concerns that do not belong to its subdomain.

## <a name="comments"></a> Comments
<a name="comment-1"></a>1. (2026-06-04 09:32:17) : marked decision as decided
