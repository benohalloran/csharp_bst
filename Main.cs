using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SearchTree
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			const int DataSize = 1000000; //one billion

			Console.WriteLine ("Hello World!");
			SearchTree mySearchTree = new SearchTree();
			var data = new int[DataSize];
			Console.Out.WriteLine("Generating Random data");
			for(int i = 0; i < DataSize; i++)
				data[i] = i;
			var rand = new Random();
			Array.Sort(data, (x, y) => rand.Next());			
			Console.Out.WriteLine("Inserting into tree");
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			Parallel.ForEach(data, (item) => {
				mySearchTree.Insert(item);
			});
			stopWatch.Stop();
			//from https://msdn.microsoft.com/en-us/library/system.diagnostics.stopwatch(v=vs.110).aspx
			var ts = stopWatch.Elapsed;
			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
				ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
			Console.Out.WriteLine(DataSize + " insertions completed in " + elapsedTime);

			var check = new int[DataSize];
			foreach(var treeNum in mySearchTree.InOrder()){
					//Console.Out.WriteLine(treeNum);
					check[treeNum]+=1;
			}
				
			Console.Out.WriteLine("Tree contains 5 " + mySearchTree.Contains(5));
			Console.Out.WriteLine("Tree contains -1 " + mySearchTree.Contains(-1));

			Debug.Assert(mySearchTree.Contains(5), "Tree did not contain 5");
			Debug.Assert(!mySearchTree.Contains(-1), "Tree had -1, which was not inserted");
			Debug.Assert(!mySearchTree.Insert(5), "Tree inserted 5 again");

			Console.Out.WriteLine("Test completed successfully");
		}
	}
}
