using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TodoApp_Backend.Models;
using TodoApp_Backend.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        public readonly todoappContext context;
        
        public TodoController(todoappContext context) 
        {
        this.context = context;
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Todo>>> GetTodosbyId(int id)
        {
            var userIdClaim = User.FindFirst("userid");
            if (userIdClaim == null) 
            {
                return Unauthorized("User id not present in token");
            }
            int userId = int.Parse(userIdClaim.Value);

            var data = await context.Todos
                .Where(todo => todo.UserId == id)
                .Select(todo =>
                new TodoDTO
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    Completed = todo.Completed,
                    UserId = todo.UserId,

                }).ToListAsync();


            if(data.Count < 1)
            {
                return NotFound("Todo not found");
            }
            return Ok(data);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<TodoDTO>>> CreateTodo([FromBody]TodoDTO todoDTO)
        {
            var userIdClaim = User.FindFirst("userid");
            if (userIdClaim == null)
            {
                return Unauthorized("User id not present in token");
            }
            int userId = int.Parse(userIdClaim.Value);
            var todo = new Todo
            {
                Title = todoDTO.Title,
                Description = todoDTO.Description,
                Completed = todoDTO.Completed,
                UserId = todoDTO.UserId
            };
            context.Todos.AddAsync(todo);
            await context.SaveChangesAsync();
            todoDTO.Id = todo.Id;
            return Ok(todoDTO);
        }
        [Authorize]
        [HttpPut ("{id}")]
        public async Task<ActionResult<TodoDTO>> UpdateTodo(int id , [FromBody] TodoDTO todoDTO) 
        {
            var userIdClaim = User.FindFirst("userid");
            if (userIdClaim == null)
            {
                return Unauthorized("User id not present in token");
            }
            int userId = int.Parse(userIdClaim.Value);

            var todo = context.Todos.FirstOrDefault(todo => todo.Id == id && todo.UserId == 1);
            if (todo == null)
            {
                return NotFound("Todo not found");
            }
            todo.Description = todoDTO.Description;
            todo.Completed = todoDTO.Completed;
            todo.Title = todoDTO.Title;

            await context.SaveChangesAsync();
            todoDTO.Id = todo.Id;
            return Ok(todoDTO);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Todo>>> DeleteTodo(int id)
        {
            var userIdClaim = User.FindFirst("userid");
            if (userIdClaim == null)
            {
                return Unauthorized("User id not present in token");
            }
            int userId = int.Parse(userIdClaim.Value);

            var data = context.Todos.FirstOrDefault(todo => todo.Id == id && todo.UserId == 1);
            if (data == null)
            {
                return NotFound("Todo not found");
            }
            context.Todos.Remove(data);
            await context.SaveChangesAsync();
            return Ok("Todo deleted sucessfully");
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Todo>>> GetTodosbyId()
        {
            var userIdClaim = User.FindFirst("userid");
            if (userIdClaim == null)
            {
                return Unauthorized("User id not present in token");
            }
            int userId = int.Parse(userIdClaim.Value);

            var data = await context.Todos
                .Where(todo => todo.UserId == userId)
                .Select(todo =>
                new TodoDTO
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    Completed = todo.Completed,
                    UserId = todo.UserId,

                }).ToListAsync();


            if (data.Count < 1)
            {
                return NotFound("Todo not found");
            }
            return Ok(data);
        }
    }
}
