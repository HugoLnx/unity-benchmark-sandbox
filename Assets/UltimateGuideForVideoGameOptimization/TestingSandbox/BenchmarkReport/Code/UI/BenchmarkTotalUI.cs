using TMPro;
using UnityEngine;

namespace BenchmarkReport
{
    public class BenchmarkTotalUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _avgText;
        [SerializeField] private TMP_Text _maxText;
        [SerializeField] private TMP_Text _minText;
        [SerializeField] private TMP_Text _percent80FPS;
        [SerializeField] private TMP_Text _percent60FPS;
        private static readonly string PercentageFormat = "0.##%";
        private static readonly string MillisFormat = "0.###ms";
        public void Display(BenchmarkTotalEntry entry)
        {
            _avgText.text = entry.Avg.ToString(MillisFormat);
            _maxText.text = entry.Max.ToString(MillisFormat);
            _minText.text = entry.Min.ToString(MillisFormat);
            _percent80FPS.text = entry.Percent80FPS.ToString(PercentageFormat);
            _percent60FPS.text = entry.Percent60FPS.ToString(PercentageFormat);
        }
    }
}