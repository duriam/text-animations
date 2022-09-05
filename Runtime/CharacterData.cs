using System;
using UnityEngine;

namespace Util.TextTween {
    [Serializable]
    public struct CharacterData {
        private int _index;
        private int _materialIndex;
        private bool _playForever;
        private float _progress;
        private float _startingTime;
        private float _totalAnimationTime;
        private int _vertexIndex;

        public CharacterData(int targetIndex, float startTime, float targetAnimationTime, bool isPlayForever,
            int targetMaterialIndex, int targetVertexIndex) {
            
            _index = targetIndex;
            _progress = 0.0f;
            _startingTime = startTime;
            _totalAnimationTime = targetAnimationTime;
            _playForever = isPlayForever;
            _vertexIndex = targetVertexIndex;
            _materialIndex = targetMaterialIndex;
        }

        public float Progress => _progress;
        public int MaterialIndex => _materialIndex;
        public int VertexIndex => _vertexIndex;
        public int Index => _index;

        public void UpdateTime(float time) {
            if (time < _startingTime) {
                _progress = 0;
                return;
            }

            float currentProgress = (time - _startingTime) / _totalAnimationTime;
            if (!_playForever) {
                currentProgress = Mathf.Clamp01(currentProgress);
            }
            _progress = currentProgress;
        }
    }
}