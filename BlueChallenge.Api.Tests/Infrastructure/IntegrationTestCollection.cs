using Xunit;

namespace BlueChallenge.Api.Tests.Infrastructure;

[CollectionDefinition("Integration Tests", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<TestWebApplicationFactory>
{
}
