package com.example.offlinetodo.todo.api;

import com.example.offlinetodo.todo.service.model.TodoItem;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.time.Instant;
import java.util.UUID;

public class TodoItemResponse {

    private final UUID id;
    private final String title;
    private final boolean completed;
    private final Instant createdAt;
    private final Instant updatedAt;
    private final Instant deletedAt;
    private final long version;

    public TodoItemResponse(UUID id, String title, boolean completed, Instant createdAt,
                            Instant updatedAt, Instant deletedAt, long version) {
        this.id = id;
        this.title = title;
        this.completed = completed;
        this.createdAt = createdAt;
        this.updatedAt = updatedAt;
        this.deletedAt = deletedAt;
        this.version = version;
    }

    public static TodoItemResponse from(TodoItem todoItem) {
        return new TodoItemResponse(
                todoItem.getId(),
                todoItem.getTitle(),
                todoItem.isCompleted(),
                todoItem.getCreatedAt(),
                todoItem.getUpdatedAt(),
                todoItem.getDeletedAt(),
                todoItem.getVersion());
    }

    public UUID getId() { return id; }
    public String getTitle() { return title; }
    @JsonProperty("isCompleted")
    public boolean isCompleted() { return completed; }
    public Instant getCreatedAt() { return createdAt; }
    public Instant getUpdatedAt() { return updatedAt; }
    public Instant getDeletedAt() { return deletedAt; }
    public long getVersion() { return version; }
}
