using ShroomSniff.Gameplay;
using ShroomSniff.Meta;
using ShroomSniff.UI;
using UnityEngine;

namespace ShroomSniff.App
{
    /// <summary>
    /// Config that holds prefab references to Game and Base scopes
    /// </summary>
    [CreateAssetMenu(fileName = "AppConfiguration", menuName = "ShroomSniff/App Config")]
    public class AppConfiguration : ScriptableObject
    {
        [Header("Scene Names")]
        [SerializeField] private string gameplaySceneName = "GameplayScene";
        [SerializeField] private string metaSceneName = "MetaScene";
        
        [Header("Scope Prefabs")]
        [SerializeField] private GameplayScope gameplayScopePrefab;
        [SerializeField] private MetaScope metaScopePrefab;
        
        [Header("Req")]
        [SerializeField] private GameUIController gameUIController;

        [Header("Audio")]
        [SerializeField] private AudioClip suctionSoundEffect;
        
        public string GameplaySceneName => gameplaySceneName;
        public string MetaSceneName => metaSceneName;
        public GameplayScope GameplayScopePrefab => gameplayScopePrefab;
        public MetaScope MetaScopePrefab => metaScopePrefab;
        public GameUIController GameUIController => gameUIController;
        public AudioClip SuctionSoundEffect => suctionSoundEffect;
    }
}