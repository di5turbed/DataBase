using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBase
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=Y:\DataBase\DataBase\MyDatabase.mdf;Integrated Security=True";

        private SqlDataAdapter playersAdapter;
        private DataTable playersTable;
        private SqlCommandBuilder playersBuilder;

        private SqlDataAdapter gamesAdapter;
        private DataTable gamesTable;
        private SqlCommandBuilder gamesBuilder;

        public Form1()
        {
            InitializeComponent();

            this.Load += new EventHandler(Form1_Load);

            buttonSave.Click += new EventHandler(buttonSave_Click);
            buttonRefresh.Click += new EventHandler(buttonRefresh_Click);

            txtSearchPlayer.TextChanged += new EventHandler(txtSearchPlayer_TextChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPlayers();
            LoadGames();
        }

        private void LoadPlayers()
        {
            try
            {
                string query = "SELECT id, nickname, first_name, last_name, phone, reg_date, date_of_birth FROM Players";
                playersAdapter = new SqlDataAdapter(query, connectionString);
                playersBuilder = new SqlCommandBuilder(playersAdapter);
                playersTable = new DataTable();

                playersAdapter.Fill(playersTable);

                dgvPlayers.DataSource = playersTable;

                if (dgvPlayers.Columns["id"] != null) dgvPlayers.Columns["id"].Visible = false;

                dgvPlayers.Columns["nickname"].HeaderText = "Никнейм";
                dgvPlayers.Columns["first_name"].HeaderText = "Имя";
            }
            catch (Exception ex) { MessageBox.Show("Ошибка игроков: " + ex.Message); }
        }

        private void LoadGames()
        {
            try
            {
                string query = "SELECT id, name, category, notes FROM Game";
                gamesAdapter = new SqlDataAdapter(query, connectionString);
                gamesBuilder = new SqlCommandBuilder(gamesAdapter);
                gamesTable = new DataTable();

                gamesAdapter.Fill(gamesTable);

                dgvGames.DataSource = gamesTable;

                if (dgvGames.Columns["id"] != null) dgvGames.Columns["id"].Visible = false;

                dgvGames.Columns["name"].HeaderText = "Название игры";
                dgvGames.Columns["category"].HeaderText = "Категория";
                dgvGames.Columns["notes"].HeaderText = "Заметки";
            }
            catch (Exception ex) { MessageBox.Show("Ошибка игр: " + ex.Message); }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPlayers.IsCurrentCellInEditMode) dgvPlayers.EndEdit();
                if (dgvGames.IsCurrentCellInEditMode) dgvGames.EndEdit();

                if (playersAdapter != null && playersTable != null) playersAdapter.Update(playersTable);
                if (gamesAdapter != null && gamesTable != null) gamesAdapter.Update(gamesTable);

                MessageBox.Show("Данные успешно сохранены в БД!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadPlayers();
                LoadGames();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadPlayers();
            LoadGames();
        }

        private void txtSearchPlayer_TextChanged(object sender, EventArgs e)
        {
            if (playersTable != null)
            {
                playersTable.DefaultView.RowFilter = string.Format("nickname LIKE '%{0}%'", txtSearchPlayer.Text);
            }
        }
    }
}
