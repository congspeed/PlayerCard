using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PlayerCard
{
    public partial class MainForm : Form
    {
        private List<Player> players;

        public MainForm()
        {
            InitializeComponent();
            LoadPlayersFromJson();
            SetupBindings();
        }



        private void SetupBindings()
        {
            listBoxPlayers.DataSource = players;
            listBoxPlayers.DisplayMember = "Name";
            listBoxPlayers.SelectedIndexChanged += ListBoxPlayers_SelectedIndexChanged;

            if (players.Any())
                UpdateCard(players[0]);
        }

        private void ListBoxPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedPlayer = listBoxPlayers.SelectedItem as Player;
            if (selectedPlayer != null)
            {
                UpdateCard(selectedPlayer);
            }
        }

        private void UpdateCard(Player player)
        {
            if (player == null)
            {
                lblName.Text = "No player selected";
                lblTeam.Text = "";
                panelCard.BackColor = Color.White;
                pictureBoxPlayer.Image = null;
                txtAchievements.Text = "";
                pictureBoxTeam.Image = null;
                return;
            }

            lblName.Text = player.Name;
            lblTeam.Text = player.Team;
            rtbStats.Clear();

            if (player.PointsPerGame > 20)
                rtbStats.SelectionColor = Color.Green;
            else
                rtbStats.SelectionColor = Color.Red;
            rtbStats.AppendText($"PPG: {player.PointsPerGame}\n");

            if (player.Rebounds > 5)
                rtbStats.SelectionColor = Color.Green;
            else
                rtbStats.SelectionColor = Color.Red;
            rtbStats.AppendText($"RPG: {player.Rebounds}\n");

            if (player.Assists > 5)
                rtbStats.SelectionColor = Color.Green;
            else
                rtbStats.SelectionColor = Color.Red;
            rtbStats.AppendText($"APG: {player.Assists}\n");

            if (player.ShootingPercentage > 50)
                rtbStats.SelectionColor = Color.Green;
            else
                rtbStats.SelectionColor = Color.Red;
            rtbStats.AppendText($"FG%: {player.ShootingPercentage}\n");

            Color backgroundColor;
            switch (player.Team)
            {
                case "Chicago Bulls":
                    backgroundColor = Color.Red;
                    break;
                case "Los Angeles Lakers":
                    backgroundColor = Color.Yellow;
                    break;
                case "Utah Jazz":
                    backgroundColor = Color.Violet;
                    break;
                default:
                    backgroundColor = Color.White;
                    break;
            }
            panelCard.BackColor = backgroundColor;

            if (!string.IsNullOrEmpty(player.PhotoPath) && System.IO.File.Exists(player.PhotoPath))
            {
                pictureBoxPlayer.Image = Image.FromFile(player.PhotoPath);
            }
            else
            {
                pictureBoxPlayer.Image = null;
            }

            string teamImagePath = GetTeamImagePath(player.Team);
            if (!string.IsNullOrEmpty(teamImagePath) && System.IO.File.Exists(teamImagePath))
            {
                pictureBoxTeam.Image = Image.FromFile(teamImagePath);
            }
            else
            {
                pictureBoxTeam.Image = null;
            }

            txtAchievements.Text = string.Join(Environment.NewLine, player.Achievements);
        }


        private string GetTeamImagePath(string teamName)
        {
            string basePath = "../../../PlayerCard/Teams";
            string teamImagePath = null;

            switch (teamName)
            {
                case "Chicago Bulls":
                    teamImagePath = System.IO.Path.Combine(basePath, "bulls.jpg");
                    break;
                case "Los Angeles Lakers":
                    teamImagePath = System.IO.Path.Combine(basePath, "lakers.jpg");
                    break;
                case "Utah Jazz":
                    teamImagePath = System.IO.Path.Combine(basePath, "jazz.jpg");
                    break;
                default:
                    teamImagePath = null;
                    break;
            }

            return teamImagePath;
        }


        private void lblStats_Click(object sender, EventArgs e)
        {

        }

        private void lblName_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var addPlayerForm = new AddPlayerForm())
            {
                if (addPlayerForm.ShowDialog() == DialogResult.OK)
                {
                    Player newPlayer = addPlayerForm.NewPlayer;

                    if (players.Any(p => p.Name == newPlayer.Name))
                    {
                        MessageBox.Show("Player with this name already exists.", "Duplicate Player", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;  
                    }

                    if (string.IsNullOrEmpty(newPlayer.PhotoPath))
                    {
                        newPlayer.PhotoPath = "../../../PlayerCard/Photos/default.jpg";
                    }

                    players.Add(newPlayer);

                    listBoxPlayers.DataSource = null;
                    listBoxPlayers.DataSource = players;
                    listBoxPlayers.DisplayMember = "Name";

                    SavePlayersToJson();

                    UpdateCard(newPlayer);
                }
            }
        }




        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selectedPlayer = listBoxPlayers.SelectedItem as Player;
            if (selectedPlayer != null)
            {
                using (var editPlayerForm = new AddPlayerForm(selectedPlayer))
                {
                    if (editPlayerForm.ShowDialog() == DialogResult.OK)
                    {
                        Player updatedPlayer = editPlayerForm.NewPlayer;

                        if (string.IsNullOrEmpty(updatedPlayer.PhotoPath))
                        {
                            updatedPlayer.PhotoPath = "../../../PlayerCard/Photos/default.jpg";
                        }

                        int index = players.IndexOf(selectedPlayer);
                        if (index >= 0)
                        {
                            players[index] = updatedPlayer; 
                        }

                        listBoxPlayers.DataSource = null;
                        listBoxPlayers.DataSource = players;
                        listBoxPlayers.DisplayMember = "Name";

                        SavePlayersToJson();

                        UpdateCard(updatedPlayer);
                    }
                }
            }
        }






        private void btnDelete_Click(object sender, MouseEventArgs e)
        {
            if (listBoxPlayers.SelectedItem is Player selectedPlayer)
            {
                var confirmation = MessageBox.Show($"Are you sure you want to delete {selectedPlayer.Name}?",
                                                   "Delete Player", MessageBoxButtons.YesNo);

                if (confirmation == DialogResult.Yes)
                {
                    players.Remove(selectedPlayer);

                    listBoxPlayers.DataSource = null;
                    listBoxPlayers.DataSource = players;
                    listBoxPlayers.DisplayMember = "Name";

                    if (players.Any())
                    {
                        UpdateCard(players.First());
                    }
                    else
                    {
                        UpdateCard(null);
                    }

                    SavePlayersToJson();

                    MessageBox.Show($"Player {selectedPlayer.Name} deleted successfully.");
                }
            }
            else
            {
                MessageBox.Show("Please select a player to delete.");
            }
        }


        private void LoadPlayersFromJson()
        {
            string jsonFilePath = "../../../PlayerCard/players.json";
            if (System.IO.File.Exists(jsonFilePath))
            {
                string json = System.IO.File.ReadAllText(jsonFilePath);
                players = JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
            }
            else
            {
                MessageBox.Show("Players JSON file not found.");
                players = new List<Player>();
            }
        }


        private void SavePlayersToJson()
        {
            try
            {
                string json = JsonConvert.SerializeObject(players, Formatting.Indented);
                System.IO.File.WriteAllText("../../../PlayerCard/players.json", json);
                MessageBox.Show("Players saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving players: {ex.Message}");
            }
        }




    }
}