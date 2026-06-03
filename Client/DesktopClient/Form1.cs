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

        // Выпадающие списки (ComboBox) для вкладки результатов
        private ComboBox _cbTeams, _cbTournaments;

        // Удалили _testTeamId и _testTourneyId, так как теперь мы берем реальные ID из базы
        private readonly Guid _cs2GameId = Guid.Parse("11111111-1111-1111-1111-000000000001");

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

            btnAddTeam.Click += async (s, e) => {
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

            btnAddPlayer.Click += async (s, e) => {
                if (string.IsNullOrWhiteSpace(txtPlayerNick.Text)) return;
                if (await ApiClient.Instance.CreatePlayerAsync(txtPlayerNick.Text, "Имя", "Фамилия", _chkUseSql.Checked)) { txtPlayerNick.Clear(); LoadAllData(); }
            };
            tabPlayers.Controls.AddRange(new Control[] { txtPlayerNick, btnAddPlayer, _gridPlayers });
            _tabControl.TabPages.Add(tabPlayers);

            // --- 3. Вкладка "Турниры" ---
            var tabTournaments = new TabPage("Турниры");
            var txtTourneyName = new TextBox { Location = new Point(10, 10), Size = new Size(150, 25), PlaceholderText = "Название турнира" };
            var txtMaxParts = new TextBox { Location = new Point(170, 10), Size = new Size(120, 25), PlaceholderText = "Макс. участников" };
            var btnAddTourney = new Button { Text = "Создать", Location = new Point(300, 8), Size = new Size(80, 27) };
            _gridTournaments = new DataGridView { Location = new Point(10, 45), Size = new Size(630, 360), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            btnAddTourney.Click += async (s, e) => {
                if (string.IsNullOrWhiteSpace(txtTourneyName.Text) || !int.TryParse(txtMaxParts.Text, out int maxP)) return;
                if (await ApiClient.Instance.CreateTournamentAsync(txtTourneyName.Text, maxP, _chkUseSql.Checked)) { txtTourneyName.Clear(); txtMaxParts.Clear(); LoadAllData(); }
            };
            tabTournaments.Controls.AddRange(new Control[] { txtTourneyName, txtMaxParts, btnAddTourney, _gridTournaments });
            _tabControl.TabPages.Add(tabTournaments);

            // --- 4. Вкладка "Результаты" ---
            var tabResults = new TabPage("Результаты");

            // СОЗДАЕМ ВЫПАДАЮЩИЕ СПИСКИ ДЛЯ ВЫБОРА КОМАНДЫ И ТУРНИРА
            _cbTeams = new ComboBox { Location = new Point(10, 10), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cbTournaments = new ComboBox { Location = new Point(170, 10), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            var txtPrizeMoney = new TextBox { Location = new Point(330, 10), Size = new Size(100, 25), PlaceholderText = "Призовые (₽)" };

            var btnAddResult = new Button { Text = "Фиксировать", Location = new Point(440, 8), Size = new Size(100, 27) };
            _gridResults = new DataGridView { Location = new Point(10, 45), Size = new Size(630, 360), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            btnAddResult.Click += async (s, e) => {
                // Проверяем, что введена сумма и выбраны реальные команда и турнир из списка
                if (!int.TryParse(txtPrizeMoney.Text, out int prize) || _cbTeams.SelectedValue == null || _cbTournaments.SelectedValue == null) return;

                // Получаем настоящие ID из базы данных
                var realTeamId = (Guid)_cbTeams.SelectedValue;
                var realTourneyId = (Guid)_cbTournaments.SelectedValue;

                if (await ApiClient.Instance.RecordResultAsync(prize, realTeamId, realTourneyId, _chkUseSql.Checked))
                {
                    txtPrizeMoney.Clear(); LoadAllData();
                }
            };
            tabResults.Controls.AddRange(new Control[] { _cbTeams, _cbTournaments, txtPrizeMoney, btnAddResult, _gridResults });
            _tabControl.TabPages.Add(tabResults);
        }

        private async void LoadAllData()
        {
            // Скачиваем данные с сервера
            var teams = await ApiClient.Instance.GetTeamsAsync("", _chkUseSql.Checked);
            var players = await ApiClient.Instance.GetPlayersAsync("", _chkUseSql.Checked);
            var tournaments = await ApiClient.Instance.GetTournamentsAsync(_chkUseSql.Checked);
            var results = await ApiClient.Instance.GetResultsAsync(_chkUseSql.Checked);

            // Обновляем таблицы
            _gridTeams.DataSource = teams;
            _gridPlayers.DataSource = players;
            _gridTournaments.DataSource = tournaments;
            _gridResults.DataSource = results;

            // ЗАПОЛНЯЕМ ВЫПАДАЮЩИЕ СПИСКИ РЕАЛЬНЫМИ ДАННЫМИ ИЗ БАЗЫ
            _cbTeams.DataSource = teams;
            _cbTeams.DisplayMember = "Name"; // То, что видит пользователь
            _cbTeams.ValueMember = "Id";     // То, что отправляется на сервер

            _cbTournaments.DataSource = tournaments;
            _cbTournaments.DisplayMember = "Name";
            _cbTournaments.ValueMember = "Id";

            // Скрываем технические колонки ID для красоты
            if (_gridTeams.Columns["Id"] != null) _gridTeams.Columns["Id"].Visible = false;
            if (_gridPlayers.Columns["Id"] != null) _gridPlayers.Columns["Id"].Visible = false;
            if (_gridTournaments.Columns["Id"] != null) _gridTournaments.Columns["Id"].Visible = false;

            if (_gridResults.Columns["Id"] != null) _gridResults.Columns["Id"].Visible = false;
            if (_gridResults.Columns["WinnerTeam"] != null) _gridResults.Columns["WinnerTeam"].Visible = false;
            if (_gridResults.Columns["TournamentId"] != null) _gridResults.Columns["TournamentId"].Visible = false;
        }
    }
}