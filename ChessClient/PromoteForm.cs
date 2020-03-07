using ChessClient.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessClient
{
    public partial class PromoteForm : Form
    {
        public GameForm Game;
        public PieceType Selected = PieceType.Pawn;
        public PromoteForm(GameForm form)
        {
            Game = form;
            InitializeComponent();
        }

        private void PromoteForm_Load(object sender, EventArgs e)
        {
            foreach(var thing in Controls)
            {
                if(thing is Button btn)
                {
                    var name = btn.Name.Substring("btn".Length);
                    var type = (PieceType)Enum.Parse(typeof(PieceType), name);
                    btn.Image = (Image)Program.GetResource($"{Game.Main.Self.Side.ToString()[0]}_{type}");
                    btn.Tag = type;
                    btn.Click += Btn_Click;
                }
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if(sender is Button btn && btn.Tag is PieceType t)
            {
                Selected = t;
                this.Close();
            }
        }
    }
}
