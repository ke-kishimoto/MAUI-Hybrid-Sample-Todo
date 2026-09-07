# AGENTS.md

このリポジトリは、オフライン優先の TODO アプリケーション PoC です。フロントエンドはローカル SQLite を正とし、必要に応じて Spring Boot API と PostgreSQL に同期します。

## リポジトリ構成（予定）

```text
backend/                 Java / Spring Boot Web API
frontend/                .NET MAUI Hybrid Blazor アプリ
docs/                    設計・開発ドキュメント
docker-compose.yml       PostgreSQL（ローカル開発用）
```

実装を開始する際は、各アプリの公式テンプレートで `backend/` と `frontend/` を作成すること。生成物（`bin/`、`obj/`、`target/`、IDE 設定）はコミットしない。

## 技術方針

- Backend: Java、Spring Boot、Spring Web、Spring Data JPA、Flyway、PostgreSQL。
- Frontend: .NET MAUI Hybrid Blazor、SQLite。初期 PoC は Windows / macOS で動作確認し、iOS 対応可能な MAUI API のみを用いる。
- データの一次保存先は端末内 SQLite。画面操作で API を必須にしない。
- API は JSON / UTF-8、`/api/v1` 配下、UTC の ISO 8601 時刻を使用する。
- スキーマ変更は、PostgreSQL は Flyway migration、SQLite は明示的な migration で管理する。既存データを暗黙に破棄しない。

## TODO の最小データモデル

`TodoItem`

- `id`: UUID（クライアント側で生成）
- `title`: 必須、1〜200文字
- `isCompleted`: 完了状態
- `createdAt`: UTC
- `updatedAt`: UTC
- `deletedAt`: nullable UTC（同期用の論理削除）
- `version`: 競合検出用の整数または更新トークン

物理削除は同期済みの tombstone 保持方針が決まるまで行わない。

## 同期の原則

1. UI 操作は SQLite への反映を先に完了させる。
2. 変更したレコードを同期キューに記録する。
3. 明示操作またはネットワーク復帰時にキューを API へ送信する。
4. 成功した項目だけをキューから完了扱いにする。失敗時のローカル変更は保持する。
5. 競合・再試行・冪等性の詳細は `docs/architecture.md` に従う。未決定の仕様を実装で推測しない。

## 実装規約

- API の契約変更は `docs/api-contract.md` とバックエンド・フロントエンドを同じ変更で更新する。
- DTO と DB Entity を直接共用・露出しない。
- エラーは HTTP ステータスと問題内容が分かる JSON を返す。
- シークレット、接続文字列、実端末のデータをコミットしない。ローカル設定は `.env` または user secrets を利用する。
- 新機能は少なくともユニットテストを追加し、同期処理はオフライン・再試行・競合のケースをテストする。
- 作業前後に既存変更を確認し、利用者の未コミット変更を上書きしない。

## 検証の目安

- Backend: formatter / test / Spring Boot 起動、Flyway migration 適用。
- Frontend: `dotnet build`、SQLite migration、Windows または macOS の実行確認。
- 結合: オフライン CRUD → 接続 → 同期 → 別クライアント相当の更新との競合を確認。

## 未決定事項

認証、ユーザー／テナントの識別、競合解決方式、同期トリガー、削除済みデータの保持期間、バックエンドの配布環境は未決定である。これらが必要な実装に入る前に、`docs/open-questions.md` を更新し利用者に確認すること。
