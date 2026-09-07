CREATE TABLE todo_items (
    id UUID PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL,
    updated_at TIMESTAMPTZ NOT NULL,
    deleted_at TIMESTAMPTZ NULL,
    version BIGINT NOT NULL DEFAULT 0,
    CONSTRAINT chk_todo_items_title_not_blank CHECK (char_length(btrim(title)) > 0),
    CONSTRAINT chk_todo_items_version_non_negative CHECK (version >= 0)
);

CREATE INDEX idx_todo_items_updated_at ON todo_items (updated_at);
