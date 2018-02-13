﻿using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace SwitchBotGw.Services {
    public interface ISwitchBotService {
        Task<bool> TurnOnAsync(string device);
        Task<bool> TurnOffAsync(string device);

        Task DoDebugAsync();
    }

    public class SwitchBotService : ISwitchBotService {
        const string SwitchBotServiceUUID = "cba20d00-224d-11e6-9fb8-0002a5d5c51b";
        public static readonly byte[] TurnOnCommand = new byte[] { 0x57, 0x01, 0x01 };
        public static readonly byte[] TurnOffCommand = new byte[] { 0x57, 0x01, 0x02 };

        public async Task<bool> TurnOnAsync(string device) {
            return await ScanSendAsync(device, TurnOnCommand);
        }

        public async Task<bool> TurnOffAsync(string device) {
            return await ScanSendAsync(device, TurnOffCommand);
        }

        private async Task<bool> ScanSendAsync(string deviceUUID,
            byte[] command) {
            // 結果受信用Subject
            var resultSubject = new Subject<bool>();

            // BluetoothLE Scanストリーム
            IDisposable scanSubscribe = null;
            scanSubscribe = CrossBleAdapter.Current.Scan().Subscribe(sr => {
                // デバイス発見
                Debug.WriteLine($"Scan Discovered:{sr.Device.Name}:{sr.Device.Uuid}:{sr.Rssi}");
                if (sr.Device.Uuid.ToString().Contains(deviceUUID)) {
                    // 目的のデバイスなら接続
                    var device = sr.Device;
                    device.Connect().Subscribe(co => {
                        Debug.WriteLine("Connected.");
                        scanSubscribe.Dispose(); //スキャンを止める

                        // SwitchBotのCharacteristicに繋ぐ
                        IDisposable serviceDiscoverSub = null;
                        serviceDiscoverSub = device
                            .WhenAnyCharacteristicDiscovered()
                            .Subscribe(ch => {
                                Debug.WriteLine($"Characteristic Discovered: {ch.Uuid}:{ch.Service.Uuid}/{ch.Service.Description}");
                                if (ch.Service.Uuid.ToString().Contains("cba20d00-224d-11e6-9fb8-0002a5d5c51b")) {
                                    if (ch.CanWrite()) {
                                        serviceDiscoverSub.Dispose(); // サービス検索を止める

                                        // コマンド送信
                                        Debug.WriteLine($"Send Command [{BitConverter.ToString(command)}]");
                                        IDisposable writeSub = null;
                                        writeSub = ch.Write(command)
                                            .Subscribe(cr => {
                                                device.CancelConnection();
                                                writeSub.Dispose();

                                                // 結果ストリームにtrueを流す
                                                resultSubject.OnNext(true);
                                                resultSubject.OnCompleted();
                                                Debug.WriteLine($"Write() completed.");
                                            }, error => {
                                                Debug.WriteLine($"Write() failed {error.Message}");
                                                resultSubject.OnNext(false);
                                                resultSubject.OnCompleted();
                                            });
                                    }
                                }
                            }, error => {
                                Debug.WriteLine($"WhenAnyCharacteristicDiscovered() failed {error.Message}");
                                resultSubject.OnNext(false);
                                resultSubject.OnCompleted();
                            });
                    }, error => {
                        Debug.WriteLine($"Connect() failed {error.Message}");
                        resultSubject.OnNext(false);
                        resultSubject.OnCompleted();
                    }, () => {
                        device.CancelConnection();
                        Debug.WriteLine($"Connection Closed");
                    });
                }
            }, error => {
                Debug.WriteLine($"Failed {error.Message}");
                // 結果ストリームにfalseを流す
                resultSubject.OnNext(false);
                resultSubject.OnCompleted();
            });

            // タイムアウト検出用ストリーム
            IDisposable timeoutSubscribe = null;
            timeoutSubscribe = Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(l => {
                    Debug.WriteLine($"ScanSendAsync timeout.");
                    // 結果ストリームにfalseを流す
                    resultSubject.OnNext(false);
                    resultSubject.OnCompleted();

                    // スキャンを止める
                    timeoutSubscribe.Dispose();
                    scanSubscribe.Dispose();
                });

            return await resultSubject.ToTask();
        }

        public Task DoDebugAsync() {
            return Task.CompletedTask;
            /*
            // 結果受信用Subject
            var resultSubject = new Subject<bool>();

            // BluetoothLE Scanストリーム
            IDisposable scanSubscribe = null;
            scanSubscribe = CrossBleAdapter.Current.Scan().Subscribe(sr => {
                // デバイス発見
                Debug.WriteLine($"Scan Discovered:{sr.Device.Name}:{sr.Device.Uuid}:{sr.Rssi}");
                if (sr.Device.Uuid.ToString().Contains(deviceUUID)) {
                    // 目的のデバイスなら接続
                    var device = sr.Device;
                    device.Connect().Subscribe(co => {
                        Debug.WriteLine("Connected.");
                        ScanSub_.Dispose(); //スキャンを止める

                        // SwitchBotのCharacteristicに繋ぐ
                        ServiceDiscoverSub_ = device
                            .WhenAnyCharacteristicDiscovered()
                            .Subscribe(ch => {
                                Debug.WriteLine($"Characteristic Discovered: {ch.Uuid}:{ch.Service.Uuid}/{ch.Service.Description}");
                                if (ch.Service.Uuid.ToString().Contains("cba20d00-224d-11e6-9fb8-0002a5d5c51b")) {
                                    if (ch.CanWrite()) {
                                        ServiceDiscoverSub_.Dispose(); // サービス検索を止める

                                        // コマンド送信
                                        Debug.WriteLine($"Send Command [{BitConverter.ToString(command)}]");
                                        WriteSub_ = ch.Write(command)
                                            .Subscribe(cr => {
                                                Debug.WriteLine($"Write Result:{BitConverter.ToString(cr.Data)}");
                                                device.CancelConnection();
                                                WriteSub_.Dispose();

                                                // 結果ストリームにtrueを流す
                                                resultSubject.OnNext(true);
                                                resultSubject.OnCompleted();
                                                return;
                                            }, onError => {

                                            });
                                    }
                                }
                            });
                    });
                }
            }, error => {
                Debug.WriteLine($"Failed {error.Message}");
                // 結果ストリームにfalseを流す
                resultSubject.OnNext(false);
                resultSubject.OnCompleted();
            });

            // タイムアウト検出用ストリーム
            IDisposable timeoutSubscribe = null;
            timeoutSubscribe = Observable.Timer(TimeSpan.FromSeconds(10))
                .Subscribe(l => {
                    // 結果ストリームにfalseを流す
                    resultSubject.OnNext(false);
                    resultSubject.OnCompleted();

                    // スキャンを止める
                    timeoutSubscribe.Dispose();
                    scanSubscribe.Dispose();
                });


            var result = await resultSubject.ToTask();
            return;
            */
        }
    }
}
