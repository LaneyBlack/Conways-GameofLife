using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Conways_Game_of_Life
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static GameField _gameField;

        public MainWindow()
        {
            InitializeComponent();
            var fieldWidth = 50; // Start value
            FieldWidthTextBox.Text = fieldWidth + "";
            var fieldHeight = (int)(fieldWidth / Width * Height); // Count 
            FieldGridInit(fieldWidth, fieldHeight);
            _gameField = new GameField(fieldWidth, fieldHeight);
        }

        private static void PointClick(object sender, RoutedEventArgs e)
        {
            Button pointButton = (Button)sender;
            pointButton.Opacity = pointButton.Opacity==100 ? 0 : 100;
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
        }

        private void Play(object sender, RoutedEventArgs e)
        {
        }

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        private void FieldWidthChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int fieldWidth;
            if (Regex.Replace(textBox.Text.Trim(), "[^.0-9]", "") == "" ||
                int.Parse(textBox.Text) <= 0)
                fieldWidth = 1;
            else
                fieldWidth = int.Parse(textBox.Text);
            var fieldHeight = (int)(fieldWidth / Width * Height);
            _gameField = new GameField(fieldWidth, fieldHeight);
            FieldGridInit(fieldWidth, fieldHeight);
        }

        private void FieldGridInit(int fieldWidth, int fieldHeight)
        {
            // Clear Grid
            FieldGrid.Children.Clear();
            FieldGrid.RowDefinitions.Clear();
            FieldGrid.ColumnDefinitions.Clear();
            // Define dimensions
            for (var i = 0; i < fieldWidth; i++)
                FieldGrid.ColumnDefinitions.Add(new ColumnDefinition());
            // Fill Grid
            for (var i = 0; i < fieldHeight; i++)
            {
                FieldGrid.RowDefinitions.Add(new RowDefinition());
                for (var j = 0; j < fieldWidth; j++)
                {
                    var button = new Button
                    {
                        Uid = "P" + i + "-" + j,
                        Opacity = 0,
                    };
                    var border = new Border
                    {
                        Child = button,
                    };
                    button.Click += PointClick;
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    FieldGrid.Children.Add(border);
                }
            }
        }
    }
}