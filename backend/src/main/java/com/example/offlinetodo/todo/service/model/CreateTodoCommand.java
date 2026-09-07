package com.example.offlinetodo.todo.service.model;

import java.util.UUID;

public class CreateTodoCommand {

    private final UUID id;
    private final String title;

    public CreateTodoCommand(UUID id, String title) {
        this.id = id;
        this.title = title;
    }

    public UUID getId() { return id; }
    public String getTitle() { return title; }
}
