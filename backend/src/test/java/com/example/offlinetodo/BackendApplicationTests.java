package com.example.offlinetodo;

import com.example.offlinetodo.todo.repository.TodoItemRepository;
import com.example.offlinetodo.todo.service.TodoItemService;
import com.example.offlinetodo.todo.service.model.CreateTodoCommand;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.util.UUID;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

@ExtendWith(MockitoExtension.class)
class BackendApplicationTests {

	@Mock
	private TodoItemRepository todoItemRepository;

	@InjectMocks
	private TodoItemService todoItemService;

	@Test
	void createsAnIncompleteTodoFromTheServiceCommand() {
		var id = UUID.fromString("f1f2a5b1-0000-4000-8000-000000000001");
		when(todoItemRepository.save(any())).thenAnswer(invocation -> invocation.getArgument(0));

		var created = todoItemService.create(new CreateTodoCommand(id, "牛乳を買う"));

		assertThat(created.getId()).isEqualTo(id);
		assertThat(created.getTitle()).isEqualTo("牛乳を買う");
		assertThat(created.isCompleted()).isFalse();
		assertThat(created.getCreatedAt()).isNotNull();
		assertThat(created.getUpdatedAt()).isEqualTo(created.getCreatedAt());
		verify(todoItemRepository).save(any());
	}

}
