using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MVCRESTAPI.Data;
using MVCRESTAPI.Dtos;
using MVCRESTAPI.Modles;
using MVCRESTAPI.Services.CommandService;

namespace MVCRESTAPI.Controllers
{

    


    //api/commands
    //[controller ] means the name before Controller word in class name
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController:ControllerBase
    {


        private readonly ICommandService _commandService;
       

        private readonly IMapper _mapper;

        public CommandsController(ICommandService commandService, IMapper mapper)
        {
            _commandService = commandService;
            _mapper = mapper;
        }
        //GET api/commands
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetAllCommands() 
        {
          var commandItems= await _commandService.GetAllCommands();
          return Ok(commandItems);
        }

        //GET api/commands/{id}
        [HttpGet("{id}",Name="GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id) 
        {
            var commandItem = _commandService.GetCommandById(id);
            
            if(commandItem != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(commandItem));
            }
            return NotFound();
        }

        //POST api/commands
        [HttpPost("[action]")]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto) 
        {


            if (commandCreateDto == null)
            {
                return BadRequest("cannot be null");
            }
            var result= _commandService.CreateCommand(commandCreateDto);
            

            var commandReadDto = _mapper.Map<CommandReadDto>(result);

            return CreatedAtRoute(nameof(GetCommandById), new {Id=commandReadDto.Id},commandReadDto);
        }

        //PUT api/commands/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            if(commandUpdateDto == null)
            {
                return BadRequest();
            }
            var result=_commandService.UpdateCommand(id,commandUpdateDto);
            
            return Ok(result);
        }

        

        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            _commandService.DeleteCommand(id);
            return NoContent();
        }


        
    }

}
