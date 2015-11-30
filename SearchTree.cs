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
				
		public SearchTree ()
		{
			head = null;		
		}
		
		public bool Insert(int Data){
			TreeNode node = new TreeNode(Data);	
			if(head == null){
				lock(this){
					head = node;
					return true;
				}
			}else
				lock(head){
					return InsertRecursive(node, head);
				}
		
		}
		

		public bool Remove(int key){
			lock(this){
				TreeNode parent, current;
				for(parent = null, current = head; current != null; 
					parent = current, current = key < current.Data ? current.Left : current.Right){
					if(current.Data == key){
						//remove current from the tree
						if(parent == null){
							//head is being removed
							Debug.Assert(current == head);
							throw new ArgumentException("Head removal not yet supported...");
						}else{
							var is_left = current == parent.Left;
							var new_root = is_left ? FindMin(current) : FindMax(current);
						}
					}
				}
			}
			return false;
		}

		private TreeNode FindMin(TreeNode node){
			if(node.Left == null) return node;
			else return FindMin(node.Left);
		}
		private TreeNode FindMax(TreeNode node){
			if(node.Right == null) return node;
			else return FindMax(node.Right);
		}


		public bool Contains(int key){
			return Contains(key, head);
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
			var DataDiff = DataNode.Data - SubTreeRoot.Data;

			if(DataDiff < 0){
				//goes to the left

					if(LeftValid)
						lock(SubTreeRoot.Left){
							return InsertRecursive(DataNode, SubTreeRoot.Left);
						}
					else{
						SubTreeRoot.Left = DataNode;
						return true;
					}
			}else if(DataDiff > 0){
				
					if(RightValid)
						lock(SubTreeRoot.Right){
							return InsertRecursive(DataNode, SubTreeRoot.Right);
						}
					else{
						SubTreeRoot.Right = DataNode;
						return true;
					}
			}else{
				return false; //in the tree
			}
		}
		
		public IEnumerable<int> InOrder(){
			foreach(var x in InOrder (head))
				yield return x;
		}
		
		private IEnumerable<int> InOrder(TreeNode head){
			if(head != null){
				foreach(var x in InOrder(head.Left))
					yield return x;
				yield return head.Data;
				foreach(var x in InOrder(head.Right))
					yield return x;
			}
		}			
	}
	class TreeNode {
		public int Data;
		public TreeNode Left, Right;

		public TreeNode(int _Data){
			Data = _Data;
		}
	}
}

