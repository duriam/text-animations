using TMPro;
using UnityEngine;

namespace Util.TextTween.Modifiers {
    [AddComponentMenu("UI/Text Tween Modifiers/Gradient Modifier", 11)]
    public sealed class TextTweenGradientModifier : TextTweenVertexModifier {
        [SerializeField] private Gradient gradient;
        private Color32[] _newVertexColors;
        private Color _targetColor;
        public override bool ModifyGeometry => false;
        public override bool ModifyVertex => true;

        public override void ModifyCharacter(CharacterData characterData, TMP_Text textComponent, TMP_TextInfo textInfo,
            float progress, TMP_MeshInfo[] meshInfo) {
            
            if (gradient == null) return;
            int materialIndex = characterData.MaterialIndex;
            _newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = characterData.VertexIndex;

            _targetColor = gradient.Evaluate(characterData.Progress);
            _newVertexColors[vertexIndex + 0] = _targetColor;
            _newVertexColors[vertexIndex + 1] = _targetColor;
            _newVertexColors[vertexIndex + 2] = _targetColor;
            _newVertexColors[vertexIndex + 3] = _targetColor;
        }
    }
}