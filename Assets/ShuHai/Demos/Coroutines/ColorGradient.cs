using UnityEngine;

namespace ShuHai.Unity.CoroutineAnywhere.Demos
{
    public class ColorGradient
    {
        public Color Color;

        public float Step = 0.01f;

        public Color NextRed() { return NextChannelValue(0); }
        public Color NextGreen() { return NextChannelValue(1); }
        public Color NextBlue() { return NextChannelValue(2); }
        public Color NextAlpha() { return NextChannelValue(3); }

        public Color NextChannelValue(int channelIndex)
        {
            Color[channelIndex] = NextValue(Color[channelIndex]);
            return Color;
        }

        private float NextValue(float value)
        {
            var stepValue = Mathf.Abs(Step);
            if (value < stepValue || value > 1 - stepValue)
                Step = -Step;
            return Mathf.Abs((value + Step) % 1f);
        }
    }
}