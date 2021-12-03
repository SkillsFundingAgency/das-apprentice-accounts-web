﻿using System;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeAccounts.Authentication;

namespace SFA.DAS.ApprenticeAccounts.Web.Services
{
    public class ApprenticeAccountProvider : IApprenticeAccountProvider
    {
        private readonly ApprenticeApi _client;

        public ApprenticeAccountProvider(ApprenticeApi client)
        {
            _client = client;
        }
        public async Task<IApprenticeAccount?> GetApprenticeAccount(Guid id)
        {
            return await _client.TryGetApprentice(id);
        }
    }
}