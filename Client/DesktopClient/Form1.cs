using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class Form1 : Form
    {
        private TabControl _tabControl;

        // Элементы вкладки "Команды"
        private DataGridView _gridTeams;
        private TextBox _txtTeamSearch, _txtTeamName;

        // Элементы вкладки "Игроки"
        private DataGridView _gridPlayers;
        private TextBox _txtPlayerSearch, _txtPlayerNick;

        // Общие настройки
        private CheckBox _chkUseSql;
        private readonly Guid _cs2GameId = Guid.Parse("11111111-1111-1111-1111-000000000001");

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            LoadTeamsData();
            LoadPlayersData();
        }

        private void SetupUI()
        {
            this.Text = "Управление киберспортивным клубом (Многотабличный режим)";
            this.Size = new Size(650, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Глобальный переключатель SQL/ORM
            _chkUseSql = new CheckBox { Text = "Использовать чистый SQL для ВСЕХ запросов", Location = new Point(20, 10), Size = new Size(300, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.Controls.Add(_chkUseSql);

            _tabControl = new TabControl { Location = new Point(10, 40), Size = new Size(610, 400) };
            this.Controls.Add(_tabControl);

            // --- ВКЛАДКА 1: КОМАНДЫ ---
            var tabTeams = new TabPage("Команды");
            _txtTeamSearch = new TextBox { Location = new Point(10, 15), Size = new Size(150, 25) };
            var btnTeamSearch = new Button { Text = "Поиск / Сброс", Location = new Point(170, 13), Size = new Size(100, 27) };
            btnTeamSearch.Click += (s, e) => LoadTeamsData();

            _txtTeamName = new TextBox { Location = new Point(10, 50), Size = new Size(150, 25) };
            var btnAddTeam = new Button { Text = "Добавить", Location = new Point(170, 48), Size = new Size(80, 27) };
            btnAddTeam.Click += BtnAddTeam_Click;
            var btnDelTeam = new Button { Text = "Удалить", Location = new Point(260, 48), Size = new Size(80, 27), BackColor = Color.LightCoral };
            btnDelTeam.Click += BtnDeleteTeam_Click;

            _gridTeams = new DataGridView { Location = new Point(10, 90), Size = new Size(580, 270), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false };

            tabTeams.Controls.AddRange(new Control[] { _txtTeamSearch, btnTeamSearch, _txtTeamName, btnAddTeam, btnDelTeam, _gridTeams });
            _tabControl.TabPages.Add(tabTeams);

            // --- ВКЛАДКА 2: ИГРОКИ ---
            var tabPlayers = new TabPage("Игроки");
            _txtPlayerSearch = new TextBox { Location = new Point(10, 15), Size = new Size(150, 25) };
            var btnPlayerSearch = new Button { Text = "Поиск / Сброс", Location = new Point(170, 13), Size = new Size(100, 27) };
            btnPlayerSearch.Click += (s, e) => LoadPlayersData();

            _txtPlayerNick = new TextBox { Location = new Point(10, 50), Size = new Size(150, 25) };
            var btnAddPlayer = new Button { Text = "Добавить", Location = new Point(170, 48), Size = new Size(80, 27) };
            btnAddPlayer.Click += BtnAddPlayer_Click;
            var btnDelPlayer = new Button { Text = "Удалить", Location = new Point(260, 48), Size = new Size(80, 27), BackColor = Color.LightCoral };
            btnDelPlayer.Click += BtnDeletePlayer_Click;

            _gridPlayers = new DataGridView { Location = new Point(10, 90), Size = new Size(580, 270), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false };

            tabPlayers.Controls.AddRange(new Control[] { _txtPlayerSearch, btnPlayerSearch, _txtPlayerNick, btnAddPlayer, btnDelPlayer, _gridPlayers });
            _tabControl.TabPages.Add(tabPlayers);
        }

        // --- ЛОГИКА ДЛЯ КОМАНД ---
        private async void LoadTeamsData()
        {
            _gridTeams.DataSource = await ApiClient.Instance.GetTeamsAsync(_txtTeamSearch.Text, _chkUseSql.Checked);
            if (_gridTeams.Columns["Id"] != null) _gridTeams.Columns["Id"].Visible = false;
        }

        private async void BtnAddTeam_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtTeamName.Text)) return;
            if (await ApiClient.Instance.CreateTeamAsync(_txtTeamName.Text, _cs2GameId, _chkUseSql.Checked)) { _txtTeamName.Clear(); LoadTeamsData(); }
        }

        private async void BtnDeleteTeam_Click(object sender, EventArgs e)
        {
            if (_gridTeams.SelectedRows.Count == 0) return;
            if (await ApiClient.Instance.DeleteTeamAsync((Guid)_gridTeams.SelectedRows[0].Cells["Id"].Value, _chkUseSql.Checked)) LoadTeamsData();
        }

        // --- ЛОГИКА ДЛЯ ИГРОКОВ ---
        private async void LoadPlayersData()
        {
            _gridPlayers.DataSource = await ApiClient.Instance.GetPlayersAsync(_txtPlayerSearch.Text, _chkUseSql.Checked);
            if (_gridPlayers.Columns["Id"] != null) _gridPlayers.Columns["Id"].Visible = false;
        }

        private async void BtnAddPlayer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtPlayerNick.Text)) return;
            if (await ApiClient.Instance.CreatePlayerAsync(_txtPlayerNick.Text, "Имя", "Фамилия", _chkUseSql.Checked)) { _txtPlayerNick.Clear(); LoadPlayersData(); }
        }

        private async void BtnDeletePlayer_Click(object sender, EventArgs e)
        {
            if (_gridPlayers.SelectedRows.Count == 0) return;
            if (await ApiClient.Instance.DeletePlayerAsync((Guid)_gridPlayers.SelectedRows[0].Cells["Id"].Value, _chkUseSql.Checked)) LoadPlayersData();
        }
    }
}