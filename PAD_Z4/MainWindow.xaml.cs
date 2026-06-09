using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PAD_Z4
{
    public partial class MainWindow : Window
    {
        // BUFORY KALKULATORA

        // Lewy operand (np. 5 w działaniu 5 + 3)
        private double _leftOperand = 0;

        // Zapamiętany prawy operand do ponownego "="
        private double _rightOperand = 0;

        // Czy prawy operand został ustawiony
        private bool _rightOperandSet = false;

        // Operator oczekujący (+ - × / x^y)
        private string _pendingOperator = "";

        // Ostatni operator
        private string _lastOperator = "";

        // Czy wpisujemy nową liczbę
        private bool _isNewInput = true;

        // Czy właśnie wykonano obliczenie
        private bool _justCalculated = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        // POBIERANIE AKTUALNEJ LICZBY
        private double CurrentValue
        {
            get
            {
                return double.Parse(
                    DisplayLabel.Text.Replace(',', '.'),
                    CultureInfo.InvariantCulture);
            }
        }

        // WYŚWIETLENIE LICZBY
        private void ShowValue(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                DisplayLabel.Text = "Błąd";
                return;
            }

            DisplayLabel.Text = value.ToString("0.##########")
                                     .Replace('.', ',');
        }

        // CYFRY
        private void BtnDigit_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string digit = button.Content.ToString();

            if (_isNewInput || _justCalculated)
            {

                if (_justCalculated)
                {
                    _pendingOperator = "";
                    _lastOperator = "";
                    _rightOperand = 0;
                    _rightOperandSet = false;
                    ExpressionLabel.Text = "";
                }

                DisplayLabel.Text = digit;
                _isNewInput = false;
                _justCalculated = false;
            }
            else
            {
                if (DisplayLabel.Text == "0")
                {
                    DisplayLabel.Text = digit;
                }
                else
                {
                    DisplayLabel.Text += digit;
                }
            }
        }

        // PRZECINEK
        private void BtnComma_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewInput)
            {
                DisplayLabel.Text = "0,";
                _isNewInput = false;
                return;
            }

            if (!DisplayLabel.Text.Contains(","))
            {
                DisplayLabel.Text += ",";
            }
        }

        // OPERATORY
        private void BtnOperator_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string op = button.Tag.ToString();

            double current = CurrentValue;

            // np. 2 + 3...
            if (!string.IsNullOrEmpty(_pendingOperator)
                && !_isNewInput
                && !_justCalculated)
            {
                _leftOperand = Compute(
                    _leftOperand,
                    _pendingOperator,
                    current);

                ShowValue(_leftOperand);
            }
            else
            {
                _leftOperand = current;
            }

            _pendingOperator = op;
            _lastOperator = op;

            ExpressionLabel.Text =
                $"{_leftOperand} {op}";

            _isNewInput = true;
            _justCalculated = false;
        }

        //  = 
        private void BtnEquals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_pendingOperator))
                return;

            double left;
            double right;

            // Kolejne "="
            if (_justCalculated)
            {
                left = CurrentValue;
                right = _rightOperand;
            }
            else
            {
                left = _leftOperand;
                right = CurrentValue;

                _rightOperand = right;
                _rightOperandSet = true;
            }

            double result =
                Compute(left,
                        _pendingOperator,
                        right);

            ShowValue(result);

            ExpressionLabel.Text =
                $"{left} {_pendingOperator} {right} =";

            _leftOperand = result;
            _lastOperator = _pendingOperator;

            _justCalculated = true;
            _isNewInput = true;
        }

        // PROCENT
        private void BtnPercent_Click(object sender, RoutedEventArgs e)
        {
            double current = CurrentValue;

            if (_pendingOperator == "+"
                || _pendingOperator == "-")
            {
                current =
                    _leftOperand * current / 100.0;
            }
            else
            {
                current /= 100.0;
            }

            ShowValue(current);
        }

        // +/-
        private void BtnPlusMinus_Click(object sender, RoutedEventArgs e)
        {
            double current = CurrentValue;
            current *= -1;

            ShowValue(current);
        }

        // pierwiastek
        private void BtnSqrt_Click(object sender, RoutedEventArgs e)
        {
            double current = CurrentValue;

            if (current < 0)
            {
                DisplayLabel.Text = "Błąd";
                return;
            }

            ShowValue(Math.Sqrt(current));
        }

        // x^2
        private void BtnSquare_Click(object sender, RoutedEventArgs e)
        {
            double current = CurrentValue;

            ShowValue(Math.Pow(current, 2));
        }

        // 1/x
        private void BtnInverse_Click(object sender, RoutedEventArgs e)
        {
            double current = CurrentValue;

            if (current == 0)
            {
                DisplayLabel.Text = "Błąd";
                return;
            }

            ShowValue(1 / current);
        }

        // C
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _leftOperand = 0;
            _rightOperand = 0;
            _rightOperandSet = false;

            _pendingOperator = "";
            _lastOperator = "";

            _isNewInput = true;
            _justCalculated = false;

            DisplayLabel.Text = "0";
            ExpressionLabel.Text = "";
        }

        // CE
        private void BtnClearEntry_Click(object sender, RoutedEventArgs e)
        {
            DisplayLabel.Text = "0";
            _isNewInput = true;
        }

        // OBLICZENIA
        private double Compute(
            double left,
            string op,
            double right)
        {
            switch (op)
            {
                case "+":
                    return left + right;

                case "-":
                    return left - right;

                case "×":
                    return left * right;

                case "÷":
                    if (right == 0)
                        return double.NaN;

                    return left / right;

                case "x^y":
                    return Math.Pow(left, right);

                default:
                    return right;
            }
        }
    }
}
