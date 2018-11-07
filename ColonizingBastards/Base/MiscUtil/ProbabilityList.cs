using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.MiscUtil
{

	
	class ProbabilityList<T, TKey>
	{

		private List<T> list;

		private Boolean listLocked;

		private Random random;

		private double mean;
		private double variance;


		public double MeanFactor { get; private set; }
		public double VarianceFactor { get; private set; }


		private Func<T, TKey> orderFunc;

		public ProbabilityList(double meanFactor, double varianceFactor, Func<T, TKey> orderFunc, Random random = null)
		{
			this.MeanFactor = meanFactor;
			this.VarianceFactor = varianceFactor;
			this.orderFunc = orderFunc;
			list = new List<T>();
			if (random == null)
			{
				this.random = new Random();
			}
			else
			{
				this.random = random;
			}
		}

		public void AddItem(T item)
		{
			if (listLocked)
				return;
			list.Add(item);
		}

		public void AddRange(IEnumerable<T> range)
		{
			if (listLocked)
				return;
			list.AddRange(range);
		}

		public void LockList(bool ascending)
		{
			listLocked = true;

			this.mean = MeanFactor * list.Count;
			this.variance = VarianceFactor * list.Count;

			// Sort list
			if (ascending)
			{
				list = list.OrderBy(orderFunc).ToList();
			}
			else
			{
				list = list.OrderByDescending(orderFunc).ToList();
			}
		}


		public T DrawItem()
		{
			 if (!listLocked)
				return default(T);

			if (list.Count <= 0)
				return default(T);

			T result;
			
			int index;
			
			double u1 = 1.0 - random.NextDouble();
			double u2 = 1.0 - random.NextDouble();
			double stdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); 
			double x = mean + variance * stdNormal;

			// calculate the index
			index = Math.Max(Math.Min((int) x, list.Count - 1), 0);

			result = list[index];
			list.Remove(result);

			return result;
		}

	}
}
