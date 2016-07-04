using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
namespace ClientForSqlServer
{
    public partial class MainForm : Form
    {
        public string getRequestString => textBox1.Text;

        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) => YourRequest();

        private void button2_Click(object sender, EventArgs e)
        {
            GetRandomRequest();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Data.stringValueForRTBGetSet = textBox1.Text;
            QueryForm qf = new QueryForm();
            qf.Owner = this;
            qf.ShowForm();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            const int valueOFX = 572;
            const int valueOFY = 443;
            SizeOfMainForm.valueOfXGetSet = valueOFX;
            SizeOfMainForm.valueOfYGetSet = valueOFY;
            NotifyMessage(10000, "Client4SQL", "Hi! :)", ToolTipIcon.Info);
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e) => VisibleOnOff();

        private void button4_Click(object sender, EventArgs e)
        {
            GetListOfDatabases();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GetTablesFromCurrentDatabase();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) => textBox2.Text = comboBox2.SelectedItem.ToString();

        public void SetValue(string value) => textBox1.Text = value;

        // Метод, формирующий и возвращающий строку подключения
        private string StringConnection()
        {
            SqlConnectionStringBuilder connectsb = new SqlConnectionStringBuilder();

            // Тут ConnectTimeout
            connectsb.ConnectTimeout = (int)numericUpDown1.Value;
            connectsb.InitialCatalog = textBox2.Text;
            connectsb.DataSource = textBox3.Text;
            // Виндовая аутентификация или серверная
            if (checkBox1.Checked) connectsb.IntegratedSecurity = true;
            else connectsb.IntegratedSecurity = false;
            connectsb.UserID = textBox4.Text;
            connectsb.Password = textBox5.Text;

            return connectsb.ConnectionString;
        }

        // Метод, который делает запрос на сервер с поля Request
        private void YourRequest()
        {
            // Лочим кнопки
            DisabledButton();

            Task.Factory.StartNew(() =>
            {
                using (SqlConnection sc = new SqlConnection())
                {
                    sc.ConnectionString = StringConnection();

                    try
                    {
                        sc.Open();

                        // Конструктору передаём наш запрос и подключение
                        SqlCommand sqlcmd = new SqlCommand(getRequestString, sc);
                        DataTable tab = new DataTable();
                        BindingSource bs = new BindingSource();

                        sqlcmd.CommandType = CommandType.Text;

                        tab.Load(sqlcmd.ExecuteReader());
                        bs.DataSource = tab.DefaultView;

                        // Из этого потока идём в основнйо и ложим там в dataGrid наш bs
                        if (this.InvokeRequired)
                            BeginInvoke(new MethodInvoker(delegate
                            {
                                dataGridView1.DataSource = bs;
                            }));
                    }
                    catch (SqlException se)
                    {
                        NotifyMessage(10000, "Client4SQL", "Fail :(", ToolTipIcon.Error);
                        MessageBox.Show($"# exception: {se.Number} \n message exception: {se.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (InvalidOperationException ioe)
                    {
                        NotifyMessage(10000, "Client4SQL", "Fail :(", ToolTipIcon.Error);
                        MessageBox.Show($"# exception: {ioe.Data} \n message exception: {ioe.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // После выполнения Task'a врубаем кнопки
            }).ContinueWith(next =>
            {
                EnabledButton();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Метод, получающий список всех БД на серваке
        private void GetListOfDatabases()
        {
            // Лочим кнопки
            DisabledButton();

            comboBox2.Items.Clear();

            Task.Factory.StartNew(() =>
            {
                using (SqlConnection sc = new SqlConnection())
                {
                    sc.ConnectionString = StringConnection();

                    try
                    {
                        sc.Open();

                        DataTable schemaTable = sc.GetSchema("Databases");

                        // Из этого идём в основной и в dataGrid ложим список БД
                        if (this.InvokeRequired)
                            BeginInvoke(new MethodInvoker(delegate
                            {
                                foreach (DataRow r in schemaTable.Rows)
                                {
                                    comboBox2.Items.Add(r[0]);
                                }
                            }));
                    }
                    catch (SqlException se)
                    {
                        NotifyMessage(10000, "Client4SQL", "Fail :(", ToolTipIcon.Error);
                        MessageBox.Show($"# exception: {se.Number} \n message exception: {se.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (InvalidOperationException ioe)
                    {
                        NotifyMessage(10000, "Client4SQL", "Fail :(", ToolTipIcon.Error);
                        MessageBox.Show($"# exception: {ioe.Data} \n message exception: {ioe.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // После выполнения Task'a врубаем кнопки
            }).ContinueWith(next =>
            {
                EnabledButton();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Метод, 
        private void GetTablesFromCurrentDatabase()
        {
            DisabledButton();

            Task.Factory.StartNew(() =>
            {
                using (SqlConnection sc = new SqlConnection())
                {
                    sc.ConnectionString = StringConnection();

                    try
                    {
                        sc.Open();

                        SqlCommand sqlcmd = new SqlCommand("select * from sys.objects where type in (N'U')", sc);
                        DataTable tab = new DataTable();
                        BindingSource bs = new BindingSource();

                        sqlcmd.CommandType = CommandType.Text;

                        tab.Load(sqlcmd.ExecuteReader());
                        bs.DataSource = tab.DefaultView;

                        if (this.InvokeRequired)
                            BeginInvoke(new MethodInvoker(delegate
                            {
                                dataGridView1.DataSource = bs;
                            }));
                    }
                    catch (SqlException se)
                    {
                        NotifyMessage(10000, "Client4SQL", "Fail :(", ToolTipIcon.Error);
                        MessageBox.Show($"# exception: {se.Number} \n message exception: {se.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (InvalidOperationException ioe)
                    {
                        NotifyMessage(10000, "Client4SQL", "Fail :(", ToolTipIcon.Error);
                        MessageBox.Show($"# exception: {ioe.Data} \n message exception: {ioe.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }).ContinueWith(next =>
            {
                EnabledButton();
                NotifyMessage(10000, "Client4SQL", "Success! :)", ToolTipIcon.Info);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Метод, формирующий запрос на основе случайной таблицы,
        // полученной из текущей БД
        private void GetRandomRequest()
        {
            // Вырубаем анопки
            DisabledButton();

            Task.Factory.StartNew(() =>
            {
                using (SqlConnection sc = new SqlConnection())
                {
                    sc.ConnectionString = StringConnection();

                    try
                    {
                        sc.Open();

                        // Тут - список со всеми таблицами
                        List<string> listOfTablesFromCurrentDatabase = new List<string>();

                        // Делаем запрос на сервак на получение всех таблиц для текущей БД
                        SqlCommand sqlcmd = new SqlCommand("select * from sys.objects where type in (N'U')", sc);
                        DataTable tab = new DataTable();
                        // Закинули полученные таблицы в DataTable
                        tab.Load(sqlcmd.ExecuteReader());

                        if (this.InvokeRequired)
                            BeginInvoke(new MethodInvoker(delegate
                            {
                                try
                                {
                                    // Идём по всем строкам в нашей DataTable 
                                    foreach (DataRow r in tab.Rows)
                                    {
                                        // И кидаем их в наш List
                                        listOfTablesFromCurrentDatabase.Add(r[0].ToString());
                                    }
                                    // В качестве случайной таблицы берём случайное значение 
                                    // из нашего List'a в диапазоне [0;list.count]
                                    Random rand = new Random();
                                    textBox1.Text = $"select * from {listOfTablesFromCurrentDatabase[rand.Next(listOfTablesFromCurrentDatabase.Count)].ToString()}";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"# exception: {ex.Data} \n message exception: {ex.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }));
                    }
                    catch (SqlException se)
                    {
                        NotifyMessage(10000, "Client4SQL", "Fail :(", ToolTipIcon.Error);
                        MessageBox.Show($"# exception: {se.Number} \n message exception: {se.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (InvalidOperationException ioe)
                    {
                        NotifyMessage(10000, "Client4SQL", "Fail :(", ToolTipIcon.Error);
                        MessageBox.Show($"# exception: {ioe.Data} \n message exception: {ioe.Message}", "SimpleClient", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                // После завершения Task'a врубаем кнопки
            }).ContinueWith(next =>
            {
                EnabledButton();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        // Метод сворачивающий/разворачивающий dataGrid
        private void VisibleOnOff()
        {
            // Если dataGrid "свёрнута"
            if (dataGridView1.Dock == DockStyle.Top)
            {
                // то проходимся по всем контролам на форме кроме 
                // dataGrid и скрываем их
                foreach (Control element in Controls)
                {
                    if (!(element is DataGridView))
                        element.Visible = false;
                }
                // Разворачиваем dataGrid на всю форму
                dataGridView1.Dock = DockStyle.Fill;
                // Разрешаем менять размер
                this.FormBorderStyle = FormBorderStyle.Sizable;
                // И разворачивать кнопкой на весь экран
                this.MaximizeBox = true;
            }
            // Если форму "развёрнута" и её нужно "свернуть"
            else
            {
                // Даём размеры окна по умолчанию
                this.WindowState = FormWindowState.Normal;
                // Размер берём из свойств, которые содержат инфу о нормальном размере окна
                this.Size = new Size(SizeOfMainForm.valueOfXGetSet, SizeOfMainForm.valueOfYGetSet);
                // Запрещаем разворачивать кнопкой на весь экран
                this.MaximizeBox = false;
                // Прилепляем dataGrid наверх
                dataGridView1.Dock = DockStyle.Top;
                // Восстанавливаем видимост всех контролов
                foreach (Control element in Controls)
                {
                    element.Visible = true;
                }
                // Запрещаем изменять размер формы
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
            }
        }

        // Метод, "отключающий" кнопки
        private void DisabledButton()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
        }

        // Метод, "включающий" кнопки
        private void EnabledButton()
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        // Метод, релизующий вывод notify
        private void NotifyMessage(int timing, string title, string message, ToolTipIcon icon)
        {
            notifyIcon1.Icon = SystemIcons.Exclamation;
            // Если есть активные notify, то более актуальынй notify
            // Перекрываем старый
            notifyIcon1.Visible = false;
            // И отображается сам
            notifyIcon1.Visible = true;
            // иницилизируем параметры notify
            notifyIcon1.ShowBalloonTip(timing, title, message, icon);
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Отменяем событие ошибки
            e.Cancel = true;
        }
    }
}
