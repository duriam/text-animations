using System;
using System.Collections;
using UnityEngine;

namespace Util.TextTween {
    public sealed class TextTweenSequence : MonoBehaviour {
        [Serializable]
        private struct TextTweenSequenceItemData {
            [SerializeField] private TMP_TextTween textTween;
            public TMP_TextTween TextTween => textTween;
            [SerializeField] private float afterInterval;
            public float AfterInterval => afterInterval;
        }

        [SerializeField] private TextTweenSequenceItemData[] sequence;
        [SerializeField] private bool playOnStart;
        private Coroutine _playCoroutine;

        private void Start() {
            if (playOnStart) Play();
        }

        public void Play() {
            if (_playCoroutine != null) StopCoroutine(_playCoroutine);
            _playCoroutine = StartCoroutine(PlayEnumerator());
        }

        private IEnumerator PlayEnumerator() {
            int playedItems = 0;
            while (playedItems < sequence.Length) {
                TextTweenSequenceItemData textTweenSequenceItemData = sequence[playedItems];
                textTweenSequenceItemData.TextTween.Play();
                yield return textTweenSequenceItemData.TextTween.WaitForCompletionEnumerator();
                yield return new WaitForSeconds(textTweenSequenceItemData.AfterInterval);
                playedItems++;
            }
        }
    }
}