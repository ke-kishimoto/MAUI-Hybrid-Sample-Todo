package com.example.offlinetodo.todo.web;

import com.example.offlinetodo.todo.service.TodoItemService;
import com.example.offlinetodo.todo.service.model.CreateTodoCommand;
import jakarta.validation.Valid;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.UUID;

@Controller
@RequestMapping("/todos")
public class TodoItemWebController {

    private final TodoItemService todoItemService;

    public TodoItemWebController(TodoItemService todoItemService) {
        this.todoItemService = todoItemService;
    }

    @GetMapping
    public String list(Model model) {
        var todos = todoItemService.findAll().stream()
                .map(TodoListItemView::from)
                .toList();

        model.addAttribute("todos", todos);
        return "todos/list";
    }

    @GetMapping("/new")
    public String newForm(@RequestParam(required = false) UUID created, Model model) {
        var form = new TodoCreateForm();
        form.setId(UUID.randomUUID());

        model.addAttribute("todoCreateForm", form);
        model.addAttribute("created", created != null);
        return "todos/new";
    }

    @PostMapping
    public String create(@Valid @ModelAttribute TodoCreateForm todoCreateForm, BindingResult bindingResult) {
        if (bindingResult.hasErrors()) {
            return "todos/new";
        }

        var created = todoItemService.create(
                new CreateTodoCommand(todoCreateForm.getId(), todoCreateForm.getTitle()));

        return "redirect:/todos/new?created=" + created.getId();
    }
}
