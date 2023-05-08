using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Conways_Game_of_Life
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static GameField _gameField;
        private static Thread _gameFieldThread;

        public MainWindow()
        {
            InitializeComponent();
            const int fieldWidth = 50; // Start value
            FieldWidthTextBox.Text = fieldWidth + "";
            var fieldHeight = (int)(fieldWidth / Width * Height); // Count 
            FieldGridInit(fieldWidth, fieldHeight);
            _gameField = new GameField(fieldWidth, fieldHeight);
            _gameFieldThread = new Thread(delegate() { _gameField.LoopCalculations(); });
            _gameFieldThread.Start();
            _gameField.RedrawEvent += FieldGridRedrawEvent;
        }

        private static void PointClick(object sender, RoutedEventArgs e)
        {
            if (!_gameField.IsPaused) return;
            Button pointButton = (Button)sender;
            if (pointButton.Opacity == 100)
            {
                pointButton.Opacity = 0;
                //ToDo Without Uid
                // Grid.GetRow(pointButton)
                var tmp = pointButton.Uid.Replace("P", "").Split('-');
                int i = int.Parse(tmp[0]);
                int j = int.Parse(tmp[1]);
                _gameField.Field[i, j] = 0;
            }
            else
            {
                pointButton.Opacity = 100;
                var tmp = pointButton.Uid.Replace("P", "").Split('-');
                int i = int.Parse(tmp[0]);
                int j = int.Parse(tmp[1]);
                _gameField.Field[i, j] = 1;
            }
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            lock (_gameField)
                _gameField.IsPaused = true;
            FieldWidthTextBox.IsEnabled = true;
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            FieldWidthTextBox.IsEnabled = false;
            lock (_gameField)
                _gameField.IsPaused = false;
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
            if (_gameField != null)
                _gameField.Field = new int[fieldWidth, fieldHeight];
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

        private void FieldGridRedraw()
        {
            for (int y = 0; y < _gameField.Field.GetLength(0); y++)
            for (int x = 0; x < _gameField.Field.GetLength(1); x++)
            {
                FieldGrid.Children
                    .Cast<Border>()
                    .First(e => Grid.GetRow(e) == y && Grid.GetColumn(e) == x)
                    .Child.Opacity = _gameField.Field[y, x] * 100;
            }
        }

        private void FieldGridRedrawEvent(object sender, string e)
        {
            Dispatcher.BeginInvoke((Action)FieldGridRedraw);
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _gameFieldThread.Abort();
        }
    }
}