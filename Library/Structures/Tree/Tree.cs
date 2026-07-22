using static FiyiPOS.Library.Structures.Tree.Utilities;

namespace FiyiPOS.Library.Structures.Tree
{
    public class Tree
    {
        public Node? RootNode { get; private set; }

        private readonly Node[] Nodes = new Node[100];
        private int i;

        public void Add(Node newNode)
        {
            newNode.LeftNode = null;
            newNode.RightNode = null;

            if (RootNode == null)
            {
                RootNode = newNode;
                return;
            }

            var aux = RootNode;
            Node parent = RootNode;

            while (aux != null)
            {
                parent = aux;
                aux = newNode.NodeId < aux.NodeId ? aux.LeftNode : aux.RightNode;
            }

            if (newNode.NodeId < parent.NodeId)
                parent.LeftNode = newNode;
            else
                parent.RightNode = newNode;
        }

        public void Balance()
        {
            i = 0;
            if (RootNode != null)
                SaveVector(RootNode);
            RootNode = null;
            Balance(0, i - 1);
        }

        public void Remove(int nodeId)
        {
            i = 0;
            if (RootNode != null)
                SaveVector(RootNode, nodeId);
            RootNode = null;
            Balance(0, i - 1);
        }

        public Node? SearchNode(int nodeId)
        {
            var aux = RootNode;

            while (aux != null)
            {
                if (nodeId == aux.NodeId) return aux;
                aux = nodeId < aux.NodeId ? aux.LeftNode : aux.RightNode;
            }

            return null;
        }

        public List<int?> ToList(EListMode mode)
        {
            var list = new List<int?>();

            switch (mode)
            {
                case EListMode.InOrder:
                    InOrder(list, RootNode);
                    break;
                case EListMode.PreOrder:
                    PreOrder(list, RootNode);
                    break;
                case EListMode.PostOrder:
                    PostOrder(list, RootNode);
                    break;
                default:
                    throw new InvalidOperationException("Modo de listado inválido.");
            }

            return list;
        }

        private static void InOrder(List<int?> list, Node? node)
        {
            if (node == null)
            {
                list.Add(null);
                return;
            }

            InOrder(list, node.LeftNode);
            list.Add(node.NodeId);
            InOrder(list, node.RightNode);
        }

        private static void PreOrder(List<int?> list, Node? node)
        {
            if (node == null)
            {
                list.Add(null);
                return;
            }

            list.Add(node.NodeId);
            PreOrder(list, node.LeftNode);
            PreOrder(list, node.RightNode);
        }

        private static void PostOrder(List<int?> list, Node? node)
        {
            if (node == null)
            {
                list.Add(null);
                return;
            }

            PostOrder(list, node.LeftNode);
            PostOrder(list, node.RightNode);
            list.Add(node.NodeId);
        }

        private void Balance(int start, int end)
        {
            if (start > end) return;

            int mid = (start + end) / 2;
            Add(Nodes[mid]);
            Balance(start, mid - 1);
            Balance(mid + 1, end);
        }

        private void SaveVector(Node node)
        {
            if (node.LeftNode != null)
                SaveVector(node.LeftNode);

            Nodes[i++] = node;

            if (node.RightNode != null)
                SaveVector(node.RightNode);
        }

        private void SaveVector(Node node, int nodeId)
        {
            if (node.LeftNode != null)
                SaveVector(node.LeftNode, nodeId);

            if (node.NodeId != nodeId)
                Nodes[i++] = node;

            if (node.RightNode != null)
                SaveVector(node.RightNode, nodeId);
        }
    }
}
