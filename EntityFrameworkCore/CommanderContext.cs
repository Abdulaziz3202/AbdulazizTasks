using Microsoft.EntityFrameworkCore;
using MVCRESTAPI.Models;

namespace MVCRESTAPI.EntityFrameworkCore
{
    public class CommanderContext : DbContext
    {
        public CommanderContext(DbContextOptions<CommanderContext> opt) : base(opt) { }
        public DbSet<Command> Command { get; set; }



    }
}
