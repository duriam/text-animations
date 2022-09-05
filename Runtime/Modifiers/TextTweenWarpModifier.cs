using TMPro;
using UnityEngine;

namespace Util.TextTween.Modifiers {
    [AddComponentMenu("UI/Text Tween Modifiers/Warp Modifier", 11)]
    public sealed class TextTweenWarpModifier : TextTweenVertexModifier {
        [SerializeField] private float curveMultiplier = 10;
        [SerializeField] private AnimationCurve intensityCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        [SerializeField] private AnimationCurve warpCurve = new(
            new Keyframe(0, 0),
            new Keyframe(0.25f, 2.0f),
            new Keyframe(0.5f, 0), new Keyframe(0.75f, 2.0f),
            new Keyframe(1, 0f));

        private Matrix4x4 _matrix;
        private Vector3[] _vertices;

        public override bool ModifyGeometry => true;
        public override bool ModifyVertex => true;

        public override void ModifyCharacter(CharacterData characterData, TMP_Text textComponent, TMP_TextInfo textInfo,
            float progress, TMP_MeshInfo[] meshInfo) {
            float boundsMinX = textComponent.bounds.min.x;
            float boundsMaxX = textComponent.bounds.max.x;

            _vertices = textInfo.meshInfo[characterData.MaterialIndex].vertices;

            int characterDataVertexIndex = characterData.VertexIndex;
            Vector3 offsetToMidBaseline = new Vector2(
                (_vertices[characterDataVertexIndex + 0].x + _vertices[characterDataVertexIndex + 2].x) / 2,
                textInfo.characterInfo[characterData.Index].baseLine);

            _vertices[characterDataVertexIndex + 0] += -offsetToMidBaseline;
            _vertices[characterDataVertexIndex + 1] += -offsetToMidBaseline;
            _vertices[characterDataVertexIndex + 2] += -offsetToMidBaseline;
            _vertices[characterDataVertexIndex + 3] += -offsetToMidBaseline;

            float x0 = (offsetToMidBaseline.x - boundsMinX) / (boundsMaxX - boundsMinX);
            float x1 = x0 + 0.0001f;
            float y0 = warpCurve.Evaluate(x0) * curveMultiplier;
            float y1 = warpCurve.Evaluate(x1) * curveMultiplier;

            y0 *= intensityCurve.Evaluate(characterData.Progress);
            y1 *= intensityCurve.Evaluate(characterData.Progress);

            var horizontal = new Vector3(1, 0, 0);
            var tangent = new Vector3(x1 * (boundsMaxX - boundsMinX) + boundsMinX, y1) -
                          new Vector3(offsetToMidBaseline.x, y0);

            float dot = Mathf.Acos(Vector3.Dot(horizontal, tangent.normalized)) * 57.2957795f;
            Vector3 cross = Vector3.Cross(horizontal, tangent);
            float angle = cross.z > 0 ? dot : 360 - dot;

            _matrix = Matrix4x4.TRS(new Vector3(0, y0, 0), Quaternion.Euler(0, 0, angle), Vector3.one);
            _vertices[characterDataVertexIndex + 0] = _matrix.MultiplyPoint3x4(_vertices[characterDataVertexIndex + 0]);
            _vertices[characterDataVertexIndex + 1] = _matrix.MultiplyPoint3x4(_vertices[characterDataVertexIndex + 1]);
            _vertices[characterDataVertexIndex + 2] = _matrix.MultiplyPoint3x4(_vertices[characterDataVertexIndex + 2]);
            _vertices[characterDataVertexIndex + 3] = _matrix.MultiplyPoint3x4(_vertices[characterDataVertexIndex + 3]);

            _vertices[characterDataVertexIndex + 0] += offsetToMidBaseline;
            _vertices[characterDataVertexIndex + 1] += offsetToMidBaseline;
            _vertices[characterDataVertexIndex + 2] += offsetToMidBaseline;
            _vertices[characterDataVertexIndex + 3] += offsetToMidBaseline;
        }
    }
}