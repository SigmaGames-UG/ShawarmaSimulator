using System;

public class AVL<T> : BST<T> where T : IComparable<T>
{
    private int Height(TreeNode<T> node)
    {
        return node == null ? 0 : node.Height;
    }

    private int GetBalance(TreeNode<T> node)
    {
        return node == null ? 0 : Height(node.Left) - Height(node.Right);
    }

    private TreeNode<T> RotateRight(TreeNode<T> y)
    {
        TreeNode<T> x = y.Left;
        TreeNode<T> T2 = x.Right;

        
        x.Right = y;
        y.Left = T2;

        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

        return x;
    }

    private TreeNode<T> RotateLeft(TreeNode<T> x)
    {
        TreeNode<T> y = x.Right;
        TreeNode<T> T2 = y.Left;

        y.Left = x;
        x.Right = T2;

        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

        return y;
    }

    public override void Insert(T value)
    {
        root = InsertRecAVL(root, value);
    }

    private TreeNode<T> InsertRecAVL(TreeNode<T> node, T value)
    {
        if (node == null) return new TreeNode<T>(value);

        int comparison = value.CompareTo(node.Value);
        if (comparison < 0)
            node.Left = InsertRecAVL(node.Left, value);
        else if (comparison > 0)
            node.Right = InsertRecAVL(node.Right, value);
        else
            return node; 

        node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;

        int balance = GetBalance(node);

        //CASOS DE ROTACIÓN 

        if (balance > 1 && value.CompareTo(node.Left.Value) < 0)
            return RotateRight(node);

        if (balance < -1 && value.CompareTo(node.Right.Value) > 0)
            return RotateLeft(node);

        if (balance > 1 && value.CompareTo(node.Left.Value) > 0)
        {
            node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }

        if (balance < -1 && value.CompareTo(node.Right.Value) < 0)
        {
            node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }

        return node;
    }
}