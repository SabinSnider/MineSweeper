﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
       
        struct Timex
        {
            public int minute, second;
            public Timex(int m, int s)
            {
                minute = m;
                second = s;
            }
        }

        //Initialise
        bool game_flag = false, game_start = false, game_pause = false, win_flag = false;
        Dictionary<Button, int> ajx = new Dictionary<Button, int>();
        Dictionary<Button, int> flx = new Dictionary<Button, int>();
        Button[,] box = new Button[10, 10];
        int[,] mines = new int[10, 10];
        Timex timer = new Timex(0, 0);
        Random rand = new Random();
        int flags = 10;
        Timer runtime;

        public Form1()
        {
            InitializeComponent();
            Pause.Enabled = false;
            Cell_diclosure();
        }

        void ExitButtonClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void Start_Game() // Could use a timer here to run the game
        {
            runtime = new Timer();
            runtime.Enabled = true;
            runtime.Interval = 1000;
            runtime.Tick += Runtime_Tick;
            Pause.Enabled = true;
        }

        void Game_Reset() // After clicking replay button
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    box[i, j].Enabled = true;
                    box[i, j].BackColor = SystemColors.Control;
                }
            }

            if (game_start) runtime.Dispose();
            Pause.Enabled = game_pause = false;
            timer.minute = timer.second = 0;
            Watch.Text = "00 : 00";
            Flag.Text = "10";
            Pause.Text = "PAUSE";
            game_start = false;
            flags = 10;
            ajx.Clear();
            if (win_flag)
            {
                win_flag = false;
                Title.Text = "MINESWEEPER";
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    flx[box[i, j]] = 0;
                }
            }
            Reset_Mines();
        }

        void Cell_diclosure() //Diclosure of cells in form
        {
            int x = 25, y = 16;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    box[i, j] = new Button();
                    box[i, j].Font = new Font("Consolas", 12F, FontStyle.Bold);
                    box[i, j].Click += new EventHandler(BoxButtonClick);
                    box[i, j].MouseDown += new MouseEventHandler(FlagSetting);
                    box[i, j].BackColor = SystemColors.Control;
                    box[i, j].FlatStyle = FlatStyle.Flat;
                    box[i, j].FlatAppearance.BorderSize = 0;
                    box[i, j].UseVisualStyleBackColor = false;
                    box[i, j].Location = new Point(x, y);
                    box[i, j].Size = new Size(30, 30);
                    box[i, j].TabIndex = 2;
                    Controls.Add(box[i, j]);
                    flx[box[i, j]] = 0;
                    x += 46;
                }
                x = 25;
                y += 46;
            }
        }

        void Set_Mines() // Setting mines in the grid randomly
        {
            for (int i = 0; i < 10; i++)
            {
                int[] visit = new int[10];
                int n = rand.Next(1, 3), x = 0; // Difficulty controlled by upperbound

                while (x < n)
                {
                    int z = rand.Next(0, 10);
                    if (visit[z] == 0)
                    {
                        
                        mines[i, z] = 1;
                        visit[z] = 1;
                        x++;
                    }
                }
            }

            // Set mine adjacencies
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int c = 0;

                    if (i + 1 < 10)
                    {
                        if (mines[i + 1, j] == 1) c++;
                    }
                    if (i - 1 >= 0)
                    {
                        if (mines[i - 1, j] == 1) c++;
                    }
                    if (j + 1 < 10)
                    {
                        if (mines[i, j + 1] == 1) c++;
                    }
                    if (j - 1 >= 0)
                    {
                        if (mines[i, j - 1] == 1) c++;
                    }
                    ajx[box[i, j]] = c;
                   
                }
            }
        }

        void Reset_Mines() // Making mines every index zero
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    mines[i, j] = 0;
                    box[i, j].Text = "";
                }
            }
            Set_Mines();
        }

        bool Is_Mine(Button b) // Checks if the selected box is a mine or not
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (box[i, j] == b)
                    {
                        if (mines[i, j] == 0)
                            return false;
                        else
                            return true;
                    }
                }
            }
            return false;
        }


        bool Win()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (mines[i, j] == 1 || flx[box[i, j]] == 1)
                        continue;
                    else
                    {
                        if (box[i, j].Enabled)
                            return false;
                    }
                }
            }
            return true;
        }

        void BoxButtonClick(object sender, EventArgs e)
        {
            if (!game_pause)
            {
                if (game_flag)
                {  //flag setting
                    Button b = (Button)sender;

                    if (flx[b] == 1) return;

                    if (Is_Mine(b))
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            for (int j = 0; j < 10; j++)
                            {
                                if (mines[i, j] == 1)
                                {
                                    box[i, j].Text = "💀";
                                    box[i, j].Enabled = false;
                                    box[i, j].BackColor = Color.LightGray;
                                    box[i, j].Font = new Font("Consolas", 15F, FontStyle.Bold);
                                }
                            }
                        }

                        if (game_start) runtime.Dispose();
                        Pause.Enabled = false;
                        game_pause = true;
                        MessageBox.Show("Failed, Try Again! ");
                    }
                    else
                    {
                        if (ajx[b] != 0)
                        {
                            b.Enabled = false;
                            b.Text = ajx[b].ToString();
                            b.BackColor = Color.LightGray;
                            b.Font = new Font("Consolas", 12F, FontStyle.Bold);
                        }
                        else
                        {
                            Special_Clear(b);
                        }

                        if (!game_start)
                        {
                            game_start = true;
                            Start_Game();
                        }
                    }
                }
            }

            if (Win())
            {
                if (game_start) runtime.Dispose();
                Title.Text = "YOU WIN!\n CLEAR.";
                Pause.Enabled = false;
                game_pause = true;
                win_flag = true;

                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (mines[i, j] == 1)
                        {
                            box[i, j].Text = "💀";
                            box[i, j].Font = new Font("Consolas", 15F, FontStyle.Bold);
                        }
                    }
                }
            }
           
        }
        void FlagSetting(object sender, MouseEventArgs e)
        { 
            if (!game_pause)
            {
                if (game_flag)
                {   

                    Button b = (Button)sender;
                    if (e.Button == MouseButtons.Right)
                    {  //FlagSetting
                        if (flx[b] == 0)
                        {
                            if (flags != 0)
                            {
                                flags--;
                                flx[b] = 1;
                                b.Text = "🚩";
                                Flag.Text = flags.ToString();
                                b.Font = new Font("Consolas", 15F, FontStyle.Bold);
                            }
                        }
                        else
                        { //flag release
                            flags++;
                            flx[b] = 0;
                            b.Text = "";
                            Flag.Text = flags.ToString();
                        }
                    }
                }
            }
        }




        void Special_Clear(Button b)
        {
            int x = 0, y = 0, up_i = 0, down_i = 9;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (box[i, j].Equals(b))
                    {
                        x = i; y = j;
                    }
                }
            }

            for (int i = x + 1; i < 10; i++) // checking lower limit
            {
                if (ajx[box[i, y]] != 0)
                {
                    down_i = i;
                    break;
                }
            }

            for (int i = x - 1; i >= 0; i--) // checking upper limit
            {
                if (ajx[box[i, y]] != 0)
                {
                    up_i = i;
                    break;
                }
            }


            for (int i = up_i; i <= down_i; i++)
            {
                for (int j = y; j < 10; j++)
                {
                    if (flx[box[i, j]] == 1) continue;

                    if (ajx[box[i, j]] != 0)
                    {
                        box[i, j].Enabled = false;
                        box[i, j].Text = ajx[box[i, j]].ToString();
                        box[i, j].BackColor = Color.LightGray;
                        box[i, j].Font = new Font("Consolas", 12F, FontStyle.Bold);
                        break;
                    }
                    else
                    {
                        box[i, j].Enabled = false;
                        box[i, j].BackColor = Color.LightGray;
                        box[i, j].Font = new Font("Consolas", 12F, FontStyle.Bold);
                    }
                }

                for (int j = y; j >= 0; j--)
                {
                    if (flx[box[i, j]] == 1) continue;

                    if (ajx[box[i, j]] != 0)
                    {
                        box[i, j].Enabled = false;
                        box[i, j].Text = ajx[box[i, j]].ToString();
                        box[i, j].BackColor = Color.LightGray;
                        box[i, j].Font = new Font("Consolas", 12F, FontStyle.Bold);
                        break;
                    }
                    else
                    {
                        box[i, j].Enabled = false;
                        box[i, j].BackColor = Color.LightGray;
                        box[i, j].Font = new Font("Consolas", 12F, FontStyle.Bold);
                    }
                }
            }
        }


        void PlayButtonClick(object sender, EventArgs e)
        {
            if (!game_flag)
            {
                Play.Text = "REPLAY";
                game_flag = true;
                Set_Mines();
            }
            else
            {
                Game_Reset();
            }
        }

        void PauseButtonClick(object sender, EventArgs e)
        {
            if (game_pause)
            {
                game_pause = false;
                Pause.Text = "PAUSE";
            }
            else
            {
                game_pause = true;
                Pause.Text = "PLAY";
            }
        }

        void Runtime_Tick(object sender, EventArgs e)
        {
            if (!game_pause)
            {
                timer.second++;
                if (timer.second == 60)
                {
                    timer.second = 0;
                    timer.minute++;
                }

                string mm = timer.minute.ToString(), ss = timer.second.ToString();
                if (mm.Length == 1)
                {
                    if (ss.Length == 1)
                    {
                        Watch.Text = "0" + mm + " : 0" + ss;
                    }
                    else
                    {
                        Watch.Text = "0" + mm + " : " + ss;
                    }
                }
                else
                {
                    if (ss.Length == 1)
                    {
                        Watch.Text = mm + " : 0" + ss;
                    }
                    else
                    {
                        Watch.Text = mm + " : " + ss;
                    }
                }
            }
        }

    }
}
