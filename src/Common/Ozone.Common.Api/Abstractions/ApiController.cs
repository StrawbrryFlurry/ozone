using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ozone.Common.Functional;

namespace Ozone.Common.Api.Abstractions;

[ApiController]
public abstract class ApiController : ControllerBase {
  protected ApiController(ISender handler, IMapper mapper) {
    Mapper = mapper;
    Handler = handler;
  }

  protected IMapper Mapper { get; }
  protected ISender Handler { get; }

  protected IActionResult HandleBadRequest(string code, string message) {
    return HandleBadRequest(new Error(code, message));
  }

  protected IActionResult HandleBadRequest(Error error) {
    return BadRequest(
      CreateProblemDetails(
        "Bad Request",
        StatusCodes.Status400BadRequest,
        error
      ));
  }

  protected IActionResult HandleBadRequestAsRedirect(Uri redirectUri, Error error) {
    var query = new QueryString();
    query = query.Add("error", error.Code);
    query = query.Add("error_description", error.Message);

    return Redirect($"{redirectUri}{query}");
  }

  protected IActionResult HandleFailure(IResult result) {
    return result switch {
      { IsSuccess: true } => throw new InvalidOperationException(),
      IValidationResult validationResult =>
        BadRequest(
          CreateProblemDetails(
            "Validation Error", StatusCodes.Status400BadRequest,
            result.Error,
            validationResult.Errors)),
      _ =>
        BadRequest(
          CreateProblemDetails(
            "Bad Request",
            StatusCodes.Status400BadRequest,
            result.Error))
    };
  }

  private static ProblemDetails CreateProblemDetails(
    string title,
    int status,
    Error error,
    Error[]? errors = null) {
    return new ProblemDetails {
      Title = title,
      Type = error.Code,
      Detail = error.Message,
      Status = status,
      Extensions = { { nameof(errors), errors } }
    };
  }
}