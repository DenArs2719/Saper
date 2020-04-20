using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper
{
    class SapperLogic
    {
        internal enum FieldTypeEnum
        {
            Bomb,
            BombCount,
            Empty
        }
        internal enum GameState
        {
            InProgress,
            Win,
            Loss
        }

        internal class Field
        {
            //pola opisuje typ i ilość bomb (istotna dla typu BombCount)
            //i dodatkowa informacja o tym czy pole jest zakryte czy odkryte
            private FieldTypeEnum fieldType;
            private int bombCount;
            private bool covered;

            public FieldTypeEnum FieldType { get { return fieldType; } }
            public int BombCount { get { return bombCount; } }
            public bool Covered { get { return covered; } set { covered = value; } }

            internal Field(FieldTypeEnum fieldType, int bombCount = 0)
            {
                this.covered = true;
                this.fieldType = fieldType;
                this.bombCount = bombCount;
            }
        }
        //Random - klasa do generowania liczb pseudolosowych
        readonly Random generator = new Random();

        //dwuwymiarowa tablica przechowująca planszę składającą się z pól
        private Field[,] board;
        //dodatkowe pole przechowuje ilość pól do odkrycia, gdy 0 to wygrana
        private int fieldsToUncover;
        //stan gry
        private GameState state;

        public int BoardWidth
        {
            //pobranie pierwszego (zerowego) wymiaru tablicy wielowymiarowej
            get { return (int)board.GetLongLength(0); }
        }
        public int BoardHeight
        {
            get { return (int)board.GetLongLength(1); }
        }
        public GameState State
        {
            get { return state; }
        }
        //konstruktor binicjujący planszę, losujący bomby, liczący sąsiedztwa bomb, itp.
        public SapperLogic(int width, int height, int bombCount)
        {
            this.fieldsToUncover = (width * height) - bombCount;
            this.board = new Field[width, height];
            //rozmieszczanie bomb
            do
            {
                int x = generator.Next(BoardWidth);
                int y = generator.Next(BoardHeight);
                if (board[x, y] == null)
                {
                    board[x, y] = new Field(FieldTypeEnum.Bomb);
                    bombCount--;
                }
            } while (bombCount > 0);

            //dla pozostałych pól zliczanie bomb w sąsiedztwie
            for (int x = 0; x < BoardWidth; x++)
            {
                for (int y = 0; y < BoardHeight; y++)
                {
                    if (board[x, y] == null)
                    {
                        int localBombCount = 0;
                        //dla pól niezainicjowanych przechodzimy po sąsiadach (x-1 do x+1) i (y-1 do y+1)
                        for (int xx = x - 1; xx <= x + 1; xx++)
                        {
                            for (int yy = y - 1; yy <= y + 1; yy++)
                            {
                                //kontrola czy w planszy i czy bomba
                                if (xx >= 0 && xx < BoardWidth &&
                                    yy >= 0 && yy < BoardHeight &&
                                    board[xx, yy] != null && board[xx, yy].FieldType == FieldTypeEnum.Bomb)
                                {
                                    localBombCount++;
                                }
                            }
                        }
                        if (localBombCount > 0)
                        {
                            board[x, y] = new Field(FieldTypeEnum.BombCount, localBombCount);
                        }
                        else
                        {
                            board[x, y] = new Field(FieldTypeEnum.Empty);
                        }
                    }
                }
            }
        }

        internal Field GetFiled(Point p)
        {
            return board[p.X, p.Y];
        }
        //odkrywanie pól podczas gry
        internal void Uncover(Point p)
        {
            Field f = board[p.X, p.Y];
            if (f.Covered)
            {
                if (f.FieldType == FieldTypeEnum.Bomb)
                {
                    state = GameState.Loss;
                    for (int x = 0; x < BoardWidth; x++)
                    {
                        for (int y = 0; y < BoardHeight; y++)
                        {
                            board[x, y].Covered = false;
                        }
                    }
                }
                else if (f.FieldType == FieldTypeEnum.BombCount)
                {
                    f.Covered = false;
                    fieldsToUncover--;
                }
                //else to pole puste (bez bomby i liczby)
                else
                {
                    f.Covered = false;
                    fieldsToUncover--;
                    //podobnie jak przy zliczaniu bomb
                    //"obchodzimy sąsiadów, żeby ich odkryć rekurencyjnie
                    for (int xx = p.X - 1; xx <= p.X + 1; xx++)
                    {
                        for (int yy = p.Y - 1; yy <= p.Y + 1; yy++)
                        {
                            if (xx >= 0 && xx < BoardWidth &&
                                yy >= 0 && yy < BoardHeight)
                            {
                                //wywołanie rekurencyjne
                                Uncover(new Point(xx, yy));
                            }
                        }
                    }
                }
                //kontrola czy jest jeszcze coś do odkrycia
                if (fieldsToUncover == 0)
                {
                    state = GameState.Win;
                }
            }
        }
    }
}
