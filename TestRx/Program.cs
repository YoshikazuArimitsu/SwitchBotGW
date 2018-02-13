using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace TestRx {
    class Program {
        static void Main(string[] args) {
            var resultSubject = new Subject<int>();

            var source = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            IDisposable sub1 = null;
            sub1 = source.Subscribe(l => {
                Console.WriteLine($"T1:{l}");
                if(l > 4) {
                    sub1.Dispose();
                    resultSubject.OnNext(5);
                    resultSubject.OnCompleted();
                }
            });
            var source2 = Observable.Timer(TimeSpan.FromSeconds(10), TimeSpan.Zero);

            IDisposable sub2 = null;
            sub2 = source2.Subscribe(t2 => {
                    Console.WriteLine("T2");
                    sub1.Dispose();
                    sub2.Dispose();
                });

            var r = resultSubject.ToTask().Result;
            Console.WriteLine($"R:{r}");

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
