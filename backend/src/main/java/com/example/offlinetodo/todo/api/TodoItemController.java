package com.example.offlinetodo.todo.api;

import com.example.offlinetodo.todo.service.TodoItemService;
import com.example.offlinetodo.todo.service.model.CreateTodoCommand;
import com.example.offlinetodo.todo.service.model.TodoItem;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.servlet.support.ServletUriComponentsBuilder;

import java.net.URI;
import java.util.List;

@RestController
@RequestMapping("/api/v1/todos")
public class TodoItemController {

    private final TodoItemService todoItemService;

    public TodoItemController(TodoItemService todoItemService) {
        this.todoItemService = todoItemService;
    }

    @GetMapping
    public List<TodoItemResponse> findAll() {
        return todoItemService.findAll().stream()
                .map(TodoItemResponse::from)
                .toList();
    }

    @PostMapping
    public ResponseEntity<CreateTodoResponse> create(@Valid @RequestBody CreateTodoRequest request) {
        var created = todoItemService.create(new CreateTodoCommand(request.getId(), request.getTitle()));
        var location = createLocation(created);

        return ResponseEntity.created(location).body(toResponse(created));
    }

    private URI createLocation(TodoItem todoItem) {
        return ServletUriComponentsBuilder.fromCurrentRequest()
                .path("/{id}")
                .buildAndExpand(todoItem.getId())
                .toUri();
    }

    private CreateTodoResponse toResponse(TodoItem todoItem) {
        return new CreateTodoResponse(
                todoItem.getId(),
                todoItem.getTitle(),
                todoItem.isCompleted(),
                todoItem.getCreatedAt(),
                todoItem.getUpdatedAt(),
                todoItem.getDeletedAt(),
                todoItem.getVersion());
    }
}
