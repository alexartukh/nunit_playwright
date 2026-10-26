# DRAFT — Playwright + NUnit C# tester

> **Status: early learning draft (August 2026).**
> Kept as a record of my progress. My current, cleaner end-to-end work is in
> [alexartukh/playwright](https://github.com/alexartukh/playwright) — Playwright + TypeScript with page objects, fixtures, API calls, network mocking and CI.

## What it is

The same order scenario that I first wrote in Playwright for TypeScript, ported to **C# and NUnit**
to learn the .NET flavour of Playwright. The tests run against a locally hosted e-commerce shop
(CosmoShop, German storefront) in a Vagrant virtual machine.

## Contents

| File | What it shows |
|---|---|
| `CosmoShopTests.cs` | `PlaceOrderTests : PageTest` — a negative registration test (required fields) and a full order flow up to the thank-you page; the order number is extracted with a regular expression. |
| `ExampleTests.cs` | The official Playwright starter tests against playwright.dev (these run anywhere). |
| `Calculator.cs` | Plain NUnit basics: `[SetUp]`, `[TearDown]`, Arrange–Act–Assert, parameterized `[TestCase]`. |
| `.runsettings` | Browser and launch options (Firefox, visible browser, slow motion for demos). |

## What is interesting here

- Playwright's .NET API: `PageTest` base class, `Expect(...)` assertions, `ContextOptions()` override (ignoring HTTPS errors of the local VM).
- Text attachments added to NUnit results via `TestContext.AddTestAttachment` — an analogue of `testInfo.attach` in Playwright Test.
- Browser configuration through `.runsettings` instead of code.

## Known limitations (why it is a draft)

- The shop tests run only against my local VM.
- Fixed waits (`WaitForTimeoutAsync`), `NetworkIdle` navigation and assertions on the whole `body` —
  approaches I later replaced with waiting for specific elements and responses.
- Selectors depend on German UI texts; no page objects.

## Running

```bash
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install     # download browsers once
dotnet test --settings .runsettings
dotnet test --filter "FullyQualifiedName~ExampleTests"   # only the tests that do not need the local shop
```

## Stack

C# · .NET 10 · NUnit 4 · Microsoft.Playwright.NUnit
