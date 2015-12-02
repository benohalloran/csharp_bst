using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SearchTree
{
	public class SearchTree
	{
		private TreeNode head;
		private ReaderWriterLockSlim treeLock;
		public SearchTree ()
		{
			head = null;		
			treeLock = new ReaderWriterLockSlim();
		}
		
		public bool Insert(int Data){
			TreeNode node = new TreeNode(Data);
			bool success;
			treeLock.EnterWriteLock();
			if(head == null){
				head = node;
				success = true;
			}else{
				success = InsertRecursive(node, head);
			}
			treeLock.ExitWriteLock();
			return success;
		}
		

		public bool Remove(int key){
			treeLock.EnterWriteLock();
			if(head == null){
				treeLock.ExitWriteLock(); 
				return false;
			}else {
				if(head.Data == key){
					var auxRoot = new TreeNode(0);
					auxRoot.Left = head;
					var result = head.Remove(key, auxRoot);
					head = auxRoot.Left;
					treeLock.ExitWriteLock();
					return result;
				}else{
					var success = head.Remove(key, null);
					treeLock.ExitWriteLock();
					return success;
				}
			}
		}

		public bool Contains(int key){
			bool success;
			treeLock.EnterReadLock();
			success = Contains(key, head);
			treeLock.ExitReadLock();
			return success;
		}
		
		private bool Contains(int key, TreeNode node){
			if(node == null) return false;
			if(node.Data == key) return true;
			if(key < node.Data) return Contains(key, node.Left);
			if(key > node.Data) return Contains (key, node.Right);
			return false;
		}

		//on method entry, the mutex for SubTreeNode is aquired by the current thread. There is no need to lock
		// Data node since it is thread-local (was created in public function, passed in as simple parameter)
		private bool InsertRecursive(TreeNode DataNode, TreeNode SubTreeRoot){
			var LeftValid = SubTreeRoot.Left != null;
			var RightValid = SubTreeRoot.Right != null;
			bool ret_val; 

			var DataDiff = DataNode.Data - SubTreeRoot.Data;

			if(DataDiff < 0){
				if(LeftValid)
					ret_val = InsertRecursive(DataNode, SubTreeRoot.Left);
				else{
					SubTreeRoot.Left = DataNode;
					ret_val = true;
				}
			}else if(DataDiff > 0){
				if(RightValid)
					ret_val = InsertRecursive(DataNode, SubTreeRoot.Right);
				else{
					SubTreeRoot.Right = DataNode;
					ret_val = true;
				}
			}else{
				ret_val = false; //in the tree
			}
			return ret_val;
		}
		
		public IEnumerable<int> InOrder(){
			treeLock.EnterReadLock();
			if(head != null){
				foreach(var x in InOrder (head))
					yield return x;
			}
			treeLock.ExitReadLock();
		}
		
		private IEnumerable<int> InOrder(TreeNode node){
			if(node != null){
				foreach(var x in InOrder(node.Left))
					yield return x;
				yield return head.Data;
				foreach(var x in InOrder(node.Right))
					yield return x;
			}
		}			
	}
	class TreeNode {
		public int Data;
		public TreeNode Left, Right;

		public TreeNode( int _Data){
			Data = _Data;
		}

		public bool Remove(int value, TreeNode parent){
			if(value < this.Data){
				if(Left != null) return Left.Remove(value, this);
				else return Right.Remove(value, this);
			}else if(value > this.Data){
				if(Right != null) return Right.Remove(value, this);
				else return false;
			}else{
				if(Left != null && Right != null){
					this.Data = Right.FindMin().Data;
					Right.Remove(this.Data, this);
				}else if(parent.Left == this){
					parent.Left = (Left == null) ? Right : Left;
				}else if(parent.Right == this){
					parent.Right = (Left == null) ? Right : Left;
				}
				return true;
			} // end mutex region
		}
		private TreeNode FindMin(){
			if(this.Left == null) return this;
			else return Left.FindMin();
		}
		private TreeNode FindMax(){
			if(this.Right == null) return this;
			else return Right.FindMax();
		}
	}
}

