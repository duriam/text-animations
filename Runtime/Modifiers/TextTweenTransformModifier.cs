using TMPro;
using UnityEngine;

namespace Util.TextTween.Modifiers {
    [AddComponentMenu("UI/Text Tween Modifiers/Transform Modifier", 11)]
    public sealed class TextTweenTransformModifier : TextTweenVertexModifier {
        [SerializeField] private bool animatePosition;
        [SerializeField] private bool animateRotation;
        [Header("Animation Parameters")] [SerializeField] private bool animateScale;
        [SerializeField] private bool applyOnX;
        [SerializeField] private bool applyOnY;
        [SerializeField] private bool applyOnZ;
        [SerializeField] private AnimationCurve positionCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        [SerializeField] private Vector3 positionMultiplier;
        [SerializeField] private AnimationCurve rotationCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        [SerializeField] private Vector3 rotationMultiplier;
        [SerializeField] private AnimationCurve scaleCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        public override bool ModifyGeometry => true;
        public override bool ModifyVertex => false;

        public override void ModifyCharacter(CharacterData characterData, TMP_Text textComponent, TMP_TextInfo textInfo,
            float progress, TMP_MeshInfo[] meshInfo) {
            
            int materialIndex = characterData.MaterialIndex;
            int vertexIndex = characterData.VertexIndex;
            Vector3[] sourceVertices = meshInfo[materialIndex].vertices;
            Vector2 charMidBaseline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
            Vector3 offset = charMidBaseline;
            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

            var finalScale = Vector3.one;
            if (animateScale) {
                if (applyOnX) finalScale.x = scaleCurve.Evaluate(characterData.Progress);
                if (applyOnY) finalScale.y = scaleCurve.Evaluate(characterData.Progress);
                if (applyOnZ) finalScale.z = scaleCurve.Evaluate(characterData.Progress);
            }

            var finalPosition = Vector3.zero;
            if (animatePosition) {
                finalPosition = positionMultiplier * positionCurve.Evaluate(characterData.Progress);
            }

            Quaternion finalQuaternion = Quaternion.identity;
            if (animateRotation) {
                finalQuaternion = Quaternion.Euler(rotationMultiplier * rotationCurve.Evaluate(characterData.Progress));
            }

            Matrix4x4 matrix = Matrix4x4.TRS(finalPosition, finalQuaternion, finalScale);
            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;
        }
    }
}