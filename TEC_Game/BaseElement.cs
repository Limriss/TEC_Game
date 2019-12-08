﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tec
{
    abstract class BaseElement
    {
        protected Image image;
        private Node node1, node2;
        private int id;

        protected BaseElement(Node node1, Node node2, int id)
        {
            image = null;
            this.node1 = node1;
            this.node2 = node2;
            this.id = id;
        }

        public void ChangeNode(Node oldNode, Node newNode)
        {
            if (oldNode == node1)
            {
                node1.RemoveElement(this);
                node1 = newNode;
                node1.AddConnectedElement(this);
            }

            if (oldNode == node2)
            {
                node2.RemoveElement(this);
                node2 = newNode;
                node2.AddConnectedElement(this);
            }
        }

        public Node GetNode1()
        {
            return node1;
        }

        public Image GetImage()
        {
            return image;
        }

        public Node GetNode2()
        {
            return node2;
        }

        public int GetId()
        {
            return id;
        }

        public abstract void Destroy();
        public abstract void ChangeImageDirectionToLand();
    }
}
