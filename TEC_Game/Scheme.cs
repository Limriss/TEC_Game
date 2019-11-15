﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace tec
{
    class Scheme
    {
        private List<BaseElement> elements;
        private List<Node> nodes;
        //Нужен список проводов

        public Scheme()
        {
            elements = new List<BaseElement>();
            nodes = new List<Node>();
        }

        public bool HasNullor()
        {
            bool hasNullator = false;
            bool hasNorator = false;
            foreach (var element in elements)
            {
                if (element is Nullator) hasNullator = true;
                if (element is Norator) hasNorator = true;
            }
            return ((hasNullator == true) && (hasNorator == true));
        }

        public bool SchemeIsConnected()
        {
            foreach (var element in elements)
            {
                if (element.GetNode2() == null)
                    return false;
            }

            return true;
        }

        public Nullator FindNullator()
        {
            foreach (var element in elements)
            {
                if (element is Nullator)
                    return (Nullator) element;
            }

            return null;
        }

        public Norator FindNorator()
        {
            foreach (var element in elements)
            {
                if (element is Norator)
                    return (Norator)element;
            }

            return null;
        }

        public int GetNodeConnectionsCount(Node node)
        {
            return node.GetConnectedElementsCount();
        }

        public int GetElementsSize()
        {
            return elements.Count();
        }

        public int GetNodesCount()
        {
            return nodes.Count;
        }

        public void AddElement(BaseElement element)
        {
            elements.Add(element);
            nodes[element.GetNode1().GetId() - 1].AddConnectedElement(element);
            nodes[element.GetNode2().GetId() - 1].AddConnectedElement(element);
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public Node GetNode(int id)
        {
            foreach (var node in nodes)
            {
                if (node.GetId() == id)
                    return node;
            }

            return null;
        }

        public Node GetNode(int X, int Y)
        {
            foreach (var node in nodes)
            {
                if ((node.GetX() == X) && (node.GetY() == Y))
                    return node;
            }

            return null;
        }

        public Node GetRightNode(Node node)
        {
            int x = node.GetX();
            while (true)
            {
                x++;
                if (GetNode(x, node.GetY()) != null)
                {
                    return GetNode(x, node.GetY());
                }
            }
        }

        public Node GetDownNode(Node node)
        {
            int y = node.GetY();
            while (true)
            {
                y++;
                if (GetNode(node.GetX(), y) != null)
                {
                    return GetNode(node.GetX(), y);
                }
            }
        }

        public void RemoveElement(BaseElement element)
        {
            elements.Remove(element);
            nodes[element.GetNode1().GetId() - 1].RemoveElement(element);
            nodes[element.GetNode2().GetId() - 1].RemoveElement(element);
            element.Destroy();
            element = null;
        }
    }
}
