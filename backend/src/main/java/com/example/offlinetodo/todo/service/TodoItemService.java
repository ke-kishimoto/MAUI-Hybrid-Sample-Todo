package com.example.offlinetodo.todo.service;

import com.example.offlinetodo.todo.domain.TodoItemEntity;
import com.example.offlinetodo.todo.repository.TodoItemRepository;
import com.example.offlinetodo.todo.service.model.CreateTodoCommand;
import com.example.offlinetodo.todo.service.model.TodoItem;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.Instant;
import java.util.List;

@Service
public class TodoItemService {

    private final TodoItemRepository todoItemRepository;

    public TodoItemService(TodoItemRepository todoItemRepository) {
        this.todoItemRepository = todoItemRepository;
    }

    @Transactional
    public TodoItem create(CreateTodoCommand command) {
        var entity = TodoItemEntity.create(command.getId(), command.getTitle(), Instant.now());
        var saved = todoItemRepository.save(entity);

        return new TodoItem(
                saved.getId(),
                saved.getTitle(),
                saved.isCompleted(),
                saved.getCreatedAt(),
                saved.getUpdatedAt(),
                saved.getDeletedAt(),
                saved.getVersion());
    }

    @Transactional(readOnly = true)
    public List<TodoItem> findAll() {
        return todoItemRepository.findAllByDeletedAtIsNullOrderByCreatedAtDesc().stream()
                .map(this::toTodoItem)
                .toList();
    }

    private TodoItem toTodoItem(TodoItemEntity entity) {
        return new TodoItem(
                entity.getId(),
                entity.getTitle(),
                entity.isCompleted(),
                entity.getCreatedAt(),
                entity.getUpdatedAt(),
                entity.getDeletedAt(),
                entity.getVersion());
    }
}
