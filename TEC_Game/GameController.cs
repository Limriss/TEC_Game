﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TEC_Game;

namespace tec
{
    class GameController
    {
        public GameWindow gameWindow;
        public Scheme scheme;
        public Player player;
        private SchemeController schemeController;

        public GameController(Player player, Scheme scheme)
        {
            this.player = player;
            this.scheme = scheme;
            schemeController = new SchemeController(this);

            //Находим объект игрового окна для добавления элементов в него
            gameWindow = null;
            foreach (Window window in Application.Current.Windows)
                if (window is GameWindow) gameWindow = window as GameWindow;

            MakeGrid();

            //Обработчики для кнопок, добавляющих нуллор
            gameWindow.addNoratorButton.Click += new RoutedEventHandler(OnNoratorButtonClick);
            gameWindow.addNullatorButton.Click += new RoutedEventHandler(OnNullatorButtonClick);
        }

        private void MakeGrid()
        {
            for (int i = 0; i < 40; i++)
            {
                gameWindow.GameGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 40; i++)
            {
                gameWindow.GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            ContentControl control = new ContentControl
            {
                Template = gameWindow.FindResource("UndefinedTemplate") as ControlTemplate,
                Background = new SolidColorBrush(Color.FromRgb(225, 226, 225))
            };

            control.SetValue(Grid.RowSpanProperty, 40);
            control.SetValue(Grid.ColumnSpanProperty, 40);
            control.SetValue(Panel.ZIndexProperty, 0);

            gameWindow.GameGrid.Children.Add(control);
        }

        public void InitializeScheme(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line = reader.ReadLine();

                    while (line != "")
                    {
                        string type = schemeController.GetSubString(ref line, 2);
                        switch (type)
                        {
                            case "Nd":
                                schemeController.PlaceNode(ref line);
                                break;
                            case "Co":
                            case "Re":
                                schemeController.PlaceElement(ref line, type);
                                break;
                            case "Wi":
                                schemeController.PlaceWire(ref line);
                                break;
                        }
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void StartElimination()
        {
            //TO DO
        }

        private void OnNodeLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = Brushes.Black;
        }

        private void OnNodeEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = Brushes.Aqua;
        }

        public void OnNodeClick(object sender, RoutedEventArgs e)
        {
            Node node = sender as Node;
            if ((node.Background == Brushes.Black) && (!player.NodesChosen()))
            {
                node.Background = Brushes.Blue;

                player.ChooseNode(node);

                if (player.NodesChosen())
                {
                    //Here we need to add animations to slides
                    gameWindow.addNoratorButton.IsEnabled = true;
                    gameWindow.addNoratorButton.Background = new SolidColorBrush(Color.FromRgb(255, 110, 64));
                    gameWindow.addNullatorButton.IsEnabled = true;
                    gameWindow.addNullatorButton.Background = new SolidColorBrush(Color.FromRgb(255, 110, 64));
                }
            }
            else if ((node.Background == Brushes.Black) && (player.NodesChosen()))
            {
                //Make text on screen
                //print alarm on it
            }
            else if (node.Background == Brushes.Blue)
            {
                node.Background = Brushes.Black;

                if (player.NodesChosen())
                {
                    //Here we need to add animations to slides
                    gameWindow.addNoratorButton.IsEnabled = false;
                    gameWindow.addNoratorButton.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                    gameWindow.addNullatorButton.IsEnabled = false;
                    gameWindow.addNullatorButton.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                }

                player.RemoveNode(node);
            }

            e.Handled = true;
        }

        public void OnNoratorButtonClick(object sender, RoutedEventArgs e)
        {
            schemeController.FindPlaceAndCreateNullor(player.GetNodeChosen1(), player.GetNodeChosen2(), "No");

            player.GetNodeChosen1().Background = Brushes.Black;
            player.GetNodeChosen2().Background = Brushes.Black;
            player.RemoveNode(player.GetNodeChosen1());
            player.RemoveNode(player.GetNodeChosen2());

            gameWindow.addNoratorButton.IsEnabled = false;
            gameWindow.addNoratorButton.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            gameWindow.addNullatorButton.IsEnabled = false;
            gameWindow.addNullatorButton.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));

            e.Handled = true;
        }

        public void OnNullatorButtonClick(object sender, RoutedEventArgs e)
        {
            schemeController.FindPlaceAndCreateNullor(player.GetNodeChosen1(), player.GetNodeChosen2(), "Nu");

            player.GetNodeChosen1().Background = Brushes.Black;
            player.GetNodeChosen2().Background = Brushes.Black;
            player.RemoveNode(player.GetNodeChosen1());
            player.RemoveNode(player.GetNodeChosen2());

            gameWindow.addNoratorButton.IsEnabled = false;
            gameWindow.addNoratorButton.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            gameWindow.addNullatorButton.IsEnabled = false;
            gameWindow.addNullatorButton.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));

            e.Handled = true;
        }

        public void ChangeNullorDirection(NullorElement element)
        {
            if (element.GetDirection() == "right") 
                element.SetDirection("left");
            else
                element.SetDirection("right");
        }

        public void AddNullator(Node node1, Node node2)
        {
            Nullator nullator = new Nullator(node1, node2, scheme.GetElementsSize());
            scheme.AddElement(nullator);
        }

        public void AddNorator(Node node1, Node node2)
        {
            Norator norator = new Norator(node1, node2, scheme.GetElementsSize());
            scheme.AddElement(norator);
        }

        public void DestroyElement(BaseElement element)
        {
            scheme.RemoveElement(element);
        }
    }
}
