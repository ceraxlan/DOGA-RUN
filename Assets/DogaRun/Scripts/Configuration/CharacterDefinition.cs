using UnityEngine;

namespace DogaRun.Configuration
{
    [CreateAssetMenu(menuName = "DogaRun/Character", fileName = "CharacterDefinition")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private string characterId = "Doga";
        [SerializeField] private GameObject visualPrefab;
        [SerializeField, Min(0.5f)] private float height = 1.65f;
        [SerializeField] private Color skinTone = new Color(1f, 0.76f, 0.58f);
        [SerializeField] private Color hairColor = new Color(1f, 0.76f, 0.2f);
        [SerializeField] private Color eyeColor = new Color(0.12f, 0.46f, 0.9f);
        [SerializeField] private Color outfitPrimary = new Color(0.08f, 0.68f, 0.72f);
        [SerializeField] private Color outfitAccent = new Color(0.95f, 0.35f, 0.27f);
        [SerializeField] private Color shoeColor = new Color(1f, 0.72f, 0.16f);

        public string CharacterId => characterId;
        public GameObject VisualPrefab => visualPrefab;
        public float Height => height;
        public Color SkinTone => skinTone;
        public Color HairColor => hairColor;
        public Color EyeColor => eyeColor;
        public Color OutfitPrimary => outfitPrimary;
        public Color OutfitAccent => outfitAccent;
        public Color ShoeColor => shoeColor;
    }
}
