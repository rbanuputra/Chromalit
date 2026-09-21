using UnityEngine;

namespace Chromalit.Color
{
    [CreateAssetMenu(fileName = "NewColorData", menuName = "Chromalit/Color Data")]
    public class ColorData : ScriptableObject
    {
        [Header("Identitas")]
        public string colorName;
        public ColorType colorType;
        public UnityEngine.Color displayColor = UnityEngine.Color.white;

        [Header("Movement Modifier")]
        [Tooltip("1.0 = normal speed")]
        public float speedMultiplier = 1f;

        [Tooltip("1.0 = normal jump")]
        public float jumpMultiplier = 1f;

        [Header("Immunities")]
        public bool immuneToFire;
        public bool immuneToPoison;
        public bool immuneToElectric;

        [Header("Abilities")]
        public bool canSwim;
        public bool canMeltIce;
        public bool canActivateElectricPanel;

        [Header("Weakness")]
        [Tooltip("Jika true, kena air langsung jadi Putih")]
        public bool neutralizedByWater;
    }
}