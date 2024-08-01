using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MVCRESTAPI.Dtos;
using MVCRESTAPI.EntityFrameworkCore;
using MVCRESTAPI.Models;

namespace MVCRESTAPI.Services.CommandService
{
    public class CommandService : ICommandService
    {
        private readonly CommanderContext _context;
        private readonly IMapper _mapper;

        public CommandService(CommanderContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public CommandReadDto CreateCommand(CommandCreateDto commandCreateDto)
        {
            try
            {

                
                var objectToAdd = _mapper.Map<Command>(commandCreateDto);

                _context.Add(objectToAdd);
                _context.SaveChanges();

                var addedObject = _mapper.Map<CommandReadDto>(objectToAdd);

                return addedObject;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.InnerException?.Message);
            }
        }

        public int DeleteCommand(int id)
        {
            try
            {
                // Find the command entity by its ID
                var commandToDelete = _context.Command.Find(id);

                if (commandToDelete == null)
                {
                    // Command with the given ID doesn't exist, return 0 to indicate no deletion
                    return 0;
                }

                // Remove the command from the context
                _context.Command.Remove(commandToDelete);

                // Save changes to the database
                _context.SaveChanges();

                // Return 1 to indicate successful deletion
                return 1;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"Error occurred while deleting command with ID {id}: {ex.Message}", ex);
            }
        }


        public async Task<IEnumerable<CommandReadDto>> GetAllCommands()
        {


            try
            {
                var result = await _context.Command.AsNoTracking().ToListAsync();
                return _mapper.Map<List<CommandReadDto>>(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.InnerException?.Message);
            }
        }

        public CommandReadDto GetCommandById(int id)
        {
            try
            {
                // Find the command entity by its ID
                var command = _context.Command.Find(id);

                if (command == null)
                {
                    // Command with the given ID doesn't exist, return null
                    return null;
                }

                // Map the command entity to CommandReadDto using AutoMapper
                var commandReadDto = _mapper.Map<CommandReadDto>(command);

                return commandReadDto;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"Error occurred while retrieving command with ID {id}: {ex.Message}", ex);
            }
        }

       
        public CommandReadDto UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            try
            {
                // Find the command entity by its ID
                var command = _context.Command.Find(id);

                if (command == null)
                {
                    // Command with the given ID doesn't exist, return null
                    return null;
                }

                // Update properties of the command entity with data from CommandUpdateDto
                _mapper.Map(commandUpdateDto, command);

                // Save changes to the database
                _context.SaveChanges();

                // Map the updated command entity to CommandReadDto
                var updatedCommandReadDto = _mapper.Map<CommandReadDto>(command);

                return updatedCommandReadDto;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"Error occurred while updating command with ID {id}: {ex.Message}", ex);
            }
        }

    }
}
