package com.example.offlinetodo.todo.web;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import java.util.UUID;

public class TodoCreateForm {

    @NotNull
    private UUID id;

    @NotBlank(message = "タイトルを入力してください。")
    @Size(max = 200, message = "タイトルは200文字以内で入力してください。")
    private String title;

    public UUID getId() {
        return id;
    }

    public void setId(UUID id) {
        this.id = id;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }
}
