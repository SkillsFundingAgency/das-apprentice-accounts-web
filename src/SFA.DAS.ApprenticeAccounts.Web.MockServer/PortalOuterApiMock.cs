using SFA.DAS.ApprenticePortal.OuterApi.Mock.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SFA.DAS.ApprenticeAccounts.Web.MockServer
{
    public class PortalOuterApiMock : IDisposable
    {
        private IRespondWithAProvider _createAccountsRequest;

        public PortalOuterApiMock() : this(0, false)
        {
        }

        public PortalOuterApiMock(int? port = 0, bool ssl = false)
        {
            MockServer = WireMockServer.Start(port, ssl);
            Reset();
        }

        public WireMockServer MockServer { get; }

        public string BaseAddress => $"{MockServer.Urls[0]}";

        public HttpClient HttpClient =>
            new HttpClient { BaseAddress = new Uri(BaseAddress) };

        public void Reset()
        {
            MockServer.Reset();

            MockServer.Given(Request.Create().WithPath("/hello")).RespondWith(Response.Create().WithSuccess());

            CanCreateAccounts();
            CanUpdateAccounts();
        }

        public void CanCreateAccounts()
        {
            _createAccountsRequest =
            MockServer.Given(
                Request.Create()
                    .UsingPost()
                    .WithPath("/apprentices"));

            _createAccountsRequest.RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        public void CanUpdateAccounts()
        {
            MockServer.Given(
                Request.Create()
                    .UsingPatch()
                    .WithPath("/apprentices/*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        public void WithoutApprentice(Guid apprenticeId)
        {
            MockServer
                .Given(Request.Create()
                    .WithPath($"/apprentices/{apprenticeId}").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.NotFound));
        }

        public PortalOuterApiMock WithApprentice(Apprentice apprentice)
        {
            MockServer
                .Given(Request.Create()
                    .WithPath($"/apprentices/{apprentice.ApprenticeUrlId()}").UsingGet())
                .RespondWith(Response.Create()
                    .WithBodyAsJson(apprentice));

            return this;
        }

        public PortalOuterApiMock WithApprentices(params Apprentice[] apprentices)
            => apprentices.Aggregate(this, (mock, apprentice) => mock.WithApprentice(apprentice));

        public void Dispose()
        {
        }

        public void RejectNewAccounts(object response)
        {
            _createAccountsRequest.RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithBodyAsJson(response));
        }
    }
}