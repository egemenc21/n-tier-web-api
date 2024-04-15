using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.Business.Services;
using Server.Model.Dtos;
using Server.Model.Models;

namespace Server.API.Controllers;

public class BaseController<TService, TEntity, TDto> : ControllerBase
    where TService : IBaseService<TEntity>
    where TEntity : class
    where TDto : class
{
    protected readonly TService _service;
    protected readonly IMapper _mapper;

    public BaseController(TService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }
    
    //Read all
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
            
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var data = _mapper.Map<List<TDto>>(await _service.GetAll());

        return Ok(data);
    }
    
    //Read by id
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!_service.Exists(id))
        {
            return NotFound();
        }
            
        var data = _mapper.Map<TDto>(await _service.GetById(id)) ;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
            
        return Ok(data);
    }
    
    //Update
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TDto entityToBeUpdated)
    {
        if (!_service.Exists(id))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityMap = _mapper.Map<TEntity>(entityToBeUpdated);

        if (!await _service.Update(id, entityMap))
        {
            ModelState.AddModelError("", "Something went wrong updating category (id's are not matching)");
            return StatusCode(500, ModelState);
        }

        await _service.Update(id, entityMap);

        return NoContent();
    }
    
    //Delete
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!_service.Exists(id))
        {
            return NotFound();
        }
        
        await _service.Delete(id);
        
        return Ok("Entity has been deleted successfully!");
    }
    
}