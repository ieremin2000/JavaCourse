using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;

namespace Starlizard.Recruitment
{
//    Please review the following code for 

//	bugs
//	algorithm and data structure issues
//	general quality issues
//	potential architectural design problems

//  Please consider and note potential issues that maybe encountered in a single and multi-threaded environment.
//    You can add your review comments using inline comments or in a separate document but please do not modify the original code.


    public interface IPriceFeed
    {
        string Ticker { get; set; }
        IObservable<Price> Prices { get; }
        Subject<Price> RawPrices { get; }
    }

    // interface not used - it shall be used by RollingAverageMidPriceAggregator
    public interface IAggregator<T>
    {
        IObservable<T> Prices { get; }
    }

    public class Price
    {
        public decimal Bid { get; set; }
        public decimal Offer { get; set; }

        public override string ToString()
        {
            return $"Bid : {Bid}, Offer : {Offer}";
        }
    }

    /// <summary>
    /// Calculate the rolling average price on a stream of price updates
    /// </summary>
    public class RollingAverageMidPriceAggregator
    {
        // prices variable is not initialized - NullReferenceException will be thrown
        private List<Price> _prices; 

        private Subject<double> _output = new Subject<double>();


        public RollingAverageMidPriceAggregator(IPriceFeed priceFeed)
        {
            // !!! 
            _prices = new List<Price>();

            // no check if prices and priceFeed are non-null
            // possible design violation as we do not pass Observables further which is against ideology of Reactive extensions
            // getting last 20 prices could be done via priceFeed.RawPrices.Buffer(20, 1).Subscribe(OnPriceList);
            priceFeed.Prices.Subscribe(OnNewPrice);

            priceFeed.RawPrices.Buffer(3, 1).Subscribe(OnPriceList);
        }

        private void OnPriceList(IList<Price> priceList)
        {
            Console.WriteLine($"Came list = {string.Join(",",priceList)}");
        }

        private void OnNewPrice(Price prices)
        {
            Console.WriteLine($"Came price bid = {prices.Bid} offer = {prices.Offer}");

            // Unsafe and non-atomic in multi-threaded operation, if code needs to maintain running list of last 20 items
            _prices.Add(prices);

            // Unsafe and non-atomic in multi-threaded operation, if code needs to maintain running list of last 20 items
            if (_prices.Count == 20)
            {
                // Removing the first element is very expensive operation for list
                // Lidt has to be replaced by LinkedList or Dictonary containers which have O(1) time complexity for adding and removal operations 
                _prices.RemoveAt(0);
            }

            var start = DateTime.Now;
            var midPrices = _prices.Select(x => (x.Bid + x.Offer) / 2);
            Console.WriteLine("Calculating mid prices took " + (DateTime.Now - start).Milliseconds + "ms");

            var averageMidPrice = midPrices.Average();
            Console.WriteLine($"Calculating average of {averageMidPrice} took " + (DateTime.Now - start).Milliseconds + "ms");

            _output.OnNext((double)averageMidPrice);
        }

        // completion absent
        public void Complete()
        {
            _output.OnCompleted();
        }

        public IObservable<double> Prices
        {
            get
            {
                return _output;
            }
        }
    }
}

