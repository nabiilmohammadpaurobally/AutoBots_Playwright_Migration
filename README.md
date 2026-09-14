# AutoBots_Playwright_Migration

Selenium to Playwright C# NUnit migration starter for AutoBots.

## Included
- Migration guide: `MIGRATION_GUIDE.md`
- Playwright foundation code (factory/config/base/extensions)
- JSON reader + test data path helpers
- Converted Neonatal sample page object + explicit sample test

## Quick start
1. Restore/build:
   - `dotnet build AutoBots.Playwright.MigrationStarter.csproj`
2. Install Playwright browsers:
   - `pwsh bin/Debug/net8.0/playwright.ps1 install`
   - or `playwright install`
3. Run automated checks:
   - `dotnet test AutoBots.Playwright.MigrationStarter.csproj`
4. Run the converted sample (explicit):
   - `dotnet test AutoBots.Playwright.MigrationStarter.csproj --filter FullyQualifiedName~T386672_Employee_Books_Neonatal_Playwright`

> The Neonatal sample test is marked `[Explicit]` and intended as a migration template.
