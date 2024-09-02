using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace GameOfLife
{
    public class GameEngine
    {
        public uint CurrentGeneration {get; private set;}
        private bool[,] field;
        private readonly int rows;
        private readonly int columns;

        public GameEngine(int rows, int columns, int density)
        {
            this.rows = rows;
            this.columns = columns;
            field = new bool[columns, rows];
            
            Random rnd = new Random();

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = rnd.Next(density) == 0;
                }
            }
        }

        public void NextGeneration()
        {
            var newField = new bool[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighboursCount = CountNeighbours(x, y);
                    var hasLife = field[x, y];

                    if (!hasLife && neighboursCount == 3)
                        newField[x, y] = true;
                    else if (hasLife && (neighboursCount < 2 || neighboursCount > 3))
                        newField[x, y] = false;
                    else
                        newField[x, y] = field[x, y];
                }
            }
            field = newField;
            CurrentGeneration++;
        }

        public bool[,] GetCurrentGeneration()
        {
            var result = new bool[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    result[x, y] = field[x, y];
                }
            }
            return result;
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

                    var col = (x + i + columns) % columns;
                    var row = (y + j + rows) % rows;

                    var isSelfChecking = col == x && row == y; //проверяем соседей, но нас(клетку) не считаем
                    var hasLife = field[col, row];

                    if (hasLife && !isSelfChecking)
                        count++; // нашли живого соседа, поэтому +1
                }
            }

            return count;
        }
        private bool ValidateCellPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < columns && y < rows;
        }

        private void UpdateCell(int x, int y, bool state)
        {
            if(ValidateCellPosition(x, y))
                field[x, y] = state;
        }

        public void AddCell(int x, int y)
        { 
            UpdateCell(x, y, state: true);
        }

        public void RemoveCell(int x, int y)
        { 
            UpdateCell(x, y, state: false);
        }
    }
}
