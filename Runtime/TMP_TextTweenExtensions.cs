using System.Collections;

namespace Util.TextTween {
    public static partial class TMP_TextTweenExtensions {
        public static IEnumerator WaitForCompletionEnumerator(this TMP_TextTween textTween) {
            while (textTween != null && textTween.IsPlaying && textTween.Progress < 1.0f)
                yield return null;
        }
    }
}