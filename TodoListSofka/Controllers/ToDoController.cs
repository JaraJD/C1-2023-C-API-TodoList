﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListSofka.Data;
using TodoListSofka.Model;

namespace TodoListSofka.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ToDoController : ControllerBase
	{

		private readonly ToDoAPIDbContext dbContext;

		public ToDoController(ToDoAPIDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		[HttpGet]
		public async Task<List<TodoItem>> GetPersonajes()
		{
			//Busca las Tareas que no hayan sido eliminados y los retorna
			var tareaActiva = dbContext.Tareas.Where(r => r.State != false).ToList();
			return tareaActiva;

			//Muestra todos los personajes 
			//return await dbContext.Tareas.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<Object> Get(int id)
		{
			var tarea = await dbContext.Tareas.FirstOrDefaultAsync(m => m.Id == id);
			if (tarea == null)
				return NotFound("El personaje no existe");
			return Ok(tarea);
		}

		[HttpGet("/Prioridad")]
		public async Task<Object> GetPriority()
		{
			var tareaImportante = dbContext.Tareas.Where(r => r.Priority == "Alta").ToList();
			return tareaImportante;
		}

		[HttpPost]
		public async Task<object> Post(ToDoCreateDTO tareaDto)
		{
			var nuevaTarea = new TodoItem();
			nuevaTarea.Title = tareaDto.Title;
			nuevaTarea.Description = tareaDto.Description;
			nuevaTarea.Responsible = tareaDto.Responsible;
			nuevaTarea.Priority = tareaDto.Priority;
			nuevaTarea.IsCompleted = tareaDto.IsCompleted;
			nuevaTarea.State = true;

			dbContext.Add(nuevaTarea);
			await dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPut]
		public async Task<Object> Put(TodoItem itemData)
		{
			if (itemData == null || itemData.Id == 0)
				return BadRequest("El ID no es correcto. ");

			var tarea = await dbContext.Tareas.FindAsync(itemData.Id);
			if (tarea == null)
				return NotFound("El personaje no existe. ");
			if (tarea.State == false)
				return NotFound("El ha sido eliminado. ");
			tarea.Title = itemData.Title;
			tarea.Description = itemData.Description;
			tarea.Responsible = itemData.Responsible;
			tarea.Priority = itemData.Priority;
			tarea.IsCompleted = itemData.IsCompleted;
			await dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPut("/Estado/{id:int}")]
		public async Task<Object> PutEstado(int id, bool estado)
		{
			var tarea = await dbContext.Tareas.FindAsync(id);
			tarea.IsCompleted = estado;
			await dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<Object> Delete(int id)
		{
			var tarea = await dbContext.Tareas.FindAsync(id);
			if (tarea.State == false) return NotFound("El personaje ya ha sido eliminado. ");
			if (tarea == null) return NotFound("ID incorrecto");
			dbContext.Tareas.Remove(tarea);
			dbContext.SaveChanges();
			return Ok();
		}
	}
}
