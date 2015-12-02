using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SearchTree
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			const int threads = 3;
			const int per_thread = 1000000;
			const int DataSize = threads * per_thread;
			SearchTree mySearchTree = new SearchTree();
			var random = new Random();
			var added = new int[DataSize];
			Console.Out.WriteLine("Inserting into tree");
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			Parallel.For(0, threads, (int thread) => {
				for(int i = 0; i < per_thread; i++){
					mySearchTree.Insert(added[thread * per_thread + i] = random.Next(DataSize));
				}
			});
			stopWatch.Stop();
			var ts = stopWatch.Elapsed;
			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
				ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
			Console.Out.WriteLine(DataSize + " insertions completed in " + elapsedTime + " on " + threads + " threads");

			var check = new int[DataSize];
			foreach(var treeNum in mySearchTree.InOrder()){
				check[treeNum]+=1;
			}
				
			Debug.Assert(!mySearchTree.Contains(-1), "Tree had -1, which was not inserted");

			foreach(var item in added){
				Debug.Assert(mySearchTree.Contains(item), "Tree couldn't find " + item);
				check[item]= Math.Max(check[item] - 1, 0);
			}
			Debug.Assert(Array.TrueForAll(check, (item) => item == 0), "Invalid enumerator!");

			//remove ~ 1/2 of the nodes, check for validity
			var removed = new System.Collections.Generic.HashSet<int>();
			for(var index = 0; index < DataSize; index+=2){
				var val = added[index];
				if(!removed.Contains(val)){
					removed.Add(val);
					Debug.Assert(mySearchTree.Remove(val), "Couldn't remove " + val);
					Debug.Assert(!mySearchTree.Contains(val), "Found " + val + " after removal");
				}
			}
			Console.Out.WriteLine("Test completed successfully");
		}
	}
}
