using VContainer;
using VContainer.Unity;

namespace ShroomSniff.Meta
{
    public class MetaScope : LifetimeScope
    {
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MetaSettingsSystem>(Lifetime.Singleton)
                .As<IMetaSettingsSystem>();
        }
    }
}