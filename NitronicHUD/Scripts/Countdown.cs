using UnityEngine;
using UnityEngine.UI;

namespace NitronicHUD.UnityScripts
{
    public class Countdown : MonoBehaviour
    {
        public float Time = -10;

        public float StartKey = 0;
        public float DurationKey = 1;

        private float EndKey = 1;
        private Image img;

        void Start()
        {
            img = GetComponent<Image>();
        }

        void Update()
        {
            Material mat = Instantiate(img.material);
            mat.SetFloat("_Progress", GetInterpolationState(Time));
            img.material = mat;
        }

        float GetInterpolationState(float time)
        {
            EndKey = StartKey + (DurationKey / 2);
            float diff = EndKey - StartKey;
            float curve = (1 / diff) * (-Mathf.Abs(time - StartKey - diff)) + 1;
            return Mathf.Max(0, curve);
        }
    }
}