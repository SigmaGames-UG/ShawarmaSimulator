using System;

public class BST<T> where T : IComparable<T>
{
    protected TreeNode<T> root;

    public virtual void Insert(T value)
    {
        root = InsertRec(root, value);
    }

    protected virtual TreeNode<T> InsertRec(TreeNode<T> node, T value)
    {
        if (node == null) return new TreeNode<T>(value);

        int comparison = value.CompareTo(node.Value);
        if (comparison < 0)
            node.Left = InsertRec(node.Left, value);
        else if (comparison > 0)
            node.Right = InsertRec(node.Right, value);
        else
            return node; // No permite duplicados exactos

        return node;
    }

    public bool Search(T value)
    {
        return SearchRec(root, value);
    }

    private bool SearchRec(TreeNode<T> node, T value)
    {
        if (node == null) return false;

        int comparison = value.CompareTo(node.Value);
        if (comparison == 0) return true;

        return comparison < 0 ? SearchRec(node.Left, value) : SearchRec(node.Right, value);
    }

    public void Clear()
    {
        root = null;
    }
}