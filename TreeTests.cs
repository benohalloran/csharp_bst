using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SearchTree
{
	[TestFixture ()]
	public class TreeTests
	{
		const int per_thread = 10000;
		const int threads = 4;
		const int DataSize = per_thread * threads;
		Random random = new Random();
		//utility methods
		public void InParallel(int[] data, Action<int> lambda){
			Assert.That(data.Length == DataSize);
			Parallel.For(0, threads, (int thread_no) =>{
				for(int i = 0; i < per_thread; i++)
					lambda.Invoke(data[per_thread * thread_no + i]);
			});
		}
		//Returns a new SearchTree and populates data param with the data used
		public SearchTree buildTreeAndData(int[] data){
			Assert.That(data.Length == DataSize);
			var tree = new SearchTree();
			var added = new System.Collections.Generic.HashSet<int>();
			int data_index = 0;
			while(added.Count < DataSize){
				var val = random.Next(DataSize);
				if(added.Add(val)){
					data[data_index] = val;
					data_index++;
				}
			}
			InParallel(data, (int item) => {
				Assert.That(tree.Insert(item), "Couldn't insert unique item " + item);
				Assert.IsFalse(tree.Insert(item), "Inserted same item twice. item = " + item);
			});
			return tree;
		}

		[Test ()]
		public void InsertTest ()
		{
			//these functions contain Assert. methods
			var data = new int[DataSize];
			var tree = buildTreeAndData(data);
		}


		[Test ()]
		public void BuildAndDestroy()
		{
			var data = new int[DataSize];
			var tree = buildTreeAndData(data);
			//enter the destroy phase
			InParallel(data, (int item) => {
				Assert.That(tree.Remove(item), "Failed to remove " + item);
				Assert.IsFalse(tree.Contains(item), "Found " + item + " after removal");
			});
		}

		[Test ()]
		public void RandomTest(){
			var data = new int[DataSize];
			var tree = buildTreeAndData(data);
			InParallel(data, (int item) => {
				Assert.That(tree.Contains(item), "Couldn't find " + item + " in random testing");
				if(random.Next() % 2 == 0){
					//remove
					Assert.That(tree.Remove(item), "Failed to remove " + item + " in random testing");
				}
			});
		}
	}
}

