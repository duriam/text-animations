using TMPro;
using UnityEngine;

namespace Util.TextTween.Modifiers {
    [AddComponentMenu("UI/Text Tween Modifiers/Per Character Color", 11)]
    public sealed class TextTweenPerCharacterColorModifier : TextTweenVertexModifier {
        [SerializeField] private AnimationCurve curve = new(new Keyframe(0, 0));
        [SerializeField] private Color[] colors;
        private Color32[] _newVertexColors;
        public override bool ModifyGeometry => false;
        public override bool ModifyVertex => true;

        public override void ModifyCharacter(CharacterData characterData, TMP_Text textComponent, TMP_TextInfo textInfo,
            float progress, TMP_MeshInfo[] meshInfo) {
            
            if (colors == null || colors.Length == 0) return;
            int materialIndex = characterData.MaterialIndex;
            _newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = characterData.VertexIndex;
            Color targetColor = colors[characterData.Index % colors.Length];
            Color currentColor = _newVertexColors[0];
            targetColor *= curve.Evaluate(characterData.Progress);
            currentColor *= (1f - curve.Evaluate(characterData.Progress));
            
            _newVertexColors[vertexIndex + 0] = currentColor + targetColor;
            _newVertexColors[vertexIndex + 1] = currentColor + targetColor;
            _newVertexColors[vertexIndex + 2] = currentColor + targetColor;
            _newVertexColors[vertexIndex + 3] = currentColor + targetColor;
        }
    }
}