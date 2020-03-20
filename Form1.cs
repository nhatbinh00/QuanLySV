using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using XmlForm.Entities;
namespace XmlForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private int Row = 0;
        private Process Process= new Process();
        private Student Student = new Student();
        private string connectionString= @"server=DESKTOP-6GHLID0\SQLEXPRESS;user id=sa;password=123;database=LiteXMLStudent";
        // get infor by id
        private void comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            string id = comboBox.SelectedItem.ToString();
            Student data = Process.GetById(connectionString, id);
            txtName.Text = data.NameStudent;
            txtPhone.Text = data.Phone;
            txtEmail.Text = data.Email;
            txtAddress.Text = data.Address;
            txtMajor.Text = data.Majors;
        }
        // get infor by ClickCell
        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow dgvRow = dgv.Rows[e.RowIndex];
            txtName.Text = dgvRow.Cells[0].Value.ToString();
            txtPhone.Text = dgvRow.Cells[1].Value.ToString();
            txtEmail.Text = dgvRow.Cells[2].Value.ToString();
            txtAddress.Text = dgvRow.Cells[3].Value.ToString();
            txtMajor.Text = dgvRow.Cells[4].Value.ToString();
        }
        //load file
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
           Row= Process.LoadFile(connectionString,dgv);
            Process.LoadID(connectionString, comboBox);
            MessageBox.Show("Total "+Row.ToString());
        }
        // add new student
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Student.NameStudent = txtName.Text;
            Student.Phone = txtPhone.Text;
            Student.Email = txtEmail.Text;
            Student.Address = txtAddress.Text;
            Student.Majors = txtMajor.Text;
            Process.Add(connectionString, Student,Row,dgv);
            Process.LoadID(connectionString, comboBox);
            Row = Row + 1;
        }
        // delete a student 
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int kq;
            string iddel = comboBox.SelectedItem.ToString();
            string txtMail = txtEmail.Text;
            kq = Process.Delete(connectionString, iddel, txtMail);
            if (kq == 0)
            {
                MessageBox.Show("Xóa chưa thành công !");
            }
            if (kq == 1)
            {
                MessageBox.Show("Xóa thành công !");
            }
            Process.LoadID(connectionString, comboBox);
            //foreach (DataGridViewRow item in this.dgv.Rows)
            //{
            //    if (item.Cells[2].Value.ToString().Trim() == txtEmail.Text)
            //    {
            //        dgv.Rows.RemoveAt(item.Index);
            //    }
            //}
            Row = Row - 1;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int kq;
            string id = comboBox.SelectedItem.ToString();
            Student.StudentId = Convert.ToInt32(id);
            Student.NameStudent = txtName.Text;
            Student.Phone = txtPhone.Text;
            Student.Email = txtEmail.Text;
            Student.Address = txtAddress.Text;
            Student.Majors = txtMajor.Text;
           kq= Process.Update(connectionString,Student);
            if (kq == 0)
            {
                MessageBox.Show("Cập nhật chưa thành công !");
            }
            if (kq == 1)
            {
                MessageBox.Show("Cập nhật thành công !");
            }
            //foreach (DataGridViewRow item in this.dgv.Rows)
            //{
            //    if (item.Cells[2].Value.ToString().Trim() == txtEmail.Text)
            //    {
            //        item.Cells[0].Value = txtName.Text;
            //        item.Cells[1].Value = txtPhone.Text;
            //        item.Cells[2].Value = txtEmail.Text;
            //        item.Cells[3].Value = txtAddress.Text;
            //        item.Cells[0].Value = txtMajor.Text;
            //    }
            //}
        }
    }
}
