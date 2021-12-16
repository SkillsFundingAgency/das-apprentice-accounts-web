using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.ApprenticeAccounts.Web.Services.InnerApi;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SFA.DAS.ApprenticeAccounts.Web.MockServer
{
    public class ApprenticeAccountsApiBuilder
    {
        private static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        private readonly WireMockServer _server;

        public ApprenticeAccountsApiBuilder(int port)
        {
            _server = WireMockServer.StartWithAdminInterface(port, true);

            _server.Given(
                    Request.Create()
                        .WithPath("/apprentices")
                        .UsingPost())
                .RespondWith(
                    Response.Create().WithStatusCode(HttpStatusCode.OK));

            _server.Given(
                    Request.Create()
                        .WithPath("/apprentices/*")
                        .UsingPatch())
                .RespondWith(
                    Response.Create().WithStatusCode(HttpStatusCode.OK));
        }

        public static ApprenticeAccountsApiBuilder Create(int port)
        {
            return new ApprenticeAccountsApiBuilder(port);
        }

        public ApprenticeAccountsApi BuildAndRun()
        {
            return new ApprenticeAccountsApi(_server);
        }

        public ApprenticeAccountsApiBuilder WithUserAccountAndTermsOfUseAgreed()
        {
            _server.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath("/apprentices/*"))
                .RespondWith(Response.Create()
                    .WithBodyAsJson(new Apprentice
                    {
                        FirstName = "Bob",
                        LastName = "Dylan",
                        DateOfBirth = new DateTime(2000, 01, 13),
                        ApprenticeId = Guid.NewGuid(),
                        Email = "bob@dylan.com",
                        TermsOfUseAccepted = true,
                    }));
            return this;
        }

        public ApprenticeAccountsApiBuilder WithUserAccountButNoTermsOfUseAgreed()
        {
            _server.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath("/apprentices/*"))
                .RespondWith(Response.Create()
                    .WithBodyAsJson(new Apprentice
                    {
                        FirstName = "Bob",
                        LastName = "Marley",
                        DateOfBirth = new DateTime(2000, 01, 13),
                        ApprenticeId = Guid.NewGuid(),
                        Email = "bob@marley.com",
                        TermsOfUseAccepted = false,
                    }));
            return this;
        }
    }
}