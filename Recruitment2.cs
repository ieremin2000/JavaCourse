using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Threading;

namespace Starlizard.Recruitment
{
    public class PriceFeed : IPriceFeed
    {
        private readonly Subject<Price> _prices =  new Subject<Price>();
        private Random _random;
        private int _num = 0;

        public PriceFeed(string ticker)
        {
            Ticker = ticker;
            _random = new Random(DateTime.Now.GetHashCode());
        }

        public void Next()
        {
            _num = Interlocked.Increment(ref _num);
            double a = _num;
            _num = Interlocked.Increment(ref _num);
            double b = _num;

            //double a = _random.NextDouble();
            //double b = _random.NextDouble();
            //double min = Math.Min(a, b);
            //double max = Math.Max(a, b);
            //decimal bid = (decimal)(min * 1000);
            //decimal ask = (decimal)(max * 1000);
            
            decimal bid = (decimal)(a);
            decimal ask = (decimal)(b);

            Price price = new Price();
            price.Bid = bid;
            price.Offer = ask;

            _prices.OnNext(price);
        }

        public void Error()
        {
            _prices.OnError(new Exception("error"));
        }

        public void Complete()
        {
            _prices.OnCompleted();
        }

        public string Ticker { get; set; }
        public IObservable<Price> Prices => _prices;
        public Subject<Price> RawPrices => _prices;
    }
}
