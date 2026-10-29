# iKim Distribution Mobile Client

A cross-platform **.NET MAUI** mobile application built for iKim's distribution operations. It enables field staff and distributors to place and manage orders, track deliveries, record daily sales, check stock, and view customer accounts, with data synced to the **iKim Web API**.

| | |
|---|---|
| **App name** | Ikim Connect |
| **Application ID** | `com.SFIT.ikimmobileclient` |
| **Version** | 1.0 |
| **Framework** | .NET 9 / .NET MAUI |
| **Current target** | Android (minimum API 21) |

---

## Table of Contents

1. [Features](#features)
2. [Tech Stack](#tech-stack)
3. [Project Structure](#project-structure)
4. [Getting Started](#getting-started)
5. [Configuration](#configuration)
6. [Build and Run](#build-and-run)
7. [Android Signing](#android-signing)
8. [App Navigation](#app-navigation)
9. [Roadmap](#roadmap)
10. [Contributing](#contributing)
11. [License](#license)

---

## Features

- **Secure authentication**: login, forgot password, and OTP verification
- **Dashboard**: at-a-glance summary with charts
- **Order management**: create new orders and party orders, and browse existing ones
- **Catalog**: category listing, category detail, item selection, and add-to-cart
- **Delivery tracking**: dedicated delivery view
- **Daily sales**: add and review daily sales entries
- **Customer accounts and addresses**: manage address details through popups and bottom sheets
- **Notifications**: notification list and detail screens
- **User profile**
- **Local storage**: SQLite support for offline-friendly data handling
- **Polished UX**: custom loader, reusable alert popups, bottom sheets, borderless inputs, and custom fonts

## Tech Stack

| Area | Technology |
|---|---|
| Framework | .NET 9, .NET MAUI (single project) |
| Language | C# (nullable reference types, implicit usings) |
| Architecture | MVVM with CommunityToolkit.Mvvm |
| UI | XAML, Syncfusion MAUI, Mopups, The49.Maui.BottomSheet, Acr.UserDialogs |
| Charts | Microcharts.Maui |
| Local data | Microsoft.Data.Sqlite.Core, SQLitePCLRaw |
| Serialization | Newtonsoft.Json |
| Backend | iKim Web API (REST) |

### NuGet Packages

| Package | Version |
|---|---|
| Acr.UserDialogs | 9.2.2 |
| CommunityToolkit.Maui | 11.1.1 |
| CommunityToolkit.Mvvm | 8.3.2 |
| Microcharts.Maui | 1.0.1 |
| Microsoft.Data.Sqlite.Core | 8.0.8 |
| Microsoft.Extensions.Logging.Debug | 9.0.8 |
| Mopups.Maui | 1.0.0 |
| Newtonsoft.Json | 13.0.4 |
| SQLitePCLRaw.bundle_green | 2.1.10 |
| Syncfusion.Maui.Inputs | 34.1.31 |
| Syncfusion.Maui.Toolkit | 1.0.10 |
| The49.Maui.BottomSheet | 8.0.3 |

## Project Structure

```
.
├── APIService/          # API client for the iKim Web API
├── Converter/           # XAML value converters
├── CustomeControls/     # Reusable custom controls (e.g. CustomLoaderView)
├── Handlers/            # Custom MAUI handlers (e.g. borderless Entry)
├── Models/              # Data models
├── Pages/               # Full-screen pages
│   ├── Accounts/        #   Login, ForgotPassword, OTPVerification
│   ├── Catregory/       #   CategoryDetailList, AddToCart
│   ├── Daily Sales/     #   AddDailySales, DailySalesList
│   ├── Dashboard/       #   DashboardPage
│   └── Notifications/   #   NotificationList, NotificationDetail
├── PopupPages/          # Popups (address, alerts, loader, notifications)
├── Views/               # Tab content views (Dashboard, Order, Delivery, Profile, ...)
├── Platforms/           # Platform-specific code
├── Properties/
├── Resources/           # App icon, splash, images, fonts, raw assets
├── Services/            # Application services
├── Utilities/           # Helpers and shared utilities
├── App.xaml / App.xaml.cs
├── GlobalUsings.cs
├── GlobalXmlns.cs
├── MauiProgram.cs       # App bootstrap, handlers, fonts, toolkit registration
├── i-KimMobileClient.csproj
└── i-KimMobileClient.sln
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- .NET MAUI workload:
  ```bash
  dotnet workload install maui
  ```
- Visual Studio 2022 (latest) with the **.NET Multi-platform App UI development** workload, or VS Code with the **.NET MAUI** extension
- Android SDK with an emulator or a physical device (API 21 or higher)
- Access to the iKim Web API (base URL and credentials)

### Installation

```bash
# 1. Clone the repository
git clone <repository-url>
cd iKim-Distribution-Mobile-Client

# 2. Restore dependencies
dotnet restore
```

## Configuration

| Setting | Where | Notes |
|---|---|---|
| API base URL | `APIService/` or `Utilities/` | Point to your iKim Web API environment |
| Syncfusion license key | `MauiProgram.cs` | Register at startup if your Syncfusion packages require it |
| Signing credentials | Build parameters or CI secrets | Never commit passwords or keystores |

## Build and Run

```bash
# Debug build
dotnet build -f net9.0-android -c Debug

# Build and run on a connected device or emulator
dotnet build -f net9.0-android -t:Run

# Release build
dotnet publish -f net9.0-android -c Release
```

In Visual Studio, open `i-KimMobileClient.sln`, select an Android emulator or device, and press **F5**.

## Android Signing

Release builds must be signed with your own keystore. Keep the keystore and its credentials out of source control and supply them at build time:

```bash
dotnet publish -f net9.0-android -c Release \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=<path-to-keystore> \
  -p:AndroidSigningKeyAlias=<alias> \
  -p:AndroidSigningStorePass=env:KEYSTORE_PASS \
  -p:AndroidSigningKeyPass=env:KEY_PASS
```

## App Navigation

After login, the app opens a tabbed home (`HomeTabbedPage`) with these views:

| View | Purpose |
|---|---|
| `DashboardView` | Overview and charts |
| `NewOrderView` / `NewPartyOrderView` | Create orders |
| `OrderView` | Browse and manage orders |
| `DeliveryView` | Track deliveries |
| `ProfileView` | User profile |

## Roadmap

- [ ] Enable and test the iOS target
- [ ] Add unit tests for services and view models
- [ ] Set up CI (build and test) with GitHub Actions
- [ ] Move environment configuration and signing credentials out of source control

## Contributing

1. Fork the project
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "Add your feature"`
4. Push the branch: `git push origin feature/your-feature`
5. Open a Pull Request

## License

License to be defined. Add a `LICENSE` file to specify usage terms.
