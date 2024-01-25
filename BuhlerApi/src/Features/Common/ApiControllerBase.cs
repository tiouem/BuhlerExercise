using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BuhlerApi.Features.Common;

[ApiController]
[Route("api/[controller]")]
public class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>()!;
}