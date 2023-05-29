using System.Collections;
using UnityEngine;

namespace BenchmarkReport
{
    public class BenchmarkReportUIRefresher : MonoBehaviour
    {
        [SerializeField] private float _refreshRate = 0.2f;
        private BenchmarkRunner _runner;
        private BenchmarkReportUI _reportUi;

        private void Awake()
        {
            _reportUi = GetComponent<BenchmarkReportUI>();
        }

        private void Start()
        {
            StartCoroutine(RefreshLoop());
        }

        public void SetRunner(BenchmarkRunner runner)
        {
            _runner = runner;
        }

        private IEnumerator RefreshLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(_refreshRate);
                Refresh();
            }
        }

        private void Refresh()
        {
            if (_runner == null) return;
            _reportUi.DisplayCalls(_runner.ReportCalls());
            _reportUi.DisplayTotal(_runner.ReportTotal());
            _reportUi.DisplayGC(_runner.ReportGC());
        }
    }
}