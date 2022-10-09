using MVCRESTAPI.Modles;

namespace MVCRESTAPI.Data
{
    public interface ICommanderRepo
    {
        bool SaveChanges();
        IEnumerable<Command> GetAllCommands();
        Command GetCommandById(int Id);
        void CreateCommand(Command command);
        void UpdateCommand(Command command);   
        void DeleteCommand(Command command);



    }
}
