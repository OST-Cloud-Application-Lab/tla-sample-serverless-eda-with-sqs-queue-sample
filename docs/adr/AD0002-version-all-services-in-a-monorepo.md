---
adr_id: "0002"
title: Version all services in a monorepo
status: decided
tags: []
links:
    precedes: []
    succeeds: []
comments: []
---

## <a name="question"></a> Question

How should the source code of the three services (TLA Manager, TLA Resolver, TLA Reports) and the shared infrastructure be version-controlled?

## <a name="options"></a> Options

1. <a name="option-1"></a> Monorepo: all services and shared infrastructure configuration in a single Git repository, each in its own subdirectory
2. <a name="option-2"></a> Repo per service: each service and the infrastructure in its own dedicated Git repository

## <a name="criteria"></a> Criteria

- **Cross-service visibility**: Changes that affect multiple services (e.g. a shared event schema) must be visible and reviewable in a single pull request.
- **Open-source readiness**: The project is intended to be published as a sample application and referenced in academic work; a single repository is easier to share, fork, and study.
- **Onboarding simplicity**: A new contributor must be able to clone one repository and get a complete picture of the entire system without navigating multiple repositories.
- **Independent deployability**: Despite sharing a repository, each service must remain independently deployable via its own CI/CD pipeline stage.

## <a name="outcome"></a> Outcome

We decided for [Option 1](#option-1).

All three services (`manager`, `resolver`, `reports`) live in a single Git repository, each in a top-level subdirectory with its own `serverless.yml` and CI/CD pipeline stage. This satisfies the open-source readiness criterion directly — the project is intended to be published as a sample and linked in academic publications, and a single repository is far simpler to share and fork than a set of four coordinated repositories. Cross-service changes, such as updates to the event schema exchanged between Manager and Resolver, are visible in one place and reviewable in a single pull request. Onboarding is reduced to a single `git clone`.

Independent deployability is preserved: each subdirectory is a self-contained Serverless service deployed by its own pipeline stage, so a change to the `reports` service does not trigger a redeployment of the `manager`.

Option 2 would offer stronger repository-level isolation but at significant coordination cost: cross-service changes would require synchronized pull requests across multiple repositories, and the project would be harder to share as a self-contained example.

## <a name="comments"></a> Comments
<a name="comment-1"></a>1. (2026-06-04 10:15:44) : marked decision as decided
