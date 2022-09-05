using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Util.TextTween.Modifiers;

namespace Util.TextTween {
    [ExecuteInEditMode, AddComponentMenu("UI/Text Tween")]
    public sealed class TMP_TextTween : MonoBehaviour {
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private float duration = 0.1f;
        [SerializeField] private float delay = 0.05f;
        [SerializeField, Range(0.0f, 1.0f)] private float progress;
        [SerializeField] private bool playWhenReady = true;
        [SerializeField] private bool loop;
        [SerializeField] private bool playForever;
        [SerializeField] private bool animationControlled;

        private TMP_MeshInfo[] _cachedMeshInfo;
        private string _cachedText = string.Empty;
        private CharacterData[] _charactersData;
        private bool _dispatchedAfterReadyMethod;
        private bool _forceUpdate;
        private float _internalTime;
        private bool _isDirty = true;
        private bool _isPlaying;
        private float _realTotalAnimationTime;
        private RectTransform _rectTransform;
        private TMP_TextInfo _textInfo;
        private bool _updateGeometry;
        private bool _updateVertexData;
        private TextTweenVertexModifier[] _vertexModifiers;

        private TMP_Text TmpText {
            get {
                if (tmpText != null) return tmpText;
                tmpText = GetComponent<TMP_Text>();
                if (tmpText == null) tmpText = GetComponentInChildren<TMP_Text>();
                return tmpText;
            }
        }

        public RectTransform RectTransform {
            get {
                if (_rectTransform == null)
                    _rectTransform = (RectTransform) transform;
                return _rectTransform;
            }
        }

        public string Text {
            get => TmpText.text;
            set {
                TmpText.text = value;
                SetDirty();
                UpdateIfDirty();
            }
        }

        public float Progress => progress;
        public bool IsPlaying => _isPlaying;

        private void Awake() {
            if (!animationControlled && Application.isPlaying)
                SetProgress(0);
        }

        public void Update() {
            if (!IsAllComponentsReady()) return;
            UpdateIfDirty();

            if (!_dispatchedAfterReadyMethod) {
                AfterIsReady();
                _dispatchedAfterReadyMethod = true;
            }

            CheckProgress();
            UpdateTime();
            if (IsPlaying || animationControlled || _forceUpdate)
                ApplyModifiers();
        }

        private void OnDisable() {
            _forceUpdate = true;
        }

        private void OnValidate() {
            _cachedText = string.Empty;
            SetDirty();

            if (tmpText == null) {
                tmpText = GetComponent<TMP_Text>();
                if (tmpText == null)
                    tmpText = GetComponentInChildren<TMP_Text>();
            }
        }


        public void Restart() {
            _internalTime = 0;
        }

        public void Play(bool fromBeginning = true) {
            if (!IsAllComponentsReady()) {
                playWhenReady = true;
                return;
            }

            if (fromBeginning) {
                Restart();
            }

            _isPlaying = true;
        }

        public void Complete() {
            if (IsPlaying) {
                progress = 1.0f;
            }
        }

        public void Stop() {
            _isPlaying = false;
        }

        public void SetProgress(float targetProgress) {
            progress = targetProgress;
            _internalTime = progress * _realTotalAnimationTime;
            UpdateTime();
            ApplyModifiers();
            tmpText.havePropertiesChanged = true;
        }

        public void SetPlayForever(bool shouldPlayForever) {
            playForever = shouldPlayForever;
        }

        public CustomYieldInstruction WaitForCompletion() {
            return new TextTween_WaitForCompletion(this);
        }


        private void AfterIsReady() {
            if (!Application.isPlaying) return;
            if (playWhenReady) Play();
            else SetProgress(progress);
        }

        private bool IsAllComponentsReady() {
            if (TmpText == null) return false;
            if (TmpText.textInfo == null) return false;
            if (TmpText.mesh == null) return false;
            return TmpText.textInfo.meshInfo != null;
        }

        private void ApplyModifiers() {
            if (_charactersData == null) return;
            tmpText.ForceMeshUpdate(true);
            for (int i = 0; i < _charactersData.Length; i++) {
                ModifyCharacter(i, _cachedMeshInfo);
            }

            if (_updateGeometry) {
                for (int i = 0; i < _textInfo.meshInfo.Length; i++) {
                    _textInfo.meshInfo[i].mesh.vertices = _textInfo.meshInfo[i].vertices;
                    TmpText.UpdateGeometry(_textInfo.meshInfo[i].mesh, i);
                }
            }

            if (_updateVertexData) TmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        private void ModifyCharacter(int info, TMP_MeshInfo[] meshInfo) {
            for (int i = 0; i < _vertexModifiers.Length; i++) {
                _vertexModifiers[i].ModifyCharacter(_charactersData[info], TmpText, _textInfo, progress, meshInfo);
            }
        }

        private void CheckProgress() {
            if (IsPlaying) {
                _internalTime += Time.deltaTime;
                if (_internalTime < _realTotalAnimationTime || playForever) return;

                if (loop)
                    _internalTime = 0;
                else {
                    _internalTime = _realTotalAnimationTime;
                    progress = 1.0f;
                    Stop();
                    OnAnimationCompleted();
                }
            }
        }

        private void OnAnimationCompleted() {
        }

        private void UpdateTime() {
            if (!IsPlaying || animationControlled) {
                _internalTime = progress * _realTotalAnimationTime;
            }
            else {
                progress = _internalTime / _realTotalAnimationTime;
            }

            if (_charactersData == null) return;
            for (int i = 0; i < _charactersData.Length; i++) {
                _charactersData[i].UpdateTime(_internalTime);
            }
        }

        private void UpdateIfDirty() {
            if (!_isDirty) return;
            if (!gameObject.activeInHierarchy || !gameObject.activeSelf) return;

            TextTweenVertexModifier[] currentComponents = GetComponents<TextTweenVertexModifier>();
            if (_vertexModifiers == null || _vertexModifiers != currentComponents) {
                _vertexModifiers = currentComponents;
                for (int i = 0; i < _vertexModifiers.Length; i++) {
                    TextTweenVertexModifier vertexModifier = _vertexModifiers[i];
                    if (!_updateGeometry && vertexModifier.ModifyGeometry) {
                        _updateGeometry = true;
                    }

                    if (!_updateVertexData && vertexModifier.ModifyVertex) {
                        _updateVertexData = true;
                    }
                }
            }

            if (string.IsNullOrEmpty(_cachedText) || !_cachedText.Equals(TmpText.text)) {
                TmpText.ForceMeshUpdate();
                _textInfo = TmpText.textInfo;
                _cachedMeshInfo = _textInfo.CopyMeshInfoVertexData();

                var newCharacterDataList = new List<CharacterData>();
                int indexCount = 0;
                for (int i = 0; i < _textInfo.characterCount; i++) {
                    if (!_textInfo.characterInfo[i].isVisible) {
                        continue;
                    }

                    CharacterData characterData = new CharacterData(indexCount,
                        delay * indexCount,
                        duration,
                        playForever,
                        _textInfo.characterInfo[i].materialReferenceIndex,
                        _textInfo.characterInfo[i].vertexIndex);
                    newCharacterDataList.Add(characterData);
                    indexCount += 1;
                }

                _charactersData = newCharacterDataList.ToArray();
                _realTotalAnimationTime = duration + _charactersData.Length * delay;
                _cachedText = TmpText.text;
            }

            _isDirty = false;
        }

        public void SetDirty() {
            _isDirty = true;
        }
    }
}