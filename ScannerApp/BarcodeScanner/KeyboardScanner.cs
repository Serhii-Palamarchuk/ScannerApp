using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScannerApp.BarcodeScanner
{
    public class KeyboardScanner
    {
        public KeyboardScanner()
        {
            _barcode = "";
            _milliseconds = 0;
            _millisecondsList = new List<double>();

            _timer = new Timer();
            _timer.Tick += new EventHandler(TimerEventProcessor);
            _timer.Interval = Convert.ToInt32(_constMaxTypingInterval) + 10;
        }

        private const double _constMaxTypingInterval = 20;

        private DateTime? _lastPress;
        double _milliseconds = 0;
        private List<double> _millisecondsList;
        private string _barcode;
        private bool _isClearNeeded;
        private Timer _timer;
        
        public delegate void BarcodeTypedHandler(string barcode);

        public event BarcodeTypedHandler BarcodeTypedEvent;

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            _barcode = _barcode + e.KeyChar.ToString();
            _milliseconds = GetMilliseconds();
            _millisecondsList.Add(_milliseconds);

            if (_milliseconds > _constMaxTypingInterval)
                Clear();
            else if (_barcode.Length == 5)
                _timer.Start();

            _lastPress = DateTime.UtcNow;
        }

        private double GetMilliseconds()
        {
            return _lastPress != null ? (DateTime.UtcNow - _lastPress.Value).TotalMilliseconds : 0;
        }

        private void Clear()
        {
            _lastPress = null;
            _millisecondsList.RemoveAll(m => true);
            _barcode = "";
            if (_timer.Enabled)
                _timer.Stop();
        }

        private void TimerEventProcessor(object sender, EventArgs e)
        {
            if (GetMilliseconds() > _constMaxTypingInterval)
            {
                _timer.Stop();
                BarcodeTypedEvent?.Invoke(_barcode);
                Clear();
            }
        }

        public void Show()
        {
            string text = $"_barcode: '{_barcode}'{Environment.NewLine} ";
            if (_millisecondsList.Count > 0)
            {
                text +=
                $"min: {_millisecondsList?.Min()}, {Environment.NewLine}" +
                $"max: {_millisecondsList?.Max()}, {Environment.NewLine}" +
                $"avg: {_millisecondsList?.Average()} " +
                Environment.NewLine + $"{string.Join("," + Environment.NewLine, _millisecondsList)}";
            }
            MessageBox.Show(text);
            _lastPress = null;
            _millisecondsList = new List<double>();
            _barcode = "";
        }
    }
}
