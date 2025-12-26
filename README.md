# 🍳 FuFood - 智慧冰箱庫存管理系統

FuFood 是一個現代化的冰箱食材庫存管理後端 API 服務，幫助使用者追蹤冰箱內的食材、管理庫存進出紀錄，並有效減少食物浪費。

## ✨ 功能特色

- **🔐 LINE OAuth 登入** - 透過 LINE 帳號快速登入，無需額外註冊
- **🧊 多冰箱管理** - 支援建立多個冰箱群組，輕鬆管理不同空間的食材
- **📦 庫存追蹤** - 記錄食材入庫、消耗，掌握庫存變化
- **📅 效期管理** - 追蹤食材有效期限，避免過期浪費
- **🏷️ 分類系統** - 食材分類包含乳製品、蔬果類、肉品類、海鮮類等 10 種類別
- **📊 API 文件** - 內建 Swagger UI 提供完整 API 文件

## 🛠️ 技術棧

| 類別 | 技術 |
|------|------|
| **框架** | ASP.NET Core 10 (Preview) |
| **語言** | C# 13 |
| **資料庫** | PostgreSQL 18 |
| **ORM** | Entity Framework Core 10 |
| **認證** | JWT (JSON Web Token) |
| **API 文件** | Swagger / OpenAPI |
| **容器化** | Docker |
| **CI/CD** | AWS CodeBuild / CodeDeploy |

## 📁 專案結構

```
FuFood/
├── Controllers/          # API 控制器
│   ├── RefrigeratorController.cs    # 冰箱 CRUD
│   ├── InventoryController.cs       # 庫存查詢
│   ├── InventoryTransactionController.cs  # 進出庫交易
│   ├── ProductsController.cs        # 產品管理
│   ├── LineOAuthController.cs       # LINE 登入
│   └── ...
├── Models/
│   ├── Entities/         # 資料庫實體
│   │   ├── User.cs                  # 使用者
│   │   ├── Refrigerator.cs          # 冰箱
│   │   ├── Product.cs               # 產品
│   │   ├── InventoryTransaction.cs  # 庫存交易
│   │   └── InventoryTransactionItem.cs  # 交易項目
│   ├── Enums/            # 列舉型別
│   │   ├── ProductCategory.cs       # 產品分類
│   │   └── UnitType.cs              # 單位類型
│   └── Requests/         # API 請求模型
├── Repositories/         # 資料存取層
├── Services/             # 業務邏輯服務
│   ├── LineOAuthService.cs          # LINE OAuth 整合
│   ├── JwtService.cs                # JWT 處理
│   └── CryptoService.cs             # 加密服務
├── Data/
│   └── AppDbContext.cs   # EF Core DbContext
└── Migrations/           # 資料庫遷移檔案
```

## 🚀 快速開始

### 前置需求

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (或使用 [mise](https://mise.jdx.dev/))
- [Docker](https://www.docker.com/) (用於執行 PostgreSQL)
- LINE Developers 帳號 (用於 OAuth 設定)

### 安裝步驟

1. **複製專案**
   ```bash
   git clone <repository-url>
   cd FuFood
   ```

2. **啟動 PostgreSQL 資料庫**
   ```bash
   cd FuFood
   docker compose up -d
   ```

3. **設定環境變數**
   
   建立 `appsettings.Development.json` 或使用 User Secrets：
   ```json
   {
     "ConnectionStrings": {
       "AppDbContext": "Host=localhost;Port=5487;Username=postgres;Password=postgres;Database=fufood_dev"
     },
     "LineOAuth": {
       "ClientId": "<your-line-channel-id>",
       "ClientSecret": "<your-line-channel-secret>",
       "RedirectUri": "http://localhost:5000/oauth/line/callback"
     },
     "Crypto": {
       "JwtSecret": "<your-jwt-secret-key>"
     }
   }
   ```

4. **執行資料庫遷移**
   ```bash
   dotnet ef database update
   ```

5. **啟動應用程式**
   ```bash
   dotnet run --project FuFood
   ```

6. **開啟 API 文件**
   
   啟動後訪問 http://localhost:5000/swagger 查看 API 文件

### 初始化種子資料

```bash
dotnet run --project FuFood -- seed
```

## 📚 API 端點

### 認證

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/oauth/line/init` | 初始化 LINE OAuth 登入流程 |
| GET | `/oauth/line/callback` | LINE OAuth 回調處理 |

### 冰箱管理

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/api/v1/refrigerators` | 取得使用者的所有冰箱 |
| GET | `/api/v1/refrigerators/{id}` | 取得單一冰箱詳情 |
| POST | `/api/v1/refrigerators` | 建立新冰箱 |
| PUT | `/api/v1/refrigerators/{id}` | 更新冰箱資訊 |
| DELETE | `/api/v1/refrigerators/{id}` | 刪除冰箱 |

### 庫存管理

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/api/v1/refrigerators/{id}/inventory` | 查詢冰箱庫存 |
| GET | `/api/v1/refrigerators/{id}/products/{productId}` | 查詢產品詳情 |

### 庫存交易

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/api/v1/inventory-transactions` | 取得交易列表 |
| POST | `/api/v1/inventory-transactions` | 建立新交易 |
| PUT | `/api/v1/inventory-transactions/{id}` | 更新交易 |
| DELETE | `/api/v1/inventory-transactions/{id}` | 刪除交易 |

## 🐳 Docker 部署

### 建置 Docker 映像

```bash
docker build -t fufood-backend -f FuFood/Dockerfile .
```

### 執行容器

```bash
docker run -p 8080:8080 \
  -e ConnectionStrings__AppDbContext="<your-connection-string>" \
  -e LineOAuth__ClientId="<your-client-id>" \
  -e LineOAuth__ClientSecret="<your-client-secret>" \
  -e Crypto__JwtSecret="<your-jwt-secret>" \
  fufood-backend
```

## 🏗️ CI/CD

專案使用 AWS CodePipeline 進行持續整合與部署：

- **buildspec.yml** - AWS CodeBuild 設定，建置 Docker 映像並推送至 ECR
- **appspec.yml** - AWS CodeDeploy 設定，自動部署至 EC2 實例

## 📝 產品分類

系統支援以下食材分類：

- 乳製品 (Dairy)
- 蔬果類 (Vegetable)
- 水果類 (Fruit)
- 肉品類 (Meat)
- 海鮮類 (Seafood)
- 冷凍食品 (Frozen)
- 乳品飲料類 (Beverage)
- 點心類 (Snack)
- 熟食類 (Prepared)
- 乾貨醬料類 (Condiment)

## 📄 授權條款

此專案為私人專案。

---

Made with ❤️ by FuFood Team
