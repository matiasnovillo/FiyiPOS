namespace FiyiPOS.Library.Structures.Graph
{
    public class Node
    {
        private Node? RootNode { get; set; } = null;
        private Node? LeftNode { get; set; }
        private Node? RightNode { get; set; }

        public string NodeName { get; set; } = string.Empty;

        public void Add(Node newNode)
        {
            if (string.IsNullOrWhiteSpace(newNode.NodeName))
                throw new ArgumentException("El nombre del nodo no puede ser nulo o vacío.");

            newNode.LeftNode = null;
            newNode.RightNode = null;

            if (RootNode == null)
            {
                RootNode = newNode;
            }
            else
            {
                var aux1 = RootNode;
                var aux2 = RootNode;

                while (aux2 != null)
                {
                    aux1 = aux2;
                    aux2 = Validator.CompareStrings(newNode.NodeName, aux2.NodeName) == 'B'
                        ? aux2.LeftNode
                        : aux2.RightNode;
                }

                if (Validator.CompareStrings(newNode.NodeName, aux1.NodeName) == 'B')
                    aux1.LeftNode = newNode;
                else
                    aux1.RightNode = newNode;
            }
        }

        public List<string> ToDataTable()
        {
            var list = new List<string> { "NodeName" };
            if (RootNode != null)
                InOrder(list, RootNode);
            return list;
        }

        public void Balance()
        {
            if (RootNode == null) return;
            int i = 0;
            SaveNodes(RootNode);
            RootNode = null;
            Balance(0, i - 1);
        }

        private void Balance(int start, int end)
        {
            if (start > end) return;

            int mid = (start + end) / 2;

            Add(Utilities.Nodes[mid]);
            Balance(start, mid - 1);
            Balance(mid + 1, end);
        }

        private static void InOrder(List<string> list, Node node)
        {
            if (node.LeftNode != null)
                InOrder(list, node.LeftNode);

            list.Add(node.NodeName);

            if (node.RightNode != null)
                InOrder(list, node.RightNode);
        }

        private static void SaveNodes(Node node)
        {
            if (node.LeftNode != null)
                SaveNodes(node.LeftNode);

            Utilities.Nodes[Utilities.i] = node;
            Utilities.i++;

            if (node.RightNode != null)
                SaveNodes(node.RightNode);
        }

        public Node? Peek(string searchString)
        {
            var aux = RootNode;

            while (aux != null)
            {
                if (searchString == aux.NodeName)
                    return aux;

                aux = Validator.CompareStrings(searchString, aux.NodeName) == 'B'
                    ? aux.LeftNode
                    : aux.RightNode;
            }

            return null;
        }

        public bool Search(string searchString)
        {
            var aux = RootNode;

            while (aux != null)
            {
                if (searchString == aux.NodeName)
                    return true;

                aux = Validator.CompareStrings(searchString, aux.NodeName) == 'B'
                    ? aux.LeftNode
                    : aux.RightNode;
            }

            return false;
        }
    }
}
