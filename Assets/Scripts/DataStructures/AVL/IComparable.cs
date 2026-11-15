using System;
using System.Collections.Generic;
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


    public List<T> ObtenerValoresEnOrden()
    {
        List<T> resultado = new List<T>();

        RecorrerEnOrden(root, resultado);

        return resultado;
    }

    private void RecorrerEnOrden(TreeNode<T> nodo, List<T> resultado)
    {
        if (nodo == null)
            return;

        RecorrerEnOrden(nodo.Left, resultado);

        resultado.Add(nodo.Value);

        RecorrerEnOrden(nodo.Right, resultado);
    }

}
