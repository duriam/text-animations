using UnityEngine;

namespace Util.TextTween {
    public sealed class TextTween_WaitForCompletion : CustomYieldInstruction {
        private readonly TMP_TextTween _textTween;
        public override bool keepWaiting => _textTween.IsPlaying && _textTween.Progress < 1;

        public TextTween_WaitForCompletion(TMP_TextTween targetTextTween) {
            _textTween = targetTextTween;
        }
    }
}