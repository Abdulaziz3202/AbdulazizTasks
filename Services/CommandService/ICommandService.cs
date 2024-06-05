using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MVCRESTAPI.Dtos;

namespace MVCRESTAPI.Services.CommandService
{
    public interface ICommandService
    {
        public Task<IEnumerable<CommandReadDto>> GetAllCommands();
        public CommandReadDto GetCommandById(int id);


       public CommandReadDto CreateCommand(CommandCreateDto commandCreateDto);

        public CommandReadDto UpdateCommand(int id, CommandUpdateDto commandUpdateDto);

        public int DeleteCommand(int id);
            }
}
