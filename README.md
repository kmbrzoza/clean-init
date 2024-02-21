# Project Clean Architecture
This is a solution template for creating a Single Page App (SPA) with Angular 17 + .NET 8.0 + Azure B2C

# Setup
1. Ensure that you have valid Data Source (current: ".") in ConnectionString (appsettings.json) in following projects: Insig.Api, Insig.Infrastructure

2. Run database migrations ("dotnet ef database update") in following projects: Insig.Infrastructure

3. Download npm packages ("npm install") in Insig.Web

4. Start web application ("ng serve -o") in Insig.Web

5. Set startup projects:
- Insig.Api

6. Rebuild solution and start application

# Features
**API Project:**
- .NET 8.0
- CQRS - MediatR
- Custom Query Builder for Dapper
- Prepared for Domain-driven design
- IoC Container Autofac
- Database - Microsoft SQL Server
- Use of dates adapted for UTC along with DateTimeProvider and banned use of DateTime.Now and DateTime.UtcNow

**Angular Project:**
- Angular 17
- Angular Material
- Toasts

**Other:**
- Domain tests

**Azure:**
There is a bicepDeploy.yaml file in the Insig.Azure/bicep folder that needs to be uploaded to the pipeline on Azure DevOps. Make sure to add the necessary variables (they are commented out in this file). This pipeline will be used to pre-create services in Azure Portal (app service, static web app, sql server & db, storage and optionally front door).

The Azure Front Door service adds:
- WAF against DDOS attacks
- caching for the web application and storage
- CSP for the web application
- IP restrictions - added prevention access to the app service outside the front door.

# Frontend Architecture

```json
{
    "domains:": {
        "<domain-name>": {
            "data": {}, // contains all models and services domain specific
            "feature-<name>": {}, // only components for specific feature e.g. user list with user search
            "ui-common": {}, // shared components for all domain
            "util-common":{}, // shared service or model for domain
            "util-<name>": {} // typescript helpers domain e.g. dates, validators etc.
        },
        "shared": {
            "data": {}, // global services for app, facades, components
            "ui-common": {}, // cross domain shared components, directives, pipes
            "util-common": {}, // global models cross domain
            "util-<name>": {}, // global utils cross domain
        },
        "shell": {
            "admin": {}, // core components/wrapper for admin panel e.g. sidebar, header etc.
            "site": {}, // core components/wrapper for site e.g. header, footer etc.
            "shared": {} // shared components, pipes etc.
         }
    }
}
```

# Recommended VSC settings

```json
{
    "editor.formatOnSave": true,
    "editor.quickSuggestions": {
        "other": true,
        "comments": false,
        "strings": true
    },
    "editor.codeActionsOnSave": {
        "source.fixAll.eslint": "explicit"
    },
    "eslint.format.enable": true,
    "eslint.validate": [
        "javascript"
    ],
    "beautify.options": {
        "end_with_newline": true,
        "preserve_newlines": true
    },
    "typescriptHero.imports.organizeOnSave": false,
    "typescriptHero.imports.stringQuoteStyle": "'",
    "html.format.wrapAttributes": "force-aligned",
    "typescriptHero.imports.multiLineTrailingComma": false,
    "workbench.tree.indent": 16,
    "[css]": {
        "editor.defaultFormatter": "michelemelluso.code-beautifier"
    },
    "[scss]": {
        "editor.defaultFormatter": "vscode.css-language-features"
    },
    "[javascript]": {
        "editor.defaultFormatter": "vscode.typescript-language-features"
    },
    "[typescript]": {
        "editor.defaultFormatter": "vscode.typescript-language-features"
    },
    "[json]": {
        "editor.defaultFormatter": "vscode.json-language-features"
    },
    "[jsonc]": {
        "editor.defaultFormatter": "vscode.json-language-features"
    },
    "[html]": {
        "editor.defaultFormatter": "vscode.html-language-features"
    }
}
```
