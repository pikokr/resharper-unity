using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.RdBackend.Common.Env;
using JetBrains.ReSharper.Plugins.Unity.HlslSupport;

namespace JetBrains.ReSharper.Plugins.Unity.Rider.Shaders
{
    [ZoneMarker]
    public class ZoneMarker : IRequire<ILanguageHlslSupportZone>, IRequire<IResharperHostCoreFeatureZone>
    {
    }
}