using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace COMportScanner_v8._1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ScanComPorts_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем список перед новым сканированием
            ComPortsList.Items.Clear();

            // Получаем доступные COM порты
            string[] ports = SerialPort.GetPortNames();

            // Добавляем порты в ListBox
            if (ports.Length > 0)
            {
                foreach (string port in ports)
                {
                    ComPortsList.Items.Add(port);
                }
            }
            else
            {
                ComPortsList.Items.Add("COM порты не найдены");
            }
        }

        private async void ComPortsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ComPortsList.SelectedItem == null)
                return;

            string selectedPort = ComPortsList.SelectedItem.ToString();
            if (selectedPort.Contains("COM"))
            {
                try
                {
                    // Открываем выбранный COM порт
                    _serialPort = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
                    _serialPort.ReadTimeout = 2000; // Таймаут чтения 2 секунды
                    _serialPort.Open();

                    // Отправляем строку "hello"
                    _serialPort.WriteLine("hello");
                    MessageBox.Show($"Отправлено 'hello' на {selectedPort}");

                    // Ждем ответа
                    string response = await ReadFromPortAsync();

                    // Проверяем, получен ли ответ
                    if (response != null)
                    {
                        MessageBox.Show($"Ответ от {selectedPort}: {response}");
                    }
                    else
                    {
                        MessageBox.Show($"Ответ не получен в течение 2 секунд.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка работы с портом: {ex.Message}");
                }
                finally
                {
                    // Закрываем порт после завершения работы
                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        _serialPort.Close();
                        MessageBox.Show($"Порт {selectedPort} закрыт.");
                    }
                }
            }
        }

        private Task<string> ReadFromPortAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    // Попытка прочитать строку из порта
                    return _serialPort.ReadLine();
                }
                catch (TimeoutException)
                {
                    // Возвращаем null, если время ожидания истекло
                    return null;
                }
                catch (Exception)
                {
                    // В случае любой другой ошибки
                    return null;
                }
            });
        }
    }
}
