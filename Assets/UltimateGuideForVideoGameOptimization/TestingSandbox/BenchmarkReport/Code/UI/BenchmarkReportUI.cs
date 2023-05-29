using System;
using System.Collections.Generic;
using UnityEngine;

namespace BenchmarkReport
{
    public class BenchmarkReportUI : MonoBehaviour
    {
        [SerializeField] private BenchmarkEntryUI _entryUIPrefab;
        [SerializeField] private BenchmarkEntryUI _gcEntryUI;
        [SerializeField] private BenchmarkTotalUI _totalUI;
        [SerializeField] private Transform _entryUIsContainer;
        private const int EntryUIsAmount = 30;
        private BenchmarkEntryUI[] _entryUIs = new BenchmarkEntryUI[EntryUIsAmount];

        private void Awake()
        {
            for (int i = 0; i < EntryUIsAmount; i++)
            {
                _entryUIs[i] = Instantiate(_entryUIPrefab, _entryUIsContainer);
                _entryUIs[i].Reset();
                RectTransform rectTrans = (_entryUIs[i].transform as RectTransform);
                rectTrans.Translate(0, -20f * i, 0);
            }
        }

        public void DisplayCalls(IEnumerable<BenchmarkCallEntry> entries)
        {
            int i = 0;
            foreach (BenchmarkCallEntry entry in entries)
            {
                _entryUIs[i++].Display(entry);
            }
        }

        public void DisplayTotal(BenchmarkTotalEntry entry)
        {
            _totalUI.Display(entry);
        }

        public void DisplayGC(BenchmarkCallEntry entry)
        {
            _gcEntryUI.Display(entry);
        }
    }
}