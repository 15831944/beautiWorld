using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//-------------------
//  能动少C71 尤佳睿
//    2150506070
//   高级程序设计
//-------------------


namespace PrimeTwin
{
	class Program
	{
		static void Main(string[] args)
		{
			string str=Console.ReadLine();
			int num = Convert.ToInt32(str);
			PrimeChart pc = new PrimeChart(num);
            pc.getPrimeTwins(num);
		}

	}
	class PrimeChart
	{
		public List<int> PrimeList;
		public PrimeChart(int max_num)
		{
			PrimeList = new List<int>();
			List<bool> isPrime = new List<bool>();
			int count_point = 0;
			for(int i=0;i<=max_num;i++)
			{
				isPrime.Add(true);
			}
			isPrime[0] = isPrime[1] = false;
			while(count_point<max_num)
			{
				if(isPrime[count_point]==true)
				{
					int p = count_point;
					for(int k=2*p;k<=max_num;k+=p)
						isPrime[k]=false;
				}
				count_point++;
			}
			for(int i=0;i<=max_num;i++)
			{
				if(isPrime[i]==true)
					PrimeList.Add(i);
			}
		}
		public void getPrimeTwins(int n)
		{
			if(n>=5)
				Console.Write("(3, 5)");
			for(int i=2;i<PrimeList.Count-1;i++)
			{
				if(PrimeList[i+1]>n)
					break;
				else if(PrimeList[i]+2==PrimeList[i+1])
					Console.Write(", ({0}, {1})",PrimeList[i],PrimeList[i+1]);
			}
		}
	}
}