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

初回は NuGet パッケージの復元に時間がかかることがあります。MAUI のデスクトップウィンドウが開き、テンプレートのホーム画面が表示されれば起動成功です。Docker や Spring Boot API の起動は、この段階では不要です。

ビルドだけを確認する場合は、次を実行します。

```powershell
dotnet build -f net10.0-windows10.0.19041.0
```

## SQLite

アプリケーション起動時、`SqliteDatabase` が端末内の `FileSystem.AppDataDirectory` に `todo.db3` を作成し、`todo_items` テーブルを初期化します。SQLite は端末ごとのローカルファイルであり、Docker Compose では起動しません。

`LocalTodoItem` は同期先の TODO データに合わせて、ID、タイトル、完了状態、作成・更新・論理削除日時、バージョンを保持します。

アプリを一度起動すると DB ファイルが作成されます。Windows で作成されたファイルを探すには、次を実行します。

```powershell
Get-ChildItem -Path $env:LOCALAPPDATA -Filter todo.db3 -Recurse -ErrorAction SilentlyContinue
```

現時点では SQLite の初期化だけを実装しており、TODO の登録・一覧画面は次の実装段階で追加します。
