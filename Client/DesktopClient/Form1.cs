using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _apiClient;
        private DataGridView _dataGridViewTeams;
        private Button _btnLoadTeams;

        public Form1()
        {
            InitializeComponent();
            _apiClient = new ApiClient();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Управление киберспортивным клубом";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            _btnLoadTeams = new Button
            {
                Text = "Загрузить команды",
                Location = new Point(20, 20),
                Size = new Size(150, 40)
            };
            _btnLoadTeams.Click += BtnLoadTeams_Click;
            this.Controls.Add(_btnLoadTeams);

            // Настраиваем таблицу
            _dataGridViewTeams = new DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(540, 250),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            this.Controls.Add(_dataGridViewTeams);
        }

        private async void BtnLoadTeams_Click(object sender, EventArgs e)
        {
            _btnLoadTeams.Enabled = false;
            _btnLoadTeams.Text = "Загрузка...";

            var teams = await _apiClient.GetTeamsAsync();
            
            _dataGridViewTeams.DataSource = teams;

            if (_dataGridViewTeams.Columns["Id"] != null)
                _dataGridViewTeams.Columns["Id"].Visible = false; 
            
            if (_dataGridViewTeams.Columns["Name"] != null)
                _dataGridViewTeams.Columns["Name"].HeaderText = "Название команды";
                
            if (_dataGridViewTeams.Columns["PlayersCount"] != null)
                _dataGridViewTeams.Columns["PlayersCount"].HeaderText = "Кол-во игроков";

            _btnLoadTeams.Enabled = true;
            _btnLoadTeams.Text = "Загрузить команды";
        }
    }
}
