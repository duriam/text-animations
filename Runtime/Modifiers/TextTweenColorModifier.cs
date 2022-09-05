using TMPro;
using UnityEngine;

namespace Util.TextTween.Modifiers {
    [AddComponentMenu("UI/Text Tween Modifiers/Colors Modifier", 11)]
    public sealed class TextTweenColorModifier : TextTweenVertexModifier {
        [SerializeField] private Color[] colors;
        private Color32[] _newVertexColors;
        private Color32 _targetColor;
        public override bool ModifyGeometry => false;
        public override bool ModifyVertex => true;

        public override void ModifyCharacter(CharacterData characterData, TMP_Text textComponent, TMP_TextInfo textInfo,
            float progress, TMP_MeshInfo[] meshInfo) {
            
            if (colors == null || colors.Length == 0) return;
            int materialIndex = characterData.MaterialIndex;
            _newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = characterData.VertexIndex;

            _targetColor = colors[Mathf.CeilToInt(characterData.Progress * (colors.Length - 1))];
            _newVertexColors[vertexIndex + 0] = _targetColor;
            _newVertexColors[vertexIndex + 1] = _targetColor;
            _newVertexColors[vertexIndex + 2] = _targetColor;
            _newVertexColors[vertexIndex + 3] = _targetColor;
        }
    }
}