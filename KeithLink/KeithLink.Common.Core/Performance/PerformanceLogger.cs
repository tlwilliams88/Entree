

// Core
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Core.Performance {
    public class PerformanceLogger {
        #region attributes
        private List<string> _headers;
        private List<string> _output;
        private StreamWriter  _file;
        private string _fileName;
        private Stopwatch _stopwatch;
        #endregion

        #region constructor
        public PerformanceLogger(string fileName) {
            _headers = new List<string>();
            _output = new List<string>();
            _fileName = fileName;
            _stopwatch = new Stopwatch();
        }
        #endregion

        #region methods
        public void StartTimer(string key) {
            if (_stopwatch.IsRunning == false) {
                _stopwatch.Reset();
                _stopwatch.Start();
                AddHeader( key );
            }
        }

        public void StopTimer() {
            _stopwatch.Stop();
            AddOutput();
        }

        private void AddHeader( string key ) {
            _headers.Add( key );
        }

        private void AddOutput() {
            _output.Add( _stopwatch.Elapsed.TotalSeconds.ToString() ); 
        }

        public void WriteOutput() {
            _file = new StreamWriter( string.Format( @"C:\logs\{0}.txt", _fileName), true );

            _file.WriteLine( string.Join( ",", _headers ) );
            _file.WriteLine( string.Join( ",", _output ) );

            _headers.Clear();
            _output.Clear();

            _file.Close();
            _file.Dispose();
        }
        #endregion

        #region properties
        #endregion
    }
}
