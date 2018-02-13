using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace SwitchBotGw.Services {
    public interface ISwitchBotService {
        Task<bool> Test(string deviceUUID, byte[] command);
    }

    class SwitchBotService : ISwitchBotService {
        const string SwitchBotServiceUUID = "cba20d00-224d-11e6-9fb8-0002a5d5c51b";
        public static readonly byte[] TurnOnCommand = new byte[] { 0x57, 0x01, 0x01 };
        public static readonly byte[] TurnOffCommand = new byte[] { 0x57, 0x01, 0x02 };

        private IDisposable ScanSub_;
        private IDisposable ServiceDiscoverSub_;
        private IDisposable WriteSub_;

        public Task<bool> Test(string deviceUUID,
            byte[] command) {
            bool success = false;

            var ScanObserbable = CrossBleAdapter.Current.Scan();
            ScanSub_ = ScanObserbable.Subscribe(sr => {
                Debug.WriteLine($"Scan Discovered:{sr.Device.Name}:{sr.Device.Uuid}:{sr.Rssi}");
                if (sr.Device.Uuid.ToString().Contains(deviceUUID)) {
                    var device = sr.Device;
                    device.Connect().Subscribe(co => {
                        Debug.WriteLine("Connected.");
                        ScanSub_.Dispose();

                        ServiceDiscoverSub_ = device
                            .WhenAnyCharacteristicDiscovered()
                            .Subscribe(ch => {
                                Debug.WriteLine($"Characteristic Discovered: {ch.Uuid}:{ch.Service.Uuid}/{ch.Service.Description}");
                                if (ch.Service.Uuid.ToString().Contains("cba20d00-224d-11e6-9fb8-0002a5d5c51b")) {
                                    if (ch.CanWrite()) {
                                        ServiceDiscoverSub_.Dispose();
                                        Debug.WriteLine($"Send Command [{BitConverter.ToString(command)}]");
                                        WriteSub_ = ch.Write(command)
                                            .Subscribe(cr => {
                                                Debug.WriteLine($"Write Result:{BitConverter.ToString(cr.Data)}");
                                                device.CancelConnection();
                                                WriteSub_.Dispose();
                                                success = true;
                                                return;
                                            });
                                    }
                                }
                            });
                    });
                }
            }, error => {
                Debug.WriteLine($"Failed {error.Message}");
            });
            return Task.FromResult(success);
        }
    }
}
