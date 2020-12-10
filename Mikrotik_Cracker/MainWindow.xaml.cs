using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using tik4net;
using tik4net.Objects.User;
using System.IO;
using System.Threading;
using System.Net;

namespace Mikrotik_Cracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Variables
        public int user_FileList_Lenght { get; set; }
        public int pass_FileList_Lenght { get; set; }


        public string[] user_FileList;
        public string[] password_FileList;

        public string User_path { get; set; }
        public string Pass_path { get; set; }

        public string User_file_name { get; set; }
        public string Pass_file_name { get; set; }

        public string Ip_Connect { get; set; }
        public string User_Connect { get; set; }
        public string Pass_Connect { get; set; }

        public string MikrotikName { get; set; }


        //MainCode to connect Mikrotik
        public int MainCode(string input_IP, string input_USER, string input_PASS)
        {
            using (ITikConnection connection = ConnectionFactory.CreateConnection(TikConnectionType.Api_v2)) // Use TikConnectionType.Api for mikrotikversion prior v6.45
            {
                try
                {

                    connection.Open(input_IP, input_USER, input_PASS);


                    //get Identity
                    ITikCommand cmd = connection.CreateCommand("/system/identity/print");
                    var identity = cmd.ExecuteScalar();
                    MikrotikName = identity;


                    return 0;
                }
                catch (System.Net.Sockets.SocketException)
                {
                    return 1;
                }
                catch (tik4net.TikCommandTrapException)
                {
                    return 2;
                }

            }
        }


        //get and run Mikrtoik Commands
        public void GetCommands(string command)
        {

            using (ITikConnection connection = ConnectionFactory.CreateConnection(TikConnectionType.Api_v2)) // Use TikConnectionType.Api for mikrotikversion prior v6.45
            {
                try
                {

                    connection.Open(Ip_Connect, User_Connect, Pass_Connect);


                    string[] commands = new string[] { command };
                    IEnumerable<ITikSentence> result = connection.CallCommandSync(command);

                    foreach (ITikSentence sentence in result)
                    {
                        if (sentence is ITikTrapSentence)
                            textbox_Main.Text += ("\nSome error occurs: {0}", ((ITikTrapSentence)sentence).Message);
                        else if (sentence is ITikDoneSentence)
                            textbox_Main.Text += ("\n... DONE ..."); // last sentence
                        else if (sentence is ITikReSentence)
                        {
                            foreach (var wordPair in sentence.Words)
                            {
                                textbox_Main.Text += string.Format("\n  {0}={1}", wordPair.Key, wordPair.Value);
                            }
                        }
                        else
                            throw new NotImplementedException("\nUnknown sentence type");
                    }

                }
                catch
                {

                }


            }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textbox_Main.Foreground = Brushes.Black;
            textbox_Main.Text += "\t\t\t\t\tTerminal\n";

            txt_commands.IsEnabled = false;


            LoadingBar.IsRunning = false;
        }


        //clear discription Text
        private void txt_ipaddress_MouseEnter(object sender, MouseEventArgs e)
        {
            txt_ipaddress.Text = checking(txt_ipaddress.Text, "IP Address");
        }

        private void txt_ipaddress_MouseLeave(object sender, MouseEventArgs e)
        {
            txt_ipaddress.Text = checking(txt_ipaddress.Text, "IP Address");
        }

        private void txt_username_MouseEnter(object sender, MouseEventArgs e)
        {
            txt_username.Text = checking(txt_username.Text, "UserName");
        }

        private void txt_username_MouseLeave(object sender, MouseEventArgs e)
        {
            txt_username.Text = checking(txt_username.Text, "UserName");
        }

        private void txt_password_MouseEnter(object sender, MouseEventArgs e)
        {
            txt_password.Text = checking(txt_password.Text, "Password");
        }

        private void txt_password_MouseLeave(object sender, MouseEventArgs e)
        {
            txt_password.Text = checking(txt_password.Text, "Password");
        }

        private void txt_commands_MouseEnter(object sender, MouseEventArgs e)
        {
            txt_commands.Text = checking(txt_commands.Text, "Type and Enter ...");
        }

        private void txt_commands_MouseLeave(object sender, MouseEventArgs e)
        {
            txt_commands.Text = checking(txt_commands.Text, "Type and Enter ...");
        }
        //


        //exit system
        private void btn_Exit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Environment.Exit(0);
        }


        //check radiobutton situation

        private void radiobutton_singleuser_Checked(object sender, RoutedEventArgs e)
        {
            btn_userlist.IsEnabled = false;
            txt_username.IsEnabled = true;
            txt_username.Text = "UserName";
        }

        private void radiobutton_multiuser_Checked(object sender, RoutedEventArgs e)
        {
            btn_userlist.IsEnabled = true;
            txt_username.IsEnabled = false;
            txt_username.Text = "choose a file of Usernames";
        }

        private void radiobutton_singlepass_Checked(object sender, RoutedEventArgs e)
        {
            btn_passlist.IsEnabled = false;
            txt_password.IsEnabled = true;
            txt_password.Text = "Password";

        }

        private void radiobutton_multipass_Checked(object sender, RoutedEventArgs e)
        {
            btn_passlist.IsEnabled = true;
            txt_password.IsEnabled = false;
            txt_password.Text = "choose a file of Passwords";
        }
        //

        //start processing
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {

            txt_commands.IsEnabled = false;

            int doneOrFaile = 0;

            textbox_Main.Clear();
            textbox_Main.Text += "\t\t\t\t\tTerminal\n";
            textbox_Main.Text += "Start Processing ...\n";

            int Counter = 0;

            if (radiobutton_singleuser.IsChecked == true)
            {
                user_FileList = new string[1] { txt_username.Text.Trim() };
            }

            if (radiobutton_singlepass.IsChecked == true)
            {
                password_FileList = new string[1] { txt_password.Text.Trim() };
            }

            LoadingBar.IsRunning = true;



            //
            try
            {
                foreach (String user in user_FileList)
                {
                    foreach (String pass in password_FileList)
                    {
                        Counter++;
                        switch (MainCode(txt_ipaddress.Text.Trim(), user, pass))
                        {
                            case 0:
                                {
                                    textbox_Main.Text += "\n[+] Successfuly Connected to " + txt_ipaddress.Text;
                                    string listbox_item = txt_ipaddress.Text.Trim() + "\n -Username: " + user + "\n -Password: " + pass + "\n------------------------------";
                                    listbox_main.Items.Add(listbox_item);
                                    doneOrFaile = 1;
                                    return;
                                }

                            case 1:
                                {
                                    textbox_Main.Text += "\n[-] Cant Connect to " + txt_ipaddress.Text.Trim();
                                    doneOrFaile = 0;
                                    return;
                                }

                            case 2:
                                {
                                    textbox_Main.Text += string.Format("\n{0}. trying: {1} , {2}", Counter, user, pass);
                                    doneOrFaile = 0;
                                }
                                break;

                        }

                    }
                }

            }

            catch (Exception E)
            {
                textbox_Main.Text += "Error: " + E;
            }
            finally
            {
                if (doneOrFaile == 0)
                {
                    textbox_Main.Text += "\n-- Cracking was not Successfuly --";
                }
                textbox_Main.Text += "\n\n<-- Finished -->";

                LoadingBar.IsRunning = false;
            }
            }
            
        


        //Save File into list
        private void SavetoList(string path, string[] list)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                int index = 0;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list[index] = line;
                    index++;
                }
            }
        }



        //choose file dialog

        private void btn_userlist_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                User_path = openFileDialog.FileName;
                User_file_name = openFileDialog.SafeFileName;
                txt_username.Text = User_file_name;
                user_FileList_Lenght = System.IO.File.ReadAllLines(User_path).Length;
                user_FileList = new string[user_FileList_Lenght];
                SavetoList(User_path, user_FileList);
            }
        }

        private void btn_passlist_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                Pass_path = openFileDialog.FileName;
                Pass_file_name = openFileDialog.SafeFileName;
                txt_password.Text = Pass_file_name;
                pass_FileList_Lenght = System.IO.File.ReadAllLines(Pass_path).Length;
                password_FileList = new string[pass_FileList_Lenght];
                SavetoList(Pass_path, password_FileList);
            }
        }


        //save listbox Results to Desktop
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            string sPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (File.Exists(sPath + "\\MikrotikCracker.txt"))
            {
                File.Delete(sPath + "\\MikrotikCracker.txt");
            }

            FileStream fs = new FileStream(sPath + "\\MikrotikCracker.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write("");
            foreach (var item in listbox_main.Items)
            {
                sw.WriteLine(item);
            }
            sw.Close();

            textbox_Main.Text += "\n\nSuccessfuly Saved to Your Desktop\n";

        }

        //send commands
        private void btn_commands_Click(object sender, RoutedEventArgs e)
        {
            Login_Form lgn = new Login_Form();
            lgn.ShowDialog();

            Ip_Connect = lgn.txt_ip.Text;
            User_Connect = lgn.txt_user.Text;
            Pass_Connect = lgn.txt_pass.Text;




            textbox_Main.Clear();
            textbox_Main.Text += "\t\t\t\t\tTerminal\n";
            textbox_Main.Text += "\nWait...";


            try
            {
                switch (MainCode(Ip_Connect, User_Connect, Pass_Connect))
                {
                    case 0:
                        {
                            textbox_Main.Text += "\n[+] Successfuly Connected to " + Ip_Connect;
                        }
                        break;

                    case 1:
                        {
                            textbox_Main.Text += "\n[-] Cant Connect to " + Ip_Connect;
                            return;
                        }

                    case 2:
                        {
                            textbox_Main.Text += "\nMaybe UserName Or Password is Wrong.";
                            return;
                        }
                        

                }

                txt_commands.IsEnabled = true;

                textbox_Main.Text += ("\n\nHelp");
                textbox_Main.Text += ("\nPlease Type Commands with no spaces Like This: ");
                textbox_Main.Text += ("\n/interface/print");
                textbox_Main.Text += ("\n\n" + MikrotikName + "> Type Commands!");



            }
            catch (System.ArgumentNullException)
            {
                textbox_Main.Text += ("\n" + "Maybe one of the Values is null");
            }





        }



        private void txt_commands_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                GetCommands(txt_commands.Text.Trim());
            }
        }
    }
}


    

