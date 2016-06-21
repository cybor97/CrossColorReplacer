using System.Windows;

namespace CrossColorReplacer
{
    public partial class SetSensivityWindow : Window
    {
        public double Alpha, Red, Green, Blue;
        public SetSensivityWindow(double a, double r, double g, double b)
        {
            InitializeComponent();
            AlphaSlider.Value = a;
            RedSlider.Value = r;
            GreenSlider.Value = g;
            BlueSlider.Value = b;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Alpha = AlphaSlider.Value;
            Red = RedSlider.Value;
            Green = GreenSlider.Value;
            Blue = BlueSlider.Value;
            DialogResult = true;
        }
    }
}
