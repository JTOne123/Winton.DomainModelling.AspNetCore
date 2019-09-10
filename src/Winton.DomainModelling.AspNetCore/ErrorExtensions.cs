// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Winton.DomainModelling.AspNetCore
{
    internal static class ErrorExtensions
    {
        internal static ActionResult ToActionResult(this Error error, Func<Error, ProblemDetails> selectProblemDetails)
        {
            ProblemDetails problemDetails = selectProblemDetails?.Invoke(error) ?? ProblemDetails(error);
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        private static ProblemDetails ProblemDetails(Error error)
        {
            ProblemDetails DefaultProblemDetails(int statusCode)
            {
                return new ProblemDetails
                {
                    Detail = error.Detail,
                    Status = statusCode,
                    Title = error.Title,
                    Type = $"https://httpstatuses.com/{statusCode}"
                };
            }

            switch (error)
            {
                case UnauthorizedError _:
                    return DefaultProblemDetails(StatusCodes.Status403Forbidden);
                case ValidationError validationError:
                    return new ValidationProblemDetails(validationError.ToDictionary(e => e.Key, e => e.Value.ToArray()))
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                case NotFoundError _:
                    return DefaultProblemDetails(StatusCodes.Status404NotFound);
                default:
                    return DefaultProblemDetails(StatusCodes.Status400BadRequest);
            }
        }
    }
}