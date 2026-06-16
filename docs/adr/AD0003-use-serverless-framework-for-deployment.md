---
adr_id: "0003"
title: Use Serverless framework to deploy to AWS
status: decided
tags: []
links:
    precedes: []
    succeeds: []
comments: []
---

## <a name="question"></a> Question

How shall we deploy our serverless microservices to the AWS cloud?

## <a name="options"></a> Options

1. <a name="option-1"></a> Serverless Framework
2. <a name="option-2"></a> AWS SAM (Serverless Application Model)
3. <a name="option-3"></a> Plain CloudFormation
4. <a name="option-4"></a> Terraform

## <a name="criteria"></a> Criteria

- **Multi-runtime support**: The tool must support deploying Java (Maven JAR), .NET (ZIP package), and Python Lambda functions from a single project configuration without workarounds.
- **Infrastructure-as-code abstraction**: Complex AWS resources (API Gateway routes, DynamoDB tables, EventBridge rules, SQS triggers) should be expressible in a high-level config without writing raw CloudFormation.
- **Deployment simplicity**: A single command should deploy all functions and infrastructure, keeping the CI/CD pipeline simple.
- **Community and documentation maturity**: Quality of documentation, active community, and availability of examples for the AWS services used in this project.
- **Cost**: No additional licensing cost beyond the AWS infrastructure charges.

## <a name="outcome"></a> Outcome

We decided for [Option 1](#option-1).

The Serverless Framework is the option that meets the multi-runtime requirement out of the box and provides a mature plugin ecosystem. AWS SAM is a strong alternative with better local testing support, but its multi-runtime story is weaker. Terraform and plain CloudFormation were ruled out due to higher configuration overhead for Lambda-centric deployments.

## <a name="comments"></a> Comments
<a name="comment-1"></a>1. (2026-06-04 16:18:03) : marked decision as decided
