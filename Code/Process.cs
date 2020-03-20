using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using XmlForm.Entities;
using System.Xml;
namespace XmlForm
{
    public class Process
    {
        private XmlElement root;
        private XmlDocument doc = new XmlDocument();
        private string fileName = @"C:\Users\votra\source\repos\XmlForm\XmlSV.xml";
        public Process()
        {
            doc.Load(fileName);
            root = doc.DocumentElement;
        }
        public void LoadID(string connectionString,ComboBox comboBox)
        {
            List<string> listid = new List<string>();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            connection.Open();
            cmd.CommandText = @"select StudentId from STUDENT";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection); //thuc hien cau lenh
            while(dbReader.Read())
            {
                listid.Add(Convert.ToString(dbReader["StudentId"]));
            }
            comboBox.DataSource = listid;
            dbReader.Close();
            connection.Close();
        }
        public Student GetById(string connectionString, string id)
        {
            Student data = new Student();
            int idget = Convert.ToInt32(id);
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            connection.Open();
            cmd.CommandText = @"select NameStudent,Phone,Email,Address,Majors from STUDENT where StudentId=@StudentId";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@StudentId", idget);
            SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            if(dbReader.Read())
            {
                data.NameStudent = Convert.ToString(dbReader["NameStudent"]);
                data.Phone = Convert.ToString(dbReader["Phone"]);
                data.Email = Convert.ToString(dbReader["Email"]);
                data.Address = Convert.ToString(dbReader["Address"]);
                data.Majors = Convert.ToString(dbReader["Majors"]);
            }
            connection.Close();
            return data;
        }
        public int LoadFile( string connectionString, DataGridView dgv)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            connection.Open();
            XmlDocument doc = new XmlDocument();
            dgv.Rows.Clear();
            dgv.ColumnCount = 5;
            int row = 0, rowAffected = 0;
            XmlNodeList list = root.SelectNodes("/lophoc/student");
            foreach (XmlNode item in list)
            {
                dgv.Rows.Add();
                dgv.Rows[row].Cells[0].Value = item.SelectSingleNode("name").InnerText;
                dgv.Rows[row].Cells[1].Value = item.SelectSingleNode("phone").InnerText;
                dgv.Rows[row].Cells[2].Value = item.SelectSingleNode("email").InnerText;
                dgv.Rows[row].Cells[3].Value = item.SelectSingleNode("address").InnerText;
                dgv.Rows[row].Cells[4].Value = item.SelectSingleNode("majors").InnerText;
                row++;
                cmd.CommandText = @"insert into STUDENT 
                                      values (@NameStudent,@Phone,@Email,@Address,@Majors)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.AddWithValue("@NameStudent", item.SelectSingleNode("name").InnerText);
                cmd.Parameters.AddWithValue("@Phone", item.SelectSingleNode("phone").InnerText);
                cmd.Parameters.AddWithValue("@Email", item.SelectSingleNode("email").InnerText);
                cmd.Parameters.AddWithValue("@Address", item.SelectSingleNode("address").InnerText);
                cmd.Parameters.AddWithValue("@Majors", item.SelectSingleNode("majors").InnerText);
                rowAffected =cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }  
            connection.Close();
            return row;
        }
        public void Add(string connectionString,Student Student,int Row, DataGridView dgv)
        {
            int rowAffected = 0;
            //add to database
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            connection.Open();
            cmd.CommandText = @"insert into STUDENT 
                                values (@NameStudent,@Phone,@Email,@Address,@Majors)";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@NameStudent", Student.NameStudent);
            cmd.Parameters.AddWithValue("@Phone", Student.Phone);
            cmd.Parameters.AddWithValue("@Email", Student.Email);
            cmd.Parameters.AddWithValue("@Address",Student.Address);
            cmd.Parameters.AddWithValue("@Majors", Student.Majors);
            rowAffected = cmd.ExecuteNonQuery();
            //add grid
            dgv.Rows.Add();
            dgv.Rows[Row].Cells[0].Value = Student.NameStudent;
            dgv.Rows[Row].Cells[1].Value = Student.Phone;
            dgv.Rows[Row].Cells[2].Value = Student.Email;
            dgv.Rows[Row].Cells[3].Value = Student.Address;
            dgv.Rows[Row].Cells[4].Value = Student.Majors;
            //add to xmlfile
            XmlNode student = doc.CreateElement("student");

            XmlElement name = doc.CreateElement("name");
            name.InnerText = Student.NameStudent;

            XmlElement phone = doc.CreateElement("phone");
            phone.InnerText = Student.Phone;

            XmlElement email = doc.CreateElement("email");
            email.InnerText = Student.Email;

            XmlElement address = doc.CreateElement("address");
            address.InnerText = Student.Address;

            XmlElement majors = doc.CreateElement("majors");
            majors.InnerText = Student.Majors;

            student.AppendChild(name);
            student.AppendChild(phone);
            student.AppendChild(email);
            student.AppendChild(address);
            student.AppendChild(majors);
            root.AppendChild(student);
            doc.Save(fileName);
            connection.Close();
        }
        public int Delete(string connectionString,string iddel,string txtMail)
        {
            int ketqua = 0;
            int idxoa = Convert.ToInt32(iddel);
            int rowAffected = 0;
            //delete to database
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            connection.Open();
            cmd.CommandText = @"Delete from STUDENT where StudentId =@StudentId";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@StudentId",idxoa);
            rowAffected = cmd.ExecuteNonQuery();
            //delete Xmlfile
            XmlNode studentxoa = doc.SelectSingleNode("//student[email='"+txtMail+"'] ");
            if (studentxoa != null)
            {
                root.RemoveChild(studentxoa);
                ketqua = 1;
                doc.Save(fileName);
            }
            return ketqua;
        }
        public int Update(string connectionString, Student data)
        {
            int ketqua = 0;
            // update database
            int rowAffected = 0;
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = @"update STUDENT 
                                    set NameStudent=@NameStudent,
                                        Phone=@Phone,
                                        Email=@Email,
                                        Address=@Address,
                                        Majors=@Majors
                                    where StudentId=@StudentID";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            connection.Open();
            cmd.Parameters.AddWithValue("@StudentId", data.StudentId);   //truyen tham so cho cau lenh SQL
            cmd.Parameters.AddWithValue("@NameStudent", data.NameStudent);
            cmd.Parameters.AddWithValue("@Phone", data.Phone);
            cmd.Parameters.AddWithValue("@Email", data.Email);
            cmd.Parameters.AddWithValue("@Address", data.Address);
            cmd.Parameters.AddWithValue("@Majors", data.Majors);
            rowAffected = cmd.ExecuteNonQuery();
            // update xml
            XmlNode OldNode = root.SelectSingleNode("//student[email ='" + data.Email + "']");
            if(OldNode != null)
            {
                OldNode.SelectSingleNode("name").InnerText = data.NameStudent;
                OldNode.SelectSingleNode("phone").InnerText = data.Phone;
                OldNode.SelectSingleNode("email").InnerText = data.Email;
                OldNode.SelectSingleNode("address").InnerText = data.Address;
                OldNode.SelectSingleNode("majors").InnerText = data.Majors;
                doc.Save(fileName);
                ketqua = 1;
            }
            connection.Close();
            return ketqua;
        }
    }
}
