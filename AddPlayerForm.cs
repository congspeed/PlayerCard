using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using JsonNet = Newtonsoft.Json.JsonSerializer;
using SystemTextJson = System.Text.Json.JsonSerializer;



namespace PlayerCard
{
    public partial class AddPlayerForm : Form
    {
        private TextBox txtTeam;
        private NumericUpDown numPointsPerGame;
        private NumericUpDown numRebounds;
        private NumericUpDown numAssists;
        private NumericUpDown numShootingPercentage;
        private Button btnSave;
        private TextBox txtAchievements;
        private TextBox txtPhotoPath;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Button button1;
        private TextBox txtName;

        public Player NewPlayer { get; private set; }

        public AddPlayerForm()
        {
            InitializeComponent();
            NewPlayer = new Player();  
        }

        public AddPlayerForm(Player existingPlayer) : this()
        {
            txtName.Text = existingPlayer.Name ?? ""; 
            txtTeam.Text = existingPlayer.Team ?? ""; 
            txtAchievements.Text = existingPlayer.Achievements != null ? string.Join(Environment.NewLine, existingPlayer.Achievements) : "";

            numPointsPerGame.DecimalPlaces = 1; 
            numPointsPerGame.Increment = 0.1m;
            numPointsPerGame.Minimum = 0; 
            numPointsPerGame.Maximum = 100;
            numPointsPerGame.ThousandsSeparator = false;

            numRebounds.DecimalPlaces = 1;
            numRebounds.Increment = 0.1m;
            numRebounds.Minimum = 0;
            numRebounds.Maximum = 50;

            numAssists.DecimalPlaces = 1;
            numAssists.Increment = 0.1m;
            numAssists.Minimum = 0;
            numAssists.Maximum = 20;

            numShootingPercentage.DecimalPlaces = 1;
            numShootingPercentage.Increment = 0.1m;
            numShootingPercentage.Minimum = 0;
            numShootingPercentage.Maximum = 100;

            numPointsPerGame.Value = existingPlayer.PointsPerGame.HasValue ? (decimal)existingPlayer.PointsPerGame.Value : 0m;
            numRebounds.Value = existingPlayer.Rebounds.HasValue ? (decimal)existingPlayer.Rebounds.Value : 0m;
            numAssists.Value = existingPlayer.Assists.HasValue ? (decimal)existingPlayer.Assists.Value : 0m;
            numShootingPercentage.Value = existingPlayer.ShootingPercentage.HasValue ? (decimal)existingPlayer.ShootingPercentage.Value : 0m;

            NewPlayer = existingPlayer; 
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtTeam.Text))
            {
                MessageBox.Show("Name and Team are required fields.");
                return;
            }

            Player playerToSave = new Player
            {
                Name = txtName.Text,
                Team = txtTeam.Text,
                PointsPerGame = (double)numPointsPerGame.Value,
                Rebounds = (double)numRebounds.Value,
                Assists = (double)numAssists.Value,
                ShootingPercentage = (double)numShootingPercentage.Value,
                Achievements = !string.IsNullOrEmpty(txtAchievements.Text) ? new List<string>(txtAchievements.Text.Split(new[] { "\r\n" }, StringSplitOptions.None)) : new List<string>()
            };

            playerToSave.PhotoPath = txtPhotoPath.Text;

            NewPlayer = playerToSave;

            SavePlayerToJson(playerToSave);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SavePlayerToJson(Player player)
        {
            string filePath = "../../../PlayerCard/players.json"; 

            List<Player> players = new List<Player>();
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);

