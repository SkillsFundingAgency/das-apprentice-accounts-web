﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.ApprenticeAccounts.Web.Exceptions
{
    [Serializable]
    public class DomainValidationException : Exception
    {
        public List<ErrorItem> Errors { get; }

        public DomainValidationException(ValidationProblemDetails errors)
        {
            Errors = errors.Errors.SelectMany(x =>
                x.Value.Select(y => new ErrorItem { PropertyName = x.Key, ErrorMessage = y }))
                .ToList();

            if (Errors.Count == 0)
            {
                Errors = new List<ErrorItem> { new ErrorItem { ErrorMessage = errors.Detail } };
            }
        }

        protected DomainValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));
            Errors = info.GetValue(nameof(Errors), typeof(List<ErrorItem>)) as List<ErrorItem>
                ?? throw new InvalidOperationException();
        }
    }

    public class ErrorItem
    {
        public string PropertyName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }
}