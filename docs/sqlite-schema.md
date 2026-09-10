# SQLite schema and migrations

## 方針

SQLite のスキーマ変更は、アプリ起動時に適用するバージョン付き migration で管理します。利用者が端末ごとに SQL を手動実行する方式にはしません。

- migration の DDL は `frontend/Data/Migrations` に保存する。
- 適用済みバージョンは `__schema_migrations` に記録する。
- 適用済み migration は変更せず、変更が必要な場合は次のバージョンを追加する。
- 各 migration の DDL と履歴登録は同じトランザクションで行う。
- 既存データを暗黙に削除しない。

## テーブル

### `todo_items`

画面が直接読み書きするローカル TODO の正本です。

| 列 | SQLite型 | 用途 |
| --- | --- | --- |
| `id` | `TEXT` | クライアント生成 UUID、主キー |
| `title` | `TEXT` | 1〜200文字のタイトル |
| `is_completed` | `INTEGER` | 0: 未完了、1: 完了 |
| `created_at` | `INTEGER` | UTC の `DateTime` ticks |
| `updated_at` | `INTEGER` | UTC の `DateTime` ticks |
| `deleted_at` | `INTEGER NULL` | 論理削除日時 |
| `version` | `INTEGER` | サーバーとの競合検出用バージョン |

### `sync_operations`

PostgreSQLへまだ反映できていない操作を保持する outbox です。行が存在する間は未同期で、APIが成功した行だけを削除します。

| 列 | SQLite型 | 用途 |
| --- | --- | --- |
| `id` | `TEXT` | 操作ごとの UUID。冪等性キーとして利用する |
| `todo_item_id` | `TEXT` | 対象 TODO。`todo_items.id` への外部キー |
| `operation_type` | `TEXT` | `create`、将来の `update` / `delete` |
| `base_version` | `INTEGER NULL` | 更新・削除時の競合検出に使う元バージョン |
| `enqueued_at` | `INTEGER` | キューへ追加したUTC日時 |
| `attempt_count` | `INTEGER` | 送信試行回数 |
| `last_attempt_at` | `INTEGER NULL` | 最終試行日時 |
| `last_error` | `TEXT NULL` | 最後の失敗内容 |

TODO登録処理では、`todo_items` の INSERT と `sync_operations` の INSERT を必ず同じSQLiteトランザクションで行います。これにより、画面には保存されたのに同期対象から漏れる状態を防ぎます。

## migration の追加

次の変更では `ISqliteMigration` を実装するクラスを追加し、`SqliteMigrationRunner` の一覧へ登録します。バージョン番号は重複させず、既存 migration のDDLは書き換えません。
