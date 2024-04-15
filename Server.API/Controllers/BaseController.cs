using AutoMapper;
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
}