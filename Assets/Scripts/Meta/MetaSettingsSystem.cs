using ShroomSniff.App;
using VContainer;

namespace ShroomSniff.Meta
{
    public interface IMetaSettingsSystem
    {
        void UpdateLevelSettings(System.Action<LevelSettingsData> update);
        void UpdateMushroomSettings(System.Action<MushroomSettingsData> update);
        void ResetToDefaults();
    }

    public class MetaSettingsSystem : IMetaSettingsSystem
    {
        private readonly IAppSaveSystem _saveSystem;

        [Inject]
        public MetaSettingsSystem(IAppSaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
        }

        public void UpdateLevelSettings(System.Action<LevelSettingsData> update)
        {
            _saveSystem.UpdateLevelSettings(update);
        }

        public void UpdateMushroomSettings(System.Action<MushroomSettingsData> update)
        {
            _saveSystem.UpdateMushroomSettings(update);
        }

        public void ResetToDefaults()
        {
            _saveSystem.ResetToDefaults();
        }
    }
}
