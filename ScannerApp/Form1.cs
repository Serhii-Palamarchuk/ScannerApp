using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.PointOfService;
using ScannerApp.BarcodeScanner;

namespace ScannerApp
{
    public partial class Form1 : Form
    {
        private PosExplorer _explorer;
        private Scanner _scanner;

        public Form1()
        {
            InitializeComponent();
            _keyboardScanner = new KeyboardScanner();
            _keyboardScanner.BarcodeTypedEvent += Form1_BarcodeTypedProcessor;

            _explorer = new PosExplorer();
            //FindScanner();
            //SetScanner();


        }

        private void Form1_BarcodeTypedProcessor(string barcode)
        {
            MessageBox.Show($"Form1_BarcodeTypedProcessor, barcode: {barcode}");
        }

        private KeyboardScanner _keyboardScanner;

        private string FindScanner()
        {

            string name = string.Empty;
            DeviceCollection scanners = _explorer.GetDevices(DeviceType.Scanner);
            foreach (DeviceInfo s in scanners)
            {
                Console.WriteLine(s.Description);
                if (string.IsNullOrEmpty(s.ManufacturerName))
                {
                    name = s.ServiceObjectName;
                    _scanner = (Scanner)_explorer.CreateInstance(s);
                    break;
                }
            }
            Console.Read();
            return name;
        }

        public void SetScanner()
        {
            ///БЕЗ ИСПОЛЬЗОВАНИЯ HydraSO.dll

            //DeviceInfo device = _explorer.GetDevice(DeviceType.Scanner, "Voyager-1200");
            //_scanner = (Scanner)_explorer.CreateInstance(device);
            _scanner.Open();
            _scanner.Claim(1000);
            _scanner.DataEvent += scanner_DataEvent;
            _scanner.ErrorEvent += scanner_ErrorEvent;
            _scanner.DeviceEnabled = true; // throws here if normal user privileges
            _scanner.DataEventEnabled = true;
            _scanner.DecodeData = true;
        }

        private void scanner_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void scanner_DataEvent(object sender, DataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            _keyboardScanner.KeyPress(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //_keyboardScanner.Show();
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _keyboardScanner.Show();
        }
    }
}
