# Backend

Java 21 / Spring Boot 4.1.1 の TODO 同期 API です。スキーマは Flyway で管理します。

## 前提条件

- Java 21
- Maven（または Maven Wrapper）
- リポジトリ直下の `docker-compose.yml` で起動した PostgreSQL

Maven を実行する Java は 21 に設定してください。Windows で確認する場合は `mvn -version` の `Java version` が `21` であることを確認します。

## データベースと migration の実行

1. リポジトリ直下で PostgreSQL を起動します。

   ```powershell
   docker compose up -d
   ```

2. このディレクトリでアプリケーションを起動します。

   ```powershell
   .\mvnw.cmd spring-boot:run
   ```

   Maven Wrapper が使えない場合は `mvn spring-boot:run` を実行します。

アプリケーションの起動時に Flyway が `src/main/resources/db/migration` の SQL をバージョン順に実行します。実行済み migration は PostgreSQL の `flyway_schema_history` テーブルで管理され、同じ migration は再実行されません。

初期 migration は `V1__create_todo_items.sql` で、TODO の ID、タイトル、完了状態、作成・更新・論理削除日時、楽観ロック用バージョンを作成します。

## 接続設定の上書き

既定値はルートの Docker Compose と一致します。必要なら以下の環境変数で上書きできます。

| 環境変数 | 既定値 |
| --- | --- |
| `DATABASE_URL` | `jdbc:postgresql://localhost:5432/offline_todo` |
| `DATABASE_USERNAME` | `offline_todo` |
| `DATABASE_PASSWORD` | `offline_todo_dev` |

新しいスキーマ変更は、既存の migration を編集せず、`V2__説明.sql` のような新規ファイルとして追加してください。

## 登録 API

`POST /api/v1/todos` で新規 TODO を登録できます。`id` はオフライン作成時にも一意性を保てるよう、クライアントで UUID を生成して送信します。

```json
{
  "id": "f1f2a5b1-0000-4000-8000-000000000001",
  "title": "牛乳を買う"
}
```

成功時は `201 Created` と作成済み TODO を返します。`title` は必須で、200 文字以内です。

## Web 画面

アプリケーション起動後、[http://localhost:8080/todos/new](http://localhost:8080/todos/new) を開くと、TODO を登録する最小限の Web フォームを利用できます。[http://localhost:8080/todos](http://localhost:8080/todos) では全 TODO を一覧表示できます。Web 画面と API はどちらも同じ Service / Repository を使用します。
