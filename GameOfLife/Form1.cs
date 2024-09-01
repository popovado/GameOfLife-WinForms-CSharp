using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private int resolution;

        private bool[,] field;
        private int rows;
        private int columns;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            if (timer1.Enabled)
                return;
            
            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            resolution = (int)nudResolution.Value;

            rows = pictureBox1.Height/resolution;
            columns = pictureBox1.Width/resolution;
            field = new bool[columns,rows];

            Random rnd = new Random();
            for (int x = 0; x < columns; x++) 
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = rnd.Next((int)nudDensity.Value)==0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void NextGeneration()
        {
            graphics.Clear(Color.Black);
            
            var newField = new bool[columns, rows];

            for (int x = 0; x < columns; x++) 
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if (!hasLife && neighboursCount==3) 
                        newField[x, y] = true;
                    else if(hasLife && (neighboursCount<2 || neighboursCount >3))
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];

                    if (hasLife)
                        graphics.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);
                }
            }
            field = newField;
            pictureBox1.Refresh();
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            { 
                for (int j = -1; j < 2; j++)
                {
                    //var col = x + i;
                    //var row = y + j;

                    // модифицируем расчепт кординат. ведь если нам попадется самая левая клетка, то х будет нулем, и тогда вылезет ошибка.
                    // наше поле - как карта мира, если слева заканчивается карта, то справа она продолжается (потому что мир круглый)

                    var col = (x + i + columns)% columns;
                    var row = (y + j + rows)% rows;

                    var isSelfChecking = col == x && row == y; //проверяем соседей, но нас(клетку) не считаем
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfChecking)
                    {
                        count++; // нашли живого соседа, поэтому +1
                    }
                }
            }

            return count;
        }

        private void StopGame()
        {
            if(!timer1.Enabled)
                return;

            timer1.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            StopGame();
        }
    }
}
