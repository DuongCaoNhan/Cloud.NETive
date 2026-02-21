# C# Coding Conventions & Style Guidelines

### General Principles
- Correctness: Code must remain correct and resilient, even after edits.
- Teaching: Demonstrate best practices and modern C# features.
- Consistency: Uniform style across all code samples and projects.
- Readability and clarity are prioritized over brevity.

### Language Guidelines
- Prefer modern C# features (records, pattern matching, init-only, etc.).
- Use language keywords: `string` instead of `System.String`, `int` instead of `System.Int32`.
- Default to `int` over unsigned types for consistency.
- Use `var` when the type is obvious, otherwise be explicit.
- Write simple, clear code—avoid overly complex logic.
- Use `async/await` for async operations; avoid blocking calls.
- Use LINQ for collection queries, but keep queries readable.

### String Handling
- Prefer string interpolation (`$"{name}"`) over concatenation.
- Use raw string literals for readability instead of escape sequences.
- Use `StringBuilder` for large/looped concatenations.

### Constructors & Initialization
- Class/struct primary constructor parameters: camelCase.
- Record primary constructor parameters: PascalCase (become public properties).
- Prefer required properties over verbose constructors.
- Use collection expressions when applicable.

### Delegates & Events
- Use `Func<>` and `Action<>` instead of custom delegates.
- Prefer lambdas for event handlers.

### Exception Handling
- Catch specific exceptions, avoid general `Exception`.
- Always dispose resources using `using` or `await using`.

### Operators & Comparisons
- Use `&&` / `||` instead of `&` / `|` for boolean expressions.

### Object Instantiation
- Use concise syntax: `var item = new Example();`
- Prefer object initializers for property setting.

### LINQ Queries
- Use meaningful variable names.
- Align query clauses under the `from` clause.
- PascalCase for anonymous type properties.

### Variable Declarations
- `var` when type is obvious, explicit type when unclear.
- Implicit typing in `for`, explicit typing in `foreach`.

### File Organization
- File-scoped namespaces.
- Place `using` directives outside namespaces.
- One namespace per file.

### Style & Layout Conventions
- Indentation: 4 spaces, no tabs.
- Max line length: 120 characters.
- Braces: Allman style `{ }`, always used.
- One statement/declaration per line.
- One blank line between members.

### Documentation & Comments
- XML `///` doc comments for public APIs.
- Summaries describe intent, not implementation.
- Use `//` for short explanations; avoid `/* */`.
- Comments start with uppercase, end with period.

---

## Identifier Naming Rules
- Must start with a letter or `_`.
- Can include Unicode letters, digits, connectors, combining, formatting characters.
- Keywords allowed with `@` prefix.
- Avoid `__` (reserved for compiler).
- Identifiers must be unique within scope.

### Identifier Naming Conventions
- Namespaces, classes, methods, properties: PascalCase.
- Interfaces: PascalCase with `I` prefix.
- Attributes: end with `Attribute`.
- Enums: singular (non-flags), plural (flags).
- Constants & readonly: PascalCase.
- Parameters & locals: camelCase.
- Private fields: `_camelCase`.
- Static fields: `s_camelCase`; thread-static: `t_camelCase`.
- Use descriptive, meaningful names. Avoid unnecessary abbreviations.

### Type Parameters
- Single type: `T`.
- Descriptive names prefixed with `T` (e.g., `TSession`).
- Apply constraints in names if useful (e.g., `TKey`, `TValue`).

---

## Null Safety
- Enable nullable reference types.
- Use `is null`, `is not null`, `??`, and `?.` operators.

## Testing & Reliability
- Prefer xUnit or NUnit.
- Test names: `Method_State_ExpectedResult`.
- Use Arrange-Act-Assert pattern.

## Linting & Formatting
- Code must pass analyzers (Roslyn, StyleCop).
- Use IDE defaults consistent with Microsoft guidelines.
- Follow configured naming/spacing rules.

## Version Compatibility
- Target .NET 6+ and C# 10+.
- Avoid deprecated APIs.
- Prefer modern language features.

---

## Do Not
- Don’t generate legacy .NET Framework code.
- Don’t hardcode secrets/connection strings.
- Don’t use outdated patterns (`Task.Result`, `async void`, `IHttpActionResult`).
- Don’t catch `Exception` without proper handling.
- Don’t place `using` directives inside namespaces.
- Don’t add TODO/incomplete placeholders.
- Don’t use multi-line `/* */` comments for explanations.
