using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class Form1 : Form
    {
        private readonly ApiClient _apiClient;
        private DataGridView _dataGridViewTeams;

        private CheckBox _chkUseSql;
        private TextBox _txtSearch;
        private TextBox _txtTeamName;

        private readonly Guid _cs2GameId = Guid.Parse("11111111-1111-1111-1111-000000000001");

        public Form1(ApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Управление киберспортивным клубом";
            this.Size = new Size(600, 480);
            this.StartPosition = FormStartPosition.CenterScreen;

            var btnLoad = new Button { Text = "Сбросить / Загрузить", Location = new Point(20, 20), Size = new Size(150, 30) };
            btnLoad.Click += (s, e) => { _txtSearch.Clear(); LoadData(); };
            this.Controls.Add(btnLoad);

            _txtSearch = new TextBox { Location = new Point(180, 25), Size = new Size(150, 25) };
            this.Controls.Add(_txtSearch);

            var btnSearch = new Button { Text = "Поиск", Location = new Point(340, 20), Size = new Size(70, 30) };
            btnSearch.Click += (s, e) => LoadData();
            this.Controls.Add(btnSearch);

            _chkUseSql = new CheckBox { Text = "Использовать чистый SQL", Location = new Point(420, 25), Size = new Size(160, 25) };
            this.Controls.Add(_chkUseSql);

            _txtTeamName = new TextBox { Location = new Point(20, 75), Size = new Size(150, 25) };
            this.Controls.Add(_txtTeamName);

            var btnAdd = new Button { Text = "Добавить", Location = new Point(180, 70), Size = new Size(90, 35) };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            var btnUpdate = new Button { Text = "Изменить (по ID)", Location = new Point(280, 70), Size = new Size(120, 35) };
            btnUpdate.Click += BtnUpdate_Click;
            this.Controls.Add(btnUpdate);

            var btnDelete = new Button { Text = "Удалить", Location = new Point(410, 70), Size = new Size(90, 35), BackColor = Color.LightCoral };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            _dataGridViewTeams = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(540, 300),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false
            };
            this.Controls.Add(_dataGridViewTeams);
        }

        private async void LoadData()
        {
            var teams = await _apiClient.GetTeamsAsync(_txtSearch.Text, _chkUseSql.Checked);
            _dataGridViewTeams.DataSource = teams;
            if (_dataGridViewTeams.Columns["Id"] != null) _dataGridViewTeams.Columns["Id"].Visible = false;
            if (_dataGridViewTeams.Columns["Name"] != null) _dataGridViewTeams.Columns["Name"].HeaderText = "Название команды";
            if (_dataGridViewTeams.Columns["PlayersCount"] != null) _dataGridViewTeams.Columns["PlayersCount"].HeaderText = "Кол-во игроков";
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtTeamName.Text)) return;
            if (await _apiClient.CreateTeamAsync(_txtTeamName.Text, _cs2GameId, _chkUseSql.Checked))
            {
                _txtTeamName.Clear(); LoadData();
            }
        }

        private async void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_dataGridViewTeams.SelectedRows.Count == 0 || string.IsNullOrWhiteSpace(_txtTeamName.Text)) return;
            var id = (Guid)_dataGridViewTeams.SelectedRows[0].Cells["Id"].Value;

            if (await _apiClient.UpdateTeamAsync(id, _txtTeamName.Text, _cs2GameId, _chkUseSql.Checked))
            {
                _txtTeamName.Clear(); LoadData();
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_dataGridViewTeams.SelectedRows.Count == 0) return;
            var id = (Guid)_dataGridViewTeams.SelectedRows[0].Cells["Id"].Value;

            if (await _apiClient.DeleteTeamAsync(id, _chkUseSql.Checked))
            {
                LoadData();
            }
        }
    }
}