using System.Net;
using System.Text.Encodings.Web;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ozone.Common.Functional;

namespace Ozone.Common.Testing.Api.Assertions;

public static class HttpResponseAssertions {
  public static void ShouldBeErrorRedirect(this HttpResponseMessage response, Uri url, Error error) {
    var errorUrl = new Uri(url, $"?error={error.Code}&error_description={UrlEncoder.Default.Encode(error.Message)}");
    if (response.StatusCode is HttpStatusCode.NotFound) {
      AssertRequestIsRedirect(response.RequestMessage, errorUrl);
    }
  }

  public static void ShouldBeRedirectTo(this HttpResponseMessage response, Uri url) {
    // When we redirect from our controller, out redirect
    // will fail, because the uri provided does not exist.
    // Therefore we need to capture the uri from the redirect
    // request and match it with the uri provided.
    if (response.StatusCode is HttpStatusCode.NotFound) {
      AssertRequestIsRedirect(response.RequestMessage, url);
    }
  }

  private static void AssertRequestIsRedirect(HttpRequestMessage request, Uri url) {
    Execute.Assertion
      .ForCondition(request.RequestUri.ToString() == url.ToString())
      .FailWith($"Expected request to be redirected to {url}, but was {request.RequestUri}");
  }

  public static AndConstraint<BadRequestAssertions> HaveErrorResult(
    this BadRequestAssertions response,
    Error error
  ) {
    response.BeAs(
      GetProblemDetails(error, StatusCodes.Status400BadRequest)
    );

    return new AndConstraint<BadRequestAssertions>(response);
  }

  private static ProblemDetails GetProblemDetails(Error error, int? statusCode) {
    return new ProblemDetails {
      Title = "Bad Request",
      Type = error.Code,
      Detail = error.Message,
      Status = statusCode,
      Extensions = { { "errors", null } }
    };
  }
}