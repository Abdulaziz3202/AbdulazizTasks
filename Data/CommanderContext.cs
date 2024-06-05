using Microsoft.EntityFrameworkCore;
using MVCRESTAPI.Modles;

namespace MVCRESTAPI.Data
{
    public class CommanderContext:DbContext
    {
        public CommanderContext(DbContextOptions<CommanderContext> opt):base(opt){}
        public DbSet<Command> Command { get; set; }



    }
}
