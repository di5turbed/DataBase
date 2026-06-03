using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class Form1 : Form
    {
        private TabControl _tabControl;
        private CheckBox _chkUseSql;

        // Таблицы
        private DataGridView _gridTeams, _gridPlayers, _gridTournaments, _gridResults;

        // Хардкод ID для упрощения ввода (в реальном приложении выбирается из ComboBox)
        private readonly Guid _cs2GameId = Guid.Parse("11111111-1111-1111-1111-000000000001");
        private readonly Guid _testMatchId = Guid.NewGuid();
        private readonly Guid _testTeamId = Guid.NewGuid();

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            LoadAllData();
        }

        private void SetupUI()
        {
            this.Text = "Киберспортивный клуб (DFD Реализация)";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            _chkUseSql = new CheckBox { Text = "Использовать чистый SQL для всех запросов", Location = new Point(20, 10), Size = new Size(350, 25), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _chkUseSql.CheckedChanged += (s, e) => LoadAllData();
            this.Controls.Add(_chkUseSql);

            _tabControl = new TabControl { Location = new Point(10, 40), Size = new Size(660, 450) };
            this.Controls.Add(_tabControl);

            // --- 1. Вкладка "Команды" ---
            var tabTeams = new TabPage("Команды");
            var txtTeamName = new TextBox { Location = new Point(10, 10), Size = new Size(150, 25), PlaceholderText = "Название команды" };
            var btnAddTeam = new Button { Text = "Добавить", Location = new Point(170, 8), Size = new Size(80, 27) };
            _gridTeams = new DataGridView { Location = new Point(10, 45), Size = new Size(630, 360), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            btnAddTeam.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtTeamName.Text)) return;
                if (await ApiClient.Instance.CreateTeamAsync(txtTeamName.Text, _cs2GameId, _chkUseSql.Checked)) { txtTeamName.Clear(); LoadAllData(); }
            };
            tabTeams.Controls.AddRange(new Control[] { txtTeamName, btnAddTeam, _gridTeams });
            _tabControl.TabPages.Add(tabTeams);

            // --- 2. Вкладка "Игроки" ---
            var tabPlayers = new TabPage("Игроки");
            var txtPlayerNick = new TextBox { Location = new Point(10, 10), Size = new Size(150, 25), PlaceholderText = "Никнейм" };
            var btnAddPlayer = new Button { Text = "Добавить", Location = new Point(170, 8), Size = new Size(80, 27) };
            _gridPlayers = new DataGridView { Location = new Point(10, 45), Size = new Size(630, 360), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            btnAddPlayer.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPlayerNick.Text)) return;
                if (await ApiClient.Instance.CreatePlayerAsync(txtPlayerNick.Text, "Имя", "Фамилия", _chkUseSql.Checked)) { txtPlayerNick.Clear(); LoadAllData(); }
            };
            tabPlayers.Controls.AddRange(new Control[] { txtPlayerNick, btnAddPlayer, _gridPlayers });
            _tabControl.TabPages.Add(tabPlayers);

            // --- 3. Вкладка "Турниры" ---
            var tabTournaments = new TabPage("Турниры");
            var txtTourneyName = new TextBox { Location = new Point(10, 10), Size = new Size(150, 25), PlaceholderText = "Название турнира" };
            var txtPrizepool = new TextBox { Location = new Point(170, 10), Size = new Size(100, 25), PlaceholderText = "Призовой (₽)" };
            var btnAddTourney = new Button { Text = "Создать", Location = new Point(280, 8), Size = new Size(80, 27) };
            _gridTournaments = new DataGridView { Location = new Point(10, 45), Size = new Size(630, 360), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            btnAddTourney.Click += async (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtTourneyName.Text) || !int.TryParse(txtPrizepool.Text, out int prize)) return;
                if (await ApiClient.Instance.CreateTournamentAsync(txtTourneyName.Text, prize, _chkUseSql.Checked)) { txtTourneyName.Clear(); txtPrizepool.Clear(); LoadAllData(); }
            };
            tabTournaments.Controls.AddRange(new Control[] { txtTourneyName, txtPrizepool, btnAddTourney, _gridTournaments });
            _tabControl.TabPages.Add(tabTournaments);

            // --- 4. Вкладка "Результаты" ---
            var tabResults = new TabPage("Результаты");
            var txtPlace = new TextBox { Location = new Point(10, 10), Size = new Size(60, 25), PlaceholderText = "Место" };
            var txtPoints = new TextBox { Location = new Point(80, 10), Size = new Size(70, 25), PlaceholderText = "Очки" };
            var txtPrize = new TextBox { Location = new Point(160, 10), Size = new Size(80, 25), PlaceholderText = "Призовые" };
            var btnAddResult = new Button { Text = "Фиксировать", Location = new Point(250, 8), Size = new Size(100, 27) };
            _gridResults = new DataGridView { Location = new Point(10, 45), Size = new Size(630, 360), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            btnAddResult.Click += async (s, e) =>
            {
                // Проверяем, что введены именно числа
                if (!int.TryParse(txtPlace.Text, out int place) || !int.TryParse(txtPoints.Text, out int points) || !int.TryParse(txtPrize.Text, out int prize)) return;

                if (await ApiClient.Instance.RecordResultAsync(place, points, prize, _testMatchId, _testTeamId, _chkUseSql.Checked))
                {
                    txtPlace.Clear(); txtPoints.Clear(); txtPrize.Clear(); LoadAllData();
                }
            };
            tabResults.Controls.AddRange(new Control[] { txtPlace, txtPoints, txtPrize, btnAddResult, _gridResults });
            _tabControl.TabPages.Add(tabResults);
        }

        private async void LoadAllData()
        {
            // Обновляем все 4 таблицы при старте и при нажатии на галочку SQL
            _gridTeams.DataSource = await ApiClient.Instance.GetTeamsAsync("", _chkUseSql.Checked);
            _gridPlayers.DataSource = await ApiClient.Instance.GetPlayersAsync("", _chkUseSql.Checked);
            _gridTournaments.DataSource = await ApiClient.Instance.GetTournamentsAsync(_chkUseSql.Checked);
            _gridResults.DataSource = await ApiClient.Instance.GetResultsAsync(_chkUseSql.Checked);

            // Скрываем технические колонки ID для красоты
            if (_gridTeams.Columns["Id"] != null) _gridTeams.Columns["Id"].Visible = false;
            if (_gridPlayers.Columns["Id"] != null) _gridPlayers.Columns["Id"].Visible = false;
            if (_gridTournaments.Columns["Id"] != null) _gridTournaments.Columns["Id"].Visible = false;
            if (_gridResults.Columns["Id"] != null)
            {
                _gridResults.Columns["Id"].Visible = false;
                _gridResults.Columns["MatchId"].Visible = false;
                _gridResults.Columns["TeamId"].Visible = false;
            }
        }
    }
}