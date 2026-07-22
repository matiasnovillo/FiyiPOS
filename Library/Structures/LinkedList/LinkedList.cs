using System;
using System.Collections.Generic;
using System.Text;

namespace FiyiPOS.Library.Structures.LinkedList
{
    /// <summary>
    /// Utilidades para trabajar con LinkedList&lt;string&gt;.
    /// Para listas enlazadas como estructura, usar System.Collections.Generic.LinkedList&lt;T&gt; directamente.
    /// </summary>
    public static class LinkedListHelper
    {
        public static List<string> ToList(LinkedList<string> linkedList)
        {
            var result = new List<string>();
            foreach (var node in linkedList)
            {
                result.Add(node);
            }
            return result;
        }

        public static string[] ToStringArray(LinkedList<string> linkedList, int startIndex = 0)
        {
            var array = new string[linkedList.Count];
            linkedList.CopyTo(array, startIndex);
            return array;
        }

        public static string HighlightNode(LinkedListNode<string> node, string highlight)
        {
            if (node.List is null)
                throw new InvalidOperationException($"El nodo '{node.Value}' no pertenece a ninguna lista.");

            var builder = new StringBuilder();
            var current = node;

            // Recorrer hacia atrás
            var previous = current.Previous;
            var leftStack = new Stack<string>();
            while (previous != null)
            {
                leftStack.Push(previous.Value);
                previous = previous.Previous;
            }
            while (leftStack.Count > 0)
            {
                builder.Append(leftStack.Pop() + " ");
            }

            // Nodo resaltado
            builder.Append($"{highlight}{current.Value}{highlight}");

            // Recorrer hacia adelante
            current = current.Next;
            while (current != null)
            {
                builder.Append(" " + current.Value);
                current = current.Next;
            }

            return builder.ToString();
        }
    }
}
