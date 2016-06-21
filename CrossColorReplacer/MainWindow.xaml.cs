using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using CutoutPro.Winforms;

namespace CrossColorReplacer
{
    public partial class MainWindow : Window
    {
        public double AlphaSensivity, RedSensivity, GreenSensivity, BlueSensivity;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FirstColorButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new ArgbColorDialog())
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Engine.SourceColor = dialog.Color;
                    FirstColorButton.Background = GetBrush(Engine.SourceColor);
                }
        }

        private void SecondColorButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new ArgbColorDialog())
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Engine.TargetColor = dialog.Color;
                    SecondColorButton.Background = GetBrush(Engine.TargetColor);
                }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new OpenFileDialog { Filter = "Images|*.jpg;*.bmp;*.png" })
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Engine.SourceBitmap = new Bitmap(dialog.FileName);
                    Engine.TargetBitmap = new Bitmap(Engine.SourceBitmap);
                    CurrentImage.Source = Engine.TargetBitmap.ToBitmapSource();
                }
        }
        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "PNG|*.png|JPEG|*.jpg" })
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    switch (dialog.FilterIndex)
                    {
                        case 0:
                            Engine.TargetBitmap.Save(dialog.FileName, ImageFormat.Png);
                            break;
                        default:
                            Engine.TargetBitmap.Save(dialog.FileName, ImageFormat.Jpeg);
                            break;
                    }
        }


        private void PushButton_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => Engine.ReColor((int)((AlphaSensivity / 100) * 255),
                (int)((RedSensivity / 100) * 255),
                (int)((GreenSensivity / 100) * 255),
                (int)((BlueSensivity / 100) * 255))).Start();
            new Thread(DisplayProcess).Start();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        void DisplayProcess()
        {
            while (!Engine.Completed)
                Dispatcher.Invoke((Action)(() =>
                {
                    lock (Engine.TargetBitmap)
                        CurrentImage.Source = Engine.TargetBitmap.ToBitmapSource();
                    Thread.Sleep(100);
                    PushButton.IsEnabled = Engine.Completed;
                }), DispatcherPriority.Background);
            Dispatcher.Invoke((Action)(() =>
            {
                lock (Engine.TargetBitmap)
                    CurrentImage.Source = Engine.TargetBitmap.ToBitmapSource();
            }));
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Engine.TargetBitmap = new Bitmap(Engine.SourceBitmap);
            CurrentImage.Source = Engine.SourceBitmap.ToBitmapSource();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void SetSensivityButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SetSensivityWindow(AlphaSensivity, RedSensivity, GreenSensivity, BlueSensivity);
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                AlphaSensivity = dialog.Alpha;
                RedSensivity = dialog.Red;
                GreenSensivity = dialog.Green;
                BlueSensivity = dialog.Blue;
            }
        }

        private void SensivitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AlphaSensivity = RedSensivity = GreenSensivity = BlueSensivity = SensivitySlider.Value;
        }

        System.Windows.Media.SolidColorBrush GetBrush(Color source)
        {
            return new System.Windows.Media.SolidColorBrush(ConvertColor(source));
        }
        System.Windows.Media.Color ConvertColor(Color source)
        {
            return System.Windows.Media.Color.FromArgb(source.A, source.R, source.G, source.B);
        }
    }
}
