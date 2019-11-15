﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using tec;

namespace TEC_Game
{
    class SchemeController
    {
        private GameController gameController;

        public SchemeController(GameController controller)
        {
            gameController = controller;
        }

        public void PlaceNode(ref string line)
        {
            int id = Int32.Parse(GetSubString(ref line, line.IndexOf(' '))); //id к=узла

            int row = Int32.Parse(GetSubString(ref line, line.IndexOf(' '))); //номер строки и столбца в grid для узла
            int column = Int32.Parse(GetSubString(ref line, line.Length));

            Node node = new Node(id, column, row);

            gameController.scheme.AddNode(node); //Добавление узла в схему

            node.Template = gameController.gameWindow.FindResource("NodeTemplate") as ControlTemplate; //Задания шаблона для узла

            node.Click += new RoutedEventHandler(gameController.OnNodeClick); //Добавление обработчика нажатия

            node.Content = id.ToString(); //Задание текста на узле

            node.SetValue(Grid.RowProperty, row); //Задание положения узла в grid
            node.SetValue(Grid.ColumnProperty, column);
            node.SetValue(Panel.ZIndexProperty, 2);

            gameController.gameWindow.GameGrid.Children.Add(node); //Добавление узла в grid
        }

        public void PlaceElement(ref string line, string type)
        {
            int id = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            int row = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
            int column = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            string direction = GetSubString(ref line, 1);

            int node1Id = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
            int node2Id = Int32.Parse(GetSubString(ref line, line.Length));

            Node node1 = gameController.scheme.GetNode(node1Id);
            Node node2 = gameController.scheme.GetNode(node2Id);

            BaseElement element;

            switch (type)
            {
                case "Re":
                    element = new Resistor(node1, node2, id);
                    break;
                case "No":
                    element = new Norator(node1, node2, id);
                    break;
                case "Nu":
                    element = new Nullator(node1, node2, id);
                    break;
                default:
                    element = new Conductor(node1, node2, id);
                    break;

            }

            gameController.scheme.AddElement(element);

            element.GetImage().SetValue(Grid.RowProperty, row);
            element.GetImage().SetValue(Grid.ColumnProperty, column);
            element.GetImage().SetValue(Panel.ZIndexProperty, 1);

            if (direction == "R")
            {
                element.ChangeImageDirectionToLand();

                element.GetImage().SetValue(Grid.RowSpanProperty, 3);
                element.GetImage().SetValue(Grid.ColumnSpanProperty, 9);
            }
            else
            {
                element.GetImage().SetValue(Grid.RowSpanProperty, 9);
                element.GetImage().SetValue(Grid.ColumnSpanProperty, 3);
            }

            gameController.gameWindow.GameGrid.Children.Add(element.GetImage());
        }

        public void PlaceWire(ref string line)
        {
            int id = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            int startRow = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
            int startColumn = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            int length = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            string direction = GetSubString(ref line, 1);

            Wire wire = new Wire(id, startRow, startColumn);

            gameController.scheme.AddWire(wire);

            wire.GetImage().SetValue(Grid.RowProperty, startRow);
            wire.GetImage().SetValue(Grid.ColumnProperty, startColumn);
            wire.GetImage().SetValue(Panel.ZIndexProperty, 1);

            if (direction == "R")
            {
                wire.ChangeImageDirectionToLand();
                wire.GetImage().SetValue(Grid.ColumnSpanProperty, length);
            }
            else
            {
                wire.GetImage().SetValue(Grid.RowSpanProperty, length);
            }

            gameController.gameWindow.GameGrid.Children.Add(wire.GetImage());
        }

        public string GetSubString(ref string line, int len)
        {
            string ans = line.Substring(0, len);
            line = line.Length <= len + 1 
                 ? "" 
                 : line.Substring(len + 1);
            return ans;
        }