                players = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Player>>(jsonString) ?? new List<Player>();
            }

            var existingPlayerIndex = players.FindIndex(p => p.Name == player.Name);
            if (existingPlayerIndex >= 0)
            {
                players[existingPlayerIndex] = player;
            }
            else
            {
                players.Add(player);
            }

            string updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(players, Formatting.Indented);
            File.WriteAllText(filePath, updatedJson);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "Select a Photo";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    string photosFolderPath = Path.Combine(Application.StartupPath, @"..\..\..\PlayerCard\Photos");
                    if (!Directory.Exists(photosFolderPath))
                    {
                        Directory.CreateDirectory(photosFolderPath);
                    }

                    string fileName = Path.GetFileName(selectedFilePath);
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                    string destinationFilePath = Path.Combine(photosFolderPath, uniqueFileName);

                    File.Copy(selectedFilePath, destinationFilePath);

                    txtPhotoPath.Text = destinationFilePath;
                }
            }
        }


        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddPlayerForm));
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtTeam = new System.Windows.Forms.TextBox();
            this.numPointsPerGame = new System.Windows.Forms.NumericUpDown();
            this.numRebounds = new System.Windows.Forms.NumericUpDown();
            this.numAssists = new System.Windows.Forms.NumericUpDown();
            this.numShootingPercentage = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.txtAchievements = new System.Windows.Forms.TextBox();
            this.txtPhotoPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numPointsPerGame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRebounds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAssists)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numShootingPercentage)).BeginInit();
            this.SuspendLayout();

            this.txtName.Location = new System.Drawing.Point(119, 28);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(120, 22);
            this.txtName.TabIndex = 0;

            this.txtTeam.Location = new System.Drawing.Point(119, 56);
            this.txtTeam.Name = "txtTeam";
            this.txtTeam.Size = new System.Drawing.Size(120, 22);
            this.txtTeam.TabIndex = 1;

            this.numPointsPerGame.Location = new System.Drawing.Point(119, 84);
            this.numPointsPerGame.Name = "numPointsPerGame";
            this.numPointsPerGame.Size = new System.Drawing.Size(120, 22);
            this.numPointsPerGame.TabIndex = 6;
 
            this.numRebounds.Location = new System.Drawing.Point(119, 112);
            this.numRebounds.Name = "numRebounds";
            this.numRebounds.Size = new System.Drawing.Size(120, 22);
            this.numRebounds.TabIndex = 7;

            this.numAssists.Location = new System.Drawing.Point(119, 140);
            this.numAssists.Name = "numAssists";
            this.numAssists.Size = new System.Drawing.Size(120, 22);
            this.numAssists.TabIndex = 8;

            this.numShootingPercentage.Location = new System.Drawing.Point(119, 168);
            this.numShootingPercentage.Name = "numShootingPercentage";
            this.numShootingPercentage.Size = new System.Drawing.Size(120, 22);
            this.numShootingPercentage.TabIndex = 9;

            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(86, 375);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 30);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.txtAchievements.Location = new System.Drawing.Point(17, 256);
            this.txtAchievements.Multiline = true;
            this.txtAchievements.Name = "txtAchievements";
            this.txtAchievements.Size = new System.Drawing.Size(222, 60);
            this.txtAchievements.TabIndex = 11;

            this.txtPhotoPath.Location = new System.Drawing.Point(119, 323);
            this.txtPhotoPath.Name = "txtPhotoPath";
            this.txtPhotoPath.Size = new System.Drawing.Size(120, 22);
            this.txtPhotoPath.TabIndex = 12;

            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 13;
            this.label1.Text = "Name";

            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 16);
            this.label2.TabIndex = 14;
            this.label2.Text = "Team";

            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 16);
            this.label3.TabIndex = 15;
            this.label3.Text = "Points per game";

            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 16);
            this.label4.TabIndex = 16;
            this.label4.Text = "Rebounds";

            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 16);
            this.label5.TabIndex = 17;
            this.label5.Text = "Assists";

            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 16);
            this.label6.TabIndex = 18;
            this.label6.Text = "Shooting % (FG)";

            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 237);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 20);
            this.label7.TabIndex = 19;
            this.label7.Text = "Achievements";

            this.button1.Location = new System.Drawing.Point(17, 323);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);

            this.ClientSize = new System.Drawing.Size(282, 453);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPhotoPath);
            this.Controls.Add(this.txtAchievements);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numShootingPercentage);
            this.Controls.Add(this.numAssists);
            this.Controls.Add(this.numRebounds);
            this.Controls.Add(this.numPointsPerGame);
            this.Controls.Add(this.txtTeam);
            this.Controls.Add(this.txtName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddPlayerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.numPointsPerGame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRebounds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAssists)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numShootingPercentage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
