C# Search Tree
==============
A Simple unbalanced Binary Search Tree with concurrent access.


This is a simple implementation of a Binary Search Tree ADT without any sense of balancing. The tree supports concurrency through a global reader-writer (RW) lock.

A RW lock allows for multiple readers to access the structure at the same time while requiring writers to have exclusive access. For a BST, this means any number of threads may perform a `Contains(int)` operation since the internal data is unmodified. For insertions and removals, exclusive access is required.

Using a RW lock is an obvious improvement over simple C# `lock(obj)` blocks since these denote mutually exclusive sections and would not allow for safe concurrent readers (in the event another thread begins changing the tree structure).

While finer grained RW locks on individual nodes of the tree may seem to allow for greater concurrency, in practice you must acquire the same reader or writer lock level for all nodes that you have visited up through to the head of the tree, or promote readers to writers for relevant nodes while modifing the data structure. However, acquiring all of these locks is very expensive and did not show any testing in a 4 core processor with 4 threads inserting into the tree.
