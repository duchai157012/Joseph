---
trigger: always_on
---

System Instruction: Senior .NET Developer Rules
1. General Coding Standards

No Unnecessary Comments: Code must be self-documenting. Only use comments to explain "Why" a complex decision was made, never "What" the code is doing.

Fail Fast & Guard Clauses: Validate inputs immediately at the start of methods. Use ArgumentNullException.ThrowIfNull or one-liner checks instead of nested if statements.

Nullable Reference Types: Treat Nullable Reference Types as the standard. Enable <Nullable>enable</Nullable> and treat all nullability warnings as build errors.

2. Architecture & Design Patterns

Strict Dependency Injection: Never use new to instantiate services. Always use Constructor Injection for all dependencies.

Options Pattern: Never access IConfiguration directly in business logic. Use IOptions<T> to inject strongly-typed configuration.

CQRS & Mediator: Decouple write operations (Commands) from read operations (Queries). Prioritize using MediatR to keep controllers thin and focused.

Rich Domain Models: Ensure Entities contain business logic (using private set; and state-modifying methods). strictly avoid "Anemic Domain Models" (classes with only public getters/setters).

3. Asynchronous Programming

Async All The Way: Use async/await for all I/O-bound operations.

Cancellation Tokens: Always propagate CancellationToken in async methods to allow for request cancellation.

Avoid Blocking Calls: Never use .Result or .Wait().

4. Error Handling & Validation

Global Exception Handling: Do not use try-catch blocks in Controllers. Use Middleware to handle exceptions globally and return standardized RFC 7807 Problem Details responses.

FluentValidation: Decouple validation logic from Models/Entities. Use FluentValidation for complex business rules instead of Data Annotations.

5. Unit Testing

AAA Pattern: Strictly follow the Arrange - Act - Assert pattern for all unit tests.

Constraint: Output STRICTLY professional production code.
- NO conversational comments (e.g., "Here is the code", "// This creates a list").
- NO emojis or decorative icons in comments.
- Only include standard XML documentation (`///`) for public methods.

Test Behavior, Not Implementation: Tests must verify the output or state change. Do not test private methods or internal implementation details.