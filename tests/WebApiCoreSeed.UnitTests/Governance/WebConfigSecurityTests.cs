using System.Xml.Linq;
using Xunit;

namespace WebApiCoreSeed.UnitTests.Governance;

public sealed class WebConfigSecurityTests
{
    [Fact(DisplayName = "IIS web.config bloqueia framing para todas as respostas")]
    public void IisConfigurationDeveConfigurarXFrameOptionsDeny()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "WebApiCoreSeed.slnx")))
        {
            directory = directory.Parent;
        }

        var root = directory?.FullName
            ?? throw new InvalidOperationException("Repository root not found.");
        var path = Path.Combine(root, "src", "WebApiCoreSeed.Api", "web.config");
        var configuration = XDocument.Load(path);

        var headers = configuration.Root?
            .Element("system.webServer")?
            .Element("httpProtocol")?
            .Element("customHeaders")?
            .Elements("add") ?? Enumerable.Empty<XElement>();

        Assert.Contains(headers, header =>
            string.Equals((string?)header.Attribute("name"), "X-Frame-Options", StringComparison.OrdinalIgnoreCase)
            && string.Equals((string?)header.Attribute("value"), "DENY", StringComparison.Ordinal));
    }
}
