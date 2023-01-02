﻿using CSharpFunctionalExtensions;
using HRManagement.Api.Models;
using HRManagement.Common.Domain;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Api.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<>))]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<>))]
public abstract class CommonController : ControllerBase
{
    protected ActionResult FormatResponse<TData>(Result<TData> result)
    {
        return Ok(ApiResponse<TData>.Ok(result.Value));
    }

    protected ActionResult FormatResponse<TData>(Result<TData, Error> result)
    {
        var (isSuccess, _, value, error) = result;
        return isSuccess ? Ok(ApiResponse<TData>.Ok(value)) : BadRequest(ApiResponse<TData>.Error(error));
    }

    protected ActionResult FormatResponse<TData>(Result<TData, List<Error>> result)
    {
        var (isSuccess, _, value, errors) = result;
        return isSuccess ? Ok(ApiResponse<TData>.Ok(value)) : BadRequest(ApiResponse<TData>.Error(errors));
    }
}