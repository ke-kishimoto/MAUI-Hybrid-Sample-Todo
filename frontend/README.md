# Frontend

MAUI Hybrid Blazor のオフライン優先 TODO クライアントです。

## Windows での起動

前提条件は .NET 10 SDK と MAUI workload です。未導入の場合は以下を実行します。

```powershell
dotnet workload install maui
```

このディレクトリで、Windows 向けターゲットを指定して起動します。

```powershell
dotnet run -f net10.0-windows10.0.19041.0
```

初回は NuGet パッケージの復元に時間がかかることがあります。MAUI のデスクトップウィンドウが開き、TODO画面が表示されれば起動成功です。ローカル登録と一覧表示にはDockerやSpring Boot APIは不要です。

ビルドだけを確認する場合は、次を実行します。

```powershell
dotnet build -f net10.0-windows10.0.19041.0
```

## SQLite

アプリケーション起動時、`SqliteDatabase` が端末内の `FileSystem.AppDataDirectory` に `todo.db3` を作成し、未適用のバージョン付き migration を順番に実行します。SQLite は端末ごとのローカルファイルであり、Docker Compose では起動しません。

`todo_items` は同期先の TODO データに合わせて、ID、タイトル、完了状態、作成・更新・論理削除日時、バージョンを保持します。`sync_operations` は PostgreSQL へ未送信の操作を保持する outbox（同期キュー）です。適用済み migration は `__schema_migrations` に記録されます。

スキーマと migration の追加手順は [SQLite schema](../docs/sqlite-schema.md) を参照してください。

アプリを一度起動すると DB ファイルが作成されます。Windows で作成されたファイルを探すには、次を実行します。

```powershell
Get-ChildItem -Path $env:LOCALAPPDATA -Filter todo.db3 -Recurse -ErrorAction SilentlyContinue
```

TODOを登録すると、`todo_items` と `sync_operations` が同じSQLiteトランザクションで更新されます。画面の「今すぐ同期」は未送信の作成操作を `POST /api/v1/todos` へ送り、成功した項目だけを同期キューから削除します。失敗した項目は端末内に残るため、backend復旧後に再試行できます。

同期先の既定値は `http://localhost:8080/api/v1/` です。変更する場合は、アプリ起動前に環境変数を設定します。

```powershell
$env:TODO_API_BASE_URL = "http://localhost:8080/api/v1/"
dotnet run -f net10.0-windows10.0.19041.0
```

現在の同期対象は新規登録（create）のみです。リモート一覧の取り込み、更新、完了、削除、競合解決はまだ実装していません。

テストはリポジトリルートで実行します。

```powershell
dotnet test frontend.tests\Frontend.Tests.csproj
```
