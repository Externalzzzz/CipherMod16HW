using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CipherMod16
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EncryptedText.IsReadOnly = true;
            DecryptedText.IsReadOnly = true;
        }


        //static string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя\"\'\\/|[]{}!@#$%^&*()1234567890-=_+,. ";


        private Dictionary <char, int> SetAlphabet(string alphabet)
        {
            Dictionary<char, int> DictAlphabet = new Dictionary<char, int>();

            for (int i = 0; i < alphabet.Length; i++)
            {
                DictAlphabet[alphabet[i]] = i; //временно
            }

            return DictAlphabet;
        }
        private string XOR (string messageChar, string keyChar)
        {
            string result = string.Empty;
            for (int i = 0; i < messageChar.Length;i++)
            {
                char mchar = messageChar[i];
                char kchar = keyChar[i];
                if (mchar == kchar && (mchar == '1' || mchar == '0'))
                    result += '0';
                else if (mchar != kchar && (kchar == '1' || mchar == '1'))
                    result += '1';

            }
            Console.WriteLine($"Результат XOR - {result}");
            return result;
        }
        private string StringToHex(string myString)
        {
            byte[] bytes = Encoding.GetEncoding(1251).GetBytes(myString);
            return string.Concat(bytes.Select(b =>  b.ToString("X2")));
        }
        private string HextoBin(string temp) => Convert.ToString(Convert.ToInt32(temp, 16), 2);

        private string CheckBin(string temp)
        {
            while (temp.Length < 8)
                temp = temp.Insert(0, "0");
            return temp;
        }
        private void BeginEncryption(object sender, RoutedEventArgs e)
        {
            string message = MessageText.Text;
            string key = KeyText.Text;

            if (message.Length != key.Length)
            {
                MessageBox.Show("Размеры сообщения и ключа не совпадают", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            message = StringToHex(message);
            key = StringToHex(key);
            string result = string.Empty;
            string temp = string.Empty;
            for (int i = 0; i < message.Length-1; i+=2)
            {
                string messbin = CheckBin(HextoBin(message[i].ToString() + message[i + 1].ToString()));
                string keybin = CheckBin(HextoBin(key[i].ToString() + key[i + 1].ToString()));
                Console.WriteLine($"Взяли  сообщения - {messbin} \nключа - \t\t{keybin}");
                temp = Convert.ToInt32(XOR(messbin, keybin), 2).ToString("X");
                if (temp.Length < 2)
                    temp = temp.Insert(0, "0");
                result += temp;
            }
            //result  = Convert.ToInt32(result, 2).ToString("X");
            //Console.WriteLine(HextoBin(temp));
            //Console.WriteLine(Convert.ToInt32(message[0].ToString(), 16));
            EncryptedText.Text = result;
        }
        private void BeginDecryption(object sender, RoutedEventArgs e)
        {
            string message = MessageText2.Text;
            string key = KeyText2.Text;
            if (message == "" && EncryptedText.Text != message )
            {
                message = EncryptedText.Text;
                MessageText2.Text = message;
            }    
            if (message.Length != key.Length * 2)
            {
                MessageBox.Show("Размеры сообщения и ключа не совпадают", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            key = StringToHex(key);
            Console.WriteLine($"Взяли строки mes - {message}\t key - {key}");


            string messchar = string.Empty;
            string keychar = string.Empty;
            string temp = string.Empty;
            string result = string.Empty;

            List <byte> encoding1251 = new List<byte>();

            for (int i = 0; i < key.Length-1; i+=2)
            {
                messchar = CheckBin(HextoBin(message[i].ToString() + message[i+1].ToString()));
                keychar = CheckBin(HextoBin(key[i].ToString() + key[i+1].ToString()));
                Console.WriteLine($"Взяли  сообщения - {messchar} \nключа - \t\t{keychar}");
                temp = XOR(messchar, keychar);
                encoding1251.Add(Convert.ToByte(temp, 2));


            }

            result = Encoding.GetEncoding(1251).GetString( encoding1251.ToArray());
            DecryptedText.Text = result;

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
           Environment.Exit(0);
        }
        private void Info(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программу создал студент группы ИЦТМС 4-3\nБорисяк Максим Андреевич\nКонтакты:\n\t* m.a.borisyak@gmail.com\n\t* https://github.com/Externalzzzz", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void SaveResults(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Результаты";
            save.DefaultExt = ".txt";
            save.Filter = "Text documents (.txt)|*.txt"; 
            string File = "";
            if (save.ShowDialog() == true)
            {
                File = save.FileName;
                //save.OpenFile();
            }
            else return;
           
            StreamWriter streamWriter = new StreamWriter(File);

            if (EncryptedText.Text != null)
            {
                streamWriter.WriteLine($"Введенное сообщение: {MessageText.Text}");
                streamWriter.WriteLine($"Введенный ключ: {KeyText.Text}");
                streamWriter.WriteLine($"Зашифрованное сообщение: {EncryptedText.Text}");

            }
            if (DecryptedText.Text != null)
            {
                streamWriter.WriteLine($"Введенное сообщение: {MessageText2.Text}");
                streamWriter.WriteLine($"Введенный ключ: {KeyText2.Text}");
                streamWriter.WriteLine($"Зашифрованное сообщение: {DecryptedText.Text}");

            }
            streamWriter.Close();
        }
        private void SaveEncrypt(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Результаты";
            save.DefaultExt = ".txt";
            save.Filter = "Text documents (.txt)|*.txt";
            string File = "";
            if (save.ShowDialog() == true)
            {
                File = save.FileName;
                //save.OpenFile();
            }
            else return;


            if (EncryptedText.Text != "")
            {
                StreamWriter streamWriter = new StreamWriter(File);
                streamWriter.WriteLine($"Ключ: {KeyText.Text}");
                streamWriter.WriteLine($"Зашифрованное сообщение: {EncryptedText.Text}");
                streamWriter.Close();
            }
            else
            {
                MessageBox.Show("Вы ничего не зашифровали!", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }
        private void SaveDecrypt(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "Результаты";
            save.DefaultExt = ".txt";
            save.Filter = "Text documents (.txt)|*.txt";
            string File = "";
            if (save.ShowDialog() == true)
            {
                File = save.FileName;
                //save.OpenFile();
            }
            else return;


            if (EncryptedText.Text != "")
            {
                StreamWriter streamWriter = new StreamWriter(File);
                streamWriter.WriteLine($"Ключ: {KeyText.Text}");
                streamWriter.WriteLine($"Расшифрованное сообщение: {DecryptedText.Text}");
                streamWriter.Close();
            }
            else
            {
                MessageBox.Show("Вы ничего не расшифровали!", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }
        }
    }
