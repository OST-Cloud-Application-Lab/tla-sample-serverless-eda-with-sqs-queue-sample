---
adr_id: "0006"
title: Integrate microservices exclusively via events and messages
status: decided
tags: []
links:
    precedes: []
    succeeds: []
comments: []
---

## <a name="question"></a> Question

How should the TLA microservices (manager, resolver, reports) integrate with each other?

## <a name="options"></a> Options

1. <a name="option-1"></a> Event-Driven Architecture (EDA) — all inter-service communication via asynchronous events (EventBridge) and messages (SQS); no direct service-to-service HTTP calls
2. <a name="option-2"></a> Synchronous REST — services call each other directly via HTTP when they need data or need to trigger an action in another service
3. <a name="option-3"></a> Hybrid — services use synchronous REST for read queries and asynchronous events only for state-change notifications

## <a name="criteria"></a> Criteria

- **Service autonomy**: Each service must be independently deployable and operable; no service should block on a synchronous response from another service.
- **Loose coupling**: Services must not hold references to each other's network endpoints or internal APIs; the only contract between services is the event and message schema.
- **Resilience to partial failure**: A failure or cold-start latency spike in one service must not propagate to other services or cause cascading timeouts.
- **Scalability**: The integration layer must absorb load spikes without creating tight throughput dependencies between services.
- **Alignment with serverless constraints**: Lambda functions are short-lived and stateless; synchronous call chains across independently cold-starting functions amplify latency unpredictably.

## <a name="outcome"></a> Outcome

We decided for [Option 1](#option-1).

All inter-service integration uses either EventBridge domain events or SQS command messages, with no direct HTTP calls between services. 

Option 2 introduces synchronous coupling. If the resolver or reports service is unavailable or cold-starting, the manager request fails or stalls, violating the resilience and autonomy criteria. Option 3 avoids this problem only partially; the query/command boundary erodes over time, and keeping any synchronous inter-service path alive still forces deployment and availability dependencies that EDA eliminates entirely.

## <a name="comments"></a> Comments
<a name="comment-1"></a>1. (2026-06-05 09:45:01) : marked decision as decided
