public class TreeNode<T>
{
    public T Value;
    public TreeNode<T> Left;
    public TreeNode<T> Right;
    public int Height; 

    public TreeNode(T value)
    {
        Value = value;
        Left = null;
        Right = null;
        Height = 1; 
    }
}