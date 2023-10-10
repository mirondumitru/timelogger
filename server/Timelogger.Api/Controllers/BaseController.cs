using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Timelogger.Foundations.Errors;

namespace Timelogger.Api.Controllers;

public abstract class BaseController : Controller
{
    protected IActionResult HttpResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        if (result.HasErrors<BadRequestError>())
            return BadRequest(result);

        if (result.HasErrors<NotFoundError>())
            return NotFound(result);

        throw new ApplicationException(string.Join("; ", result.Errors.Select(x => x.Message)));
    }
}