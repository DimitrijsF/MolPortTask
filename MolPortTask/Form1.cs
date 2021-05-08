using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MolPortTask
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<string> tmp = new List<string>();

        private void FillAuthorSource()
        {
            Worker work = new Worker();
           comboBox1.DataSource = work.GetAuthorSource();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = DateTime.Now.Year;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "id";
            FillAuthorSource();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //search
            Worker work = new Worker();
            if (tableLayoutPanel2.Controls.ContainsKey("grid"))
                tableLayoutPanel2.Controls.RemoveByKey("grid");
            DataTable data = work.DoSearch(textBox1.Text);
            if (data.Rows.Count > 0)
            {
                tableLayoutPanel2.Controls.Add(new DataGridView()
                {
                    Name = "grid",
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                    DataSource = data,
                    Dock = DockStyle.Fill,
                    AllowUserToAddRows = false,
                    RowHeadersVisible = false
                }, 0, 1);
            }
            else
                MessageBox.Show("No entries found!");
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell != null)
            {
                tmp.Remove(Convert.ToString(dataGridView1["name", dataGridView1.CurrentRow.Index].Value));
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //add author to  list
            if (tmp.Contains(comboBox1.Text))
                MessageBox.Show("Already added!");
            else
            {
                dataGridView1.AllowUserToAddRows = true;
                dataGridView1.Rows.Add();
                dataGridView1["id", dataGridView1.Rows.Count - 2].Value = comboBox1.SelectedValue;
                dataGridView1["name", dataGridView1.Rows.Count - 2].Value = comboBox1.Text;
                dataGridView1.AllowUserToAddRows = false;
                tmp.Add(comboBox1.Text);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //show popup for new author adding
            if (!Controls.ContainsKey("newAuthor"))
            {
                GroupBox box = new GroupBox() { Size = new Size(150, 100), Location = new Point(Width / 2 - 75, Height / 2 - 50), Parent = this, Name = "newAuthor", Text = "Add new author" };
                TextBox txt = new TextBox() { Location = new Point(10, 30), Width = 130 };
                box.Controls.Add(txt);
                Button btn = new Button()
                {
                    Text = "Add",
                    Width = 60,
                    Height = 35,
                    Location = new Point(5, 60),
                };
                btn.Click += (s, ev) =>
                {
                    if (txt.Text == "")
                        MessageBox.Show("Author can not be empty!");
                    else if(tmp.Contains(txt.Text))
                        MessageBox.Show("Already added!");
                    else
                    {
                        Worker work = new Worker();
                        long id = work.AddAuthor(txt.Text);
                        tmp.Add(txt.Text);
                        dataGridView1.AllowUserToAddRows = true;
                        dataGridView1.Rows.Add();
                        dataGridView1["id", dataGridView1.Rows.Count - 2].Value = id;
                        dataGridView1["name", dataGridView1.Rows.Count - 2].Value = txt.Text;
                        dataGridView1.AllowUserToAddRows = false;
                        FillAuthorSource();
                        Controls.RemoveByKey("newAuthor");
                    }
                };
                box.Controls.Add(btn);
                Button btn1 = new Button() {
                    Text = "Close",
                    Width = 60,
                    Height = 35,
                    Location = new Point(85, 60),
                };
                btn1.Click += (s, ev) =>
                {
                    Controls.RemoveByKey("newAuthor");
                };
                box.Controls.Add(btn1);
                box.BringToFront();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //add new book
            Worker work = new Worker();
            int[] auids = new int[dataGridView1.Rows.Count];
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                auids[i] = Convert.ToInt32(dataGridView1["id", i].Value);
            long bid = work.AddBook(textBox2.Text, textBox3.Text, Convert.ToInt32(numericUpDown1.Value), auids);
            if (bid > -1)
            {
                textBox2.Text = "";
                textBox3.Text = "";
                numericUpDown1.Value = DateTime.Now.Year;
                dataGridView1.Rows.Clear();
                MessageBox.Show("Saved!");
            }
        }
    }
}
