package com.example.offlinetodo.todo.repository;

import com.example.offlinetodo.todo.domain.TodoItemEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.UUID;

public interface TodoItemRepository extends JpaRepository<TodoItemEntity, UUID> {

    List<TodoItemEntity> findAllByDeletedAtIsNullOrderByCreatedAtDesc();
}
