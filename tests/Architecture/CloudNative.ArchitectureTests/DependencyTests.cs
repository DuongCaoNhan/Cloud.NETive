using NetArchTest.Rules;
using Xunit;

namespace CloudNative.ArchitectureTests;

public class DependencyTests
{
    private const string DomainNs      = "CloudNative.*Service.Domain";
    private const string AppNs         = "CloudNative.*Service.Application";
    private const string InfrastructureNs = "CloudNative.*Service.Infrastructure";

    [Fact]
    public void Domain_Should_Not_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(DependencyTests).Assembly)
            .That().ResideInNamespaceMatching(DomainNs)
            .ShouldNot().HaveDependencyOn(InfrastructureNs)
            .GetResult();
        Assert.True(result.IsSuccessful,
            $"Domain layer must not reference Infrastructure: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Application_Should_Not_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(typeof(DependencyTests).Assembly)
            .That().ResideInNamespaceMatching(AppNs)
            .ShouldNot().HaveDependencyOn(InfrastructureNs)
            .GetResult();
        Assert.True(result.IsSuccessful,
            $"Application layer must not reference Infrastructure: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
