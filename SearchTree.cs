using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SearchTree
{
	/* An unbalanced binary search tree protected by a singe RW lock.
	 *
	 */
	public class SearchTree
	{
		private TreeNode head;
		private ReaderWriterLockSlim treeLock;
		public SearchTree ()
		{
			head = null;
			treeLock = new ReaderWriterLockSlim();
		}

		/**
		 * Insert an item into the tree
		 *
		 * @param {[type]} int Data the new item to add to the three
		 * @return true if the item was inserted (ie it was not already in the tree)
		 */
		public bool Insert(int Data){
			TreeNode DataNode = new TreeNode(Data);
			bool success = false;
			treeLock.EnterWriteLock();
			if(head == null){
				head = DataNode;
				success = true;
			}else{
				var node = head;
				while(node != null && !success){
					var LeftValid = node.Left != null;
					var RightValid = node.Right != null;
					if(DataNode.Data < node.Data){
						if(LeftValid) node = node.Left;
						else{
							node.Left = DataNode;
							success = true;
						}
					}else if(DataNode.Data > node.Data){
						if(RightValid) node = node.Right;
						else{
							node.Right = DataNode;
							success = true;
						}
					}else break;
				}

			}
			treeLock.ExitWriteLock();
			return success;
		}

		/**
		 * Remove an element from the tree.
		 *
		 * @param {[type]} int key @param key the data to be removed
		 * @return true if the item was removed without error
		 */
		public bool Remove(int key){
			treeLock.EnterWriteLock();
			if(head == null){
				treeLock.ExitWriteLock();
				return false;
			}else {
				if(head.Data == key){
					var auxRoot = new TreeNode(0); //the 0 value is just a dumby value. Not used for anything
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
		/* Search the tree for a data element
		 *
		 * @param {[type]} int key @param key to look up
		 * @return true if key is in the tree
		 */
		public bool Contains(int key){
			bool success = false;
			treeLock.EnterReadLock();
			var node = head;
			while(node != null && !success){
				if(key < node.Data)
					node = node.Left;
				else if(key > node.Data)
					node = node.Right;
				else success = true;
			}
			treeLock.ExitReadLock();
			return success;
		}

		/**
		 * Iterate over the elements in the tree using inder order traversal,
		 * e.g. 1, 2, 3 etc
		 */
		public IEnumerable<int> InOrder(){
			treeLock.EnterReadLock();
			if(head != null){
				foreach(var x in InOrder (head))
					yield return x;
			}
			treeLock.ExitReadLock();
		}
		//recursive helper routine for the InOrder enumerator
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
