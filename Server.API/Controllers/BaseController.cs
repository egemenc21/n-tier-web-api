using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Business.Services;

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
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> GetById(string id)
    {
        if (!await _service.Exists(id))
        {
            return NotFound();
        }

        var data = _mapper.Map<TDto>(await _service.GetById(id.ToString()));

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(data);
    }

    //Update

    [HttpPut("{id}")]
    [Authorize]
    public virtual async Task<IActionResult> Update(string id, [FromBody] TDto entityToBeUpdated)
    {
        if (!await _service.Exists(id))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entityMap = _mapper.Map<TEntity>(entityToBeUpdated);

        Console.WriteLine(entityMap + "_________________________________________");

        try
        {
            if (!await _service.Update(id, entityMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category (id's are not matching)");
                return StatusCode(500, ModelState);
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return StatusCode(500, ModelState);
        }

        return NoContent();
    }


    //Delete

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        if (!await _service.Exists(id))
            return NotFound();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _service.Delete(id);
        }
        catch (KeyNotFoundException e)
        {
            ModelState.AddModelError("", e.Message);
        }


        return NoContent();
    }
}