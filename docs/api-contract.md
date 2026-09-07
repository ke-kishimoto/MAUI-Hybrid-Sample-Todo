# API 契約（初期案）

ベースパスは `/api/v1`、Content-Type は `application/json` です。

## Todo

```json
{
  "id": "f1f2a5b1-0000-4000-8000-000000000001",
  "title": "牛乳を買う",
  "isCompleted": false,
  "createdAt": "2026-09-07T00:00:00Z",
  "updatedAt": "2026-09-07T00:00:00Z",
  "deletedAt": null,
  "version": 1
}
```

## CRUD

| Method | Path | 説明 |
| --- | --- | --- |
| `GET` | `/todos` | 未削除 TODO の一覧取得 |
| `GET` | `/todos/{id}` | TODO の取得 |
| `PUT` | `/todos/{id}` | 作成または全項目更新。リクエスト本文の `version` を検証する |
| `DELETE` | `/todos/{id}` | 論理削除 |

`PUT` と `DELETE` には再送判定用の `Idempotency-Key` ヘッダーを必須にする案です。競合時は `409 Conflict`、入力不正時は `400 Bad Request` を返します。

## 同期

`POST /sync` は複数のローカル操作をまとめて送るためのエンドポイントです。具体的な request / response 形式は、認証・競合解決・削除保持期間を決めた後に確定します。PoC 初期では CRUD API だけで同期を実装しても構いませんが、クライアント側の同期サービスを UI から分離してください。
