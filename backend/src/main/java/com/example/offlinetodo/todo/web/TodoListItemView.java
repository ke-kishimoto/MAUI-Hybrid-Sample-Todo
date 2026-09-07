package com.example.offlinetodo.todo.web;

import com.example.offlinetodo.todo.service.model.TodoItem;

public class TodoListItemView {

    private final String title;
    private final boolean completed;

    private TodoListItemView(String title, boolean completed) {
        this.title = title;
        this.completed = completed;
    }

    public static TodoListItemView from(TodoItem todoItem) {
        return new TodoListItemView(todoItem.getTitle(), todoItem.isCompleted());
    }

    public String getTitle() { return title; }
    public boolean isCompleted() { return completed; }
}
