using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace ConsoleApp1
{
    static class ExtensionMethods
    {
        public static IDisposable Inspect<T>(this IObservable<T> self, string name)
        {
            return self.Subscribe(
                x => Console.WriteLine($"{name} has generates value {x}"),
                ex => Console.WriteLine($"{name} has generates exception {ex.Message}"),
                () => Console.WriteLine($"{name} has completed")
            );
        }

        // private static Random _random;

        public static IEnumerable<decimal> GenerateRandomDecimalEnum()
        {
            var random = new Random();
            for (; ; )
            {
                // Return some random numbers
                yield return new Decimal(random.NextDouble());
            }
        }

        public static IObservable<decimal> GenerateRandomDecObservable()
        {
            return GenerateRandomDecimalEnum().ToObservable();
        }

    }
}
