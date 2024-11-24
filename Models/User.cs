using System;
using System.Collections.Generic;

namespace TodoApp_Backend.Models
{
    public partial class User
    {
        public User()
        {
            Todos = new HashSet<Todo>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public virtual ICollection<Todo> Todos { get; set; }
    }
}
