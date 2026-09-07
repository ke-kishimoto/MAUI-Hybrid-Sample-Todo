# Offline-first TODO PoC

MAUI Hybrid Blazor + SQLite のローカル完結型クライアントと、Spring Boot + PostgreSQL の同期 API を分離して検証するための PoC です。

現時点ではプロジェクトの実装前設計のみを配置しています。詳細は [アーキテクチャ](docs/architecture.md)、[API 契約](docs/api-contract.md)、[未決定事項](docs/open-questions.md) を参照してください。

## 目標

- オフライン中でも TODO の作成・参照・更新・削除ができる。
- ローカルデータを必要なタイミングでバックエンドへ同期できる。
- Windows / macOS で PoC を検証し、将来の iPhone 対応を妨げない。

## 非目標（PoC 初期）

- 本番用の認証・認可
- リアルタイム双方向同期
- 添付ファイル、共有リスト、通知
- 高可用性・運用監視