        public void FindPlaceAndCreateNullor(Node node1, Node node2, string type)
        {
            List<BaseElement> blockingElements = FindBlockingElements(node1, node2);

            int row = 0;
            int column = 0;
            int id = gameController.scheme.GetElementsSize();
            string line = "";

            if (node1.GetX() < node2.GetX())
                column = node1.GetX();
            else
                column = node2.GetX();

            if (node1.GetY() < node2.GetY())
                row = node1.GetY();
            else
                row = node2.GetY();

            if ((node1.GetX() != node2.GetX()) && (node1.GetY() == node2.GetY()))
            {
                if (blockingElements.Count == 0)
                {
                    int length = Math.Abs(node1.GetX() - node2.GetX());

                    if (length > 8)
                    {
                        int wireId = gameController.scheme.GetWiresCount();
                        line = wireId + " " + row + " " + (column + 8) + " " + (length - 7) + " R";

                        PlaceWire(ref line);
                        line = "";
                    }

                    if (type == "Nu")
                    {
                        line = id + " " + (row - 1) + " " + column + " R " + node1.GetId() + " " + node2.GetId();
                    }
                    else
                    {
                        line = id + " " + (row - 1) + " " + column + " R " + node1.GetId() + " " + node2.GetId();
                    }

                    PlaceElement(ref line, type);
                }
                else
                {
                    //TO DO
                }
                //Если элемент будет расположен горизонтально
            }
            else if ((node1.GetX() == node2.GetX()) && (node1.GetY() != node2.GetY()))
            {
                if (blockingElements.Count == 0)
                {
                    int length = Math.Abs(node1.GetY() - node2.GetY());

                    if (length > 8)
                    {
                        int wireId = gameController.scheme.GetWiresCount();
                        line = wireId + " " + (row + 8) + " " + column + " " + (length - 7) + " D";

                        PlaceWire(ref line);
                        line = "";
                    }

                    if (type == "Nu")
                    {
                        line = id + " " + row + " " + (column - 1) + " D " + node1.GetId() + " " + node2.GetId();
                    }
                    else
                    {
                        line = id + " " + row + " " + (column - 1) + " D " + node1.GetId() + " " + node2.GetId();
                    }

                    PlaceElement(ref line, type);
                }
                else
                {
                    //TO DO
                }
                //Если элемент будет расположен вертикально
            }
            else
            {
                //Если есть выбор
            }
        }

        private List<BaseElement> FindBlockingElements(Node node1, Node node2)
        {
            List<BaseElement> ans = new List<BaseElement>();
            Node node = node1.Clone() as Node;

            while (node.GetX() != node2.GetX())
            {
                if (FindRightElement(node) != null)
                  ans.Add(FindRightElement(node));

                node = gameController.scheme.GetRightNode(node);
            }

            while (node.GetY() != node2.GetY())
            {
                if (FindDownElement(node) != null)
                    ans.Add(FindDownElement(node));

                node = gameController.scheme.GetDownNode(node);
            }

            if ((node1.GetX() != node2.GetX()) && (node1.GetY() != node2.GetY()))
            {
                node = node1.Clone() as Node;

                while (node.GetY() != node2.GetY())
                {
                    if (FindDownElement(node) != null)
                        ans.Add(FindDownElement(node));

                    node = gameController.scheme.GetDownNode(node);
                }

                while (node.GetX() != node2.GetX())
                {
                    if (FindRightElement(node) != null)
                        ans.Add(FindRightElement(node));

                    node = gameController.scheme.GetRightNode(node);
                }
            }

            return ans;
        }

        private BaseElement FindDownElement(Node node)
        {
            Node temp = gameController.scheme.GetDownNode(node);

            foreach (var element in node.GetConnectedElements())
                if ((element.GetNode1() == temp) || (element.GetNode2() == temp))
                    return element;
            return null;
        }

        private BaseElement FindRightElement(Node node)
        {
            Node temp = gameController.scheme.GetRightNode(node);

            foreach (var element in node.GetConnectedElements())
                if ((element.GetNode1() == temp) || (element.GetNode2() == temp))
                    return element;
            return null;
        }
    }
}
