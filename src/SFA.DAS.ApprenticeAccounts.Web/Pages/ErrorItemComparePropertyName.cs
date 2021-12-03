using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.ApprenticeAccounts.Web.Exceptions;

namespace SFA.DAS.ApprenticeAccounts.Web.Pages
{
    internal class ErrorItemComparePropertyName : IEqualityComparer<ErrorItem>
    {
        public bool Equals([AllowNull] ErrorItem x, [AllowNull] ErrorItem y)
            => x?.PropertyName.Equals(y?.PropertyName) ?? false;

        public int GetHashCode([DisallowNull] ErrorItem obj) => obj.PropertyName.GetHashCode();
    }
}