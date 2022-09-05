using TMPro;
using UnityEngine;

namespace Util.TextTween.Modifiers {
    [RequireComponent(typeof(TMP_TextTween))]
    public abstract class TextTweenVertexModifier : MonoBehaviour {
        private TMP_TextTween _cachedTextTween;
        public abstract bool ModifyGeometry { get; }
        public abstract bool ModifyVertex { get; }

        protected TMP_TextTween TextTween {
            get {
                if (_cachedTextTween == null)
                    _cachedTextTween = GetComponent<TMP_TextTween>();
                return _cachedTextTween;
            }
        }

        private void OnValidate() {
            TextTween.SetDirty();
        }

        public abstract void ModifyCharacter(CharacterData characterData, TMP_Text textComponent, TMP_TextInfo textInfo,
            float progress, TMP_MeshInfo[] meshInfo);
    }
}