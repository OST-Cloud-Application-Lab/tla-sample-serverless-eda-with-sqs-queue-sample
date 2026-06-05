---
adr_id: "0004"
title: Apply Onion Architecture in the Manager and Resolver microservices
status: decided
tags: []
links:
    precedes: []
    succeeds: []
comments: []
---

## <a name="question"></a> Question

How shall we structure the internal architecture of the TLA Manager and TLA Resolver microservices?

## <a name="options"></a> Options

1. <a name="option-1"></a> Onion Architecture (Domain -> Application -> Infrastructure layers)
2. <a name="option-2"></a> Traditional Layered Architecture (Presentation -> Business Logic -> Data Access)
3. <a name="option-3"></a> Transaction Script (no explicit layer separation, logic directly in handler functions)
4. <a name="option-4"></a> Hexagonal Architecture (Ports and Adapters)

## <a name="criteria"></a> Criteria

- **Domain isolation**: The domain model (entities, value objects, domain exceptions) must not depend on any infrastructure concern such as AWS SDK types, database schemas, or HTTP DTOs.
- **Testability**: Core business logic must be unit-testable without instantiating AWS infrastructure (DynamoDB, EventBridge, SQS).
- **Replaceability of infrastructure**: AWS-specific implementations (DynamoDB repositories, EventBridge publishers, SQS publishers) must be replaceable via interfaces defined in the application layer, without touching domain or application logic.
- **Alignment with DDD principles**: The architecture should naturally accommodate DDD building blocks such as aggregates, value objects, domain exceptions, and application services.
- **Code clarity**: The project structure should make it easy for a new team member to locate domain logic, use cases, and infrastructure adapters separately.

## <a name="outcome"></a> Outcome

We decided for [Option 1](#option-1).

Onion Architecture enforces the dependency rule that outer layers depend on inner layers, never the reverse. This keeps the `Domain` and `Application` projects free of AWS SDK dependencies. Repository and messaging interfaces are defined in the `Application` layer and implemented in the `Infrastructure` layer, allowing the domain and application logic to be tested with simple in-memory fakes. Option 4 (Hexagonal) would achieve similar goals but Onion Architecture maps more directly to the DDD layering conventions the team is familiar with. Option 3 was ruled out as it would entangle AWS Lambda handler concerns with domain logic, making the codebase difficult to test and maintain.

Note: the TLA Reports microservice is intentionally kept as a simple Python script (Option 3) because its responsibility (generating a PDF from accepted TLAs) does not contain complex domain logic that would benefit from layering.

## <a name="comments"></a> Comments
<a name="comment-1"></a>1. (2026-06-04 20:56:22) : marked decision as decided
