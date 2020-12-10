using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;


namespace Mikrotik_Cracker
{
    /// <summary>
    /// Interaction logic for Login_Form.xaml
    /// </summary>
    public partial class Login_Form : Window
    {
        public Login_Form()
        {
            InitializeComponent();
        }


        //close this window
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //discover text function
        public static string checking(string textbox, string content)
        {
            if (textbox == content)
            {
                return textbox = "";
            }

            if (textbox == "")
            {
                return textbox = content;
            }

            return textbox;
        }

        private void txt_ip_MouseEnter(object sender, MouseEventArgs e)
        {
            txt_ip.Text = checking(txt_ip.Text, "Type IP Here");
        }

        private void txt_ip_MouseLeave(object sender, MouseEventArgs e)
        {
            txt_ip.Text = checking(txt_ip.Text, "Type IP Here");
        }

        private void txt_user_MouseEnter(object sender, MouseEventArgs e)
        {
            txt_user.Text = checking(txt_user.Text, "UserName");
        }

        private void txt_user_MouseLeave(object sender, MouseEventArgs e)
        {
            txt_user.Text = checking(txt_user.Text, "UserName");
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (txt_ip.Text == "" || txt_ip.Text == "Type IP Here" || txt_ip.Text == " ")
            {
                MessageBox.Show("Please type IP", "Error",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                this.Close();
            }
            

        }
    }
}
