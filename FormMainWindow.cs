using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saper
{
    public partial class FormMainWindow : Form
    {
        private SapperLogic myGame;
        private const int fieldSize = 30; ///wielkość pojedynczego buttona w pikselach
        public FormMainWindow()
        {
            InitializeComponent();

            ///wywolanie metody tworzącej na start prostej gry
            easyToolStripMenuItem_Click(null, null);
        }

        private void easyToolStripMenuItem_Click(object p1, object p2)
        {
            myGame = new SapperLogic(8, 8, 10);
            generateView();
        }
        private void panel2_Paint(object p1, object p2)
        {

        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myGame = new SapperLogic(12, 10, 25);
            generateView();
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myGame = new SapperLogic(20, 15, 50);
            generateView();
        }

        ///tworzenie nowej planszy
        private void generateView()
        {
            panelButton.Controls.Clear(); ///kasowanie starych buttonów

            ///generowanie nowych buttonów
            for(int x = 0;x<myGame.BoardWidth;x++)
            {
                for(int y=0;y<myGame.BoardHeight;y++)
                {
                    Button b = new Button(); ///tworzy nowy button
                    b.Size = new Size(fieldSize, fieldSize);
                    b.Location = new Point(fieldSize * x, fieldSize * y);
                    b.Click += B_Click;
                    panelButton.Controls.Add(b);

                    //Tag jest typu object i można tam wstawić cokolwiek
                    //Trzeba jednak później pamiętać o kontroli i konwersji typów
                    //Każdy przycisk oznaczam przy pomocy jego logicznego położenia w odniesienu do planszy gry
                    b.Tag = new Point(x, y);
                }
            }
        }

        private void B_Click(object sender, EventArgs e) ///obsluga kliknięncia na battony pod czas gry
        {
            if(myGame.State == SapperLogic.GameState.InProgress)
            {
                if(sender is Button)
                {
                    Button b = sender as Button;
                    if(b.Tag is Point)
                    {
                        Point p = (Point)b.Tag;

                        myGame.Uncover(p);
                        refreshView();

                        if(myGame.State == SapperLogic.GameState.Win)
                        {
                            MessageBox.Show("Win");
                        }
                        else if (myGame.State == SapperLogic.GameState.Loss)
                        {
                            MessageBox.Show("Loss");
                        }
                    }
                }
            }
            
        }

        private void refreshView()
        {
            foreach(Button b in panelButton.Controls) ///przechodzimy po wszystkich buttonach tworzanych w grę
            {
                ///pobieram z gry pole powiązane z danym buttonem
                SapperLogic.Field f = myGame.GetFiled((Point)b.Tag);

                if(f.Covered == false) ///pole jest otkryte
                {
                    if(f.FieldType == SapperLogic.FieldTypeEnum.Bomb) ///jeżeli bomba
                    {
                        b.BackColor = Color.Red;
                        b.Text = "@";
                    }
                    else ///pole puste lub pole z cyframi
                    {
                        b.BackColor = Color.White; ///dla obydwo rodzajów pól
                        
                        if(f.FieldType == SapperLogic.FieldTypeEnum.BombCount) ///tylko dla pól z cyframi
                        {
                            b.Text = f.BombCount.ToString();
                        }
                    }

                }
            }
        }
    }
}
