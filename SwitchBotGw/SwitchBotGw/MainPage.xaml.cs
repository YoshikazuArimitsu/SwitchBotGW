using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SwitchBotGw {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();

            Debug.WriteLine("Start scanning...");
            bool connected = false;
            bool sended = false;
            CrossBleAdapter.Current.Scan().Subscribe(sr => {
                Debug.WriteLine($"Scan Discovered:{sr.Device.Name}:{sr.Device.Uuid}:{sr.Rssi}");
                if (sr.Device.Uuid.ToString().Contains("cf46d116b6a1")) {
                    var device = sr.Device;

                    device.Connect().Subscribe(co => {
                        connected = true;
                        Debug.WriteLine("Connected.");

                        device.WhenAnyCharacteristicDiscovered().Subscribe(ch => {
                            string valueStr = ch.Value != null ? BitConverter.ToString(ch.Value) : "null";

                            Debug.WriteLine($"Characteristic Discovered: {valueStr}:{ch.Uuid}:{ch.Service.Uuid}/{ch.Service.Description}");
                            if (ch.Service.Uuid.ToString().Contains("cba20d00-224d-11e6-9fb8-0002a5d5c51b")) {
                                if (ch.CanWrite() && !sended) {
                                    var b = ch.Value;
                                    var cmd = $"570100";
                                    Debug.WriteLine($"Send Command [{cmd}]");
                                    ch.Write(new byte[] { 0x57, 0x01, 0x01})
                                        .Subscribe(cr => {
                                            sended = true;
                                            Debug.WriteLine($"Write Result:{BitConverter.ToString(cr.Data)}");
                                        });
                                }
                            }
                        });
                    });
                    
                }
            });
        }
    }
}
