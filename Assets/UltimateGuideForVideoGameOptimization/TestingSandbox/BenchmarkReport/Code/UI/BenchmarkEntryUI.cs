using System;
using TMPro;
using UnityEngine;

namespace BenchmarkReport
{
    public class BenchmarkEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _percentageText;
        [SerializeField] private TMP_Text _avgText;
        [SerializeField] private TMP_Text _maxText;
        [SerializeField] private TMP_Text _minText;
        [SerializeField] private TMP_Text[] _refTexts;
        private TMP_Text[] _rowTexts;
        private static readonly string PercentageFormat = "0.##%";
        private static readonly string MillisFormat = "0.###ms";
        private static readonly string ReferenceFormat = "x0.##";
        private static readonly Color[] ReferenceColors = new Color[]
        {
            Color.white, Color.red, Color.green, Color.blue, Color.yellow, Color.magenta
        };

        private void Awake()
        {
            _rowTexts = new TMP_Text[]
            {
                _titleText, _percentageText, _avgText, _maxText, _minText
            };
        }

        public void Display(BenchmarkCallEntry callEntry)
        {
            SetRowTextColor(ReferenceColors[callEntry.ReferenceId]);
            _titleText.text = callEntry.Title;
            _percentageText.text = callEntry.Percentage.ToString(PercentageFormat);
            _avgText.text = callEntry.Avg.ToString(MillisFormat);
            _maxText.text = callEntry.Max.ToString(MillisFormat);
            _minText.text = callEntry.Min.ToString(MillisFormat);
            for (int i = 0; i < _refTexts.Length; i++)
            {
                if (i < callEntry.ReferenceComparisons.Length)
                {
                    _refTexts[i].text = callEntry.ReferenceComparisons[i].ToString(ReferenceFormat);
                    _refTexts[i].color = ReferenceColors[i+1];
                }
                else
                {
                    _refTexts[i].text = "";
                }
            }
        }

        private void SetRowTextColor(Color color)
        {
            if (_rowTexts == null) return;
            foreach (TMP_Text text in _rowTexts)
            {
                text.color = color;
            }
        }

        public void Reset()
        {
            _titleText.text = "";
            _percentageText.text = "";
            _avgText.text = "";
            _maxText.text = "";
            _minText.text = "";
            for (int i = 0; i < _refTexts.Length; i++)
            {
                _refTexts[i].text = "";
            }
        }
    }
}