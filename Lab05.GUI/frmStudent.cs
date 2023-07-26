using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity.Migrations;
using Lab05.BUS;
using Lab05.DAL.Models;
using System.IO;
using Lab05.DAL.Models.Models;
using static System.Net.WebRequestMethods;
using System.Net.NetworkInformation;

namespace Lab05.GUI
{
    public partial class frmStudent : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public frmStudent()
        {
            InitializeComponent();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Hàm binding list dữ liệu khoa vào combobox có tên hiện thị là tên khoa, giá trị là Mã khoa         
        private void FillFalcultyCombobox(List<Faculty> listFacultys)         
        {  
            listFacultys.Insert(0, new Faculty());
            this.cmbFaculty.DataSource = listFacultys;  
            this.cmbFaculty.DisplayMember = "FacultyName";             
            this.cmbFaculty.ValueMember = "FacultyID";         
        }  
        //Hàm binding gridView từ list sinh viên                  
        private void BindGrid(List<Student> listStudent)       
        {             
            dgvStudent.Rows.Clear();             
            foreach (var item in listStudent)             
            {                 
                int index = dgvStudent.Rows.Add();                 
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;                 
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;                 
                if(item.Faculty!= null)                    
                    dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;                 
                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore + "";

                if (item.MajorID != null)
                    dgvStudent.Rows[index].Cells[4].Value = item.Major.Name + "";
                ShowAvatar(item.Avatar);
            }         
        }
        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = dgvStudent.CurrentCell.RowIndex;
            StudentService studentDAO = new StudentService();
            Student f =  studentDAO.FindById(dgvStudent.Rows[r].Cells[0].Value.ToString());
            if (f != null)
            {
                txtMaSV.Text = f.StudentID;
                txtHoTen.Text = f.FullName;
                if(f.FacultyID!= null)    
                   cmbFaculty.SelectedValue = f.FacultyID;
                txtDTB.Text = f.AverageScore.ToString();
                ShowAvatar(f.Avatar); 
            }
        }
        private void ShowAvatar(string ImageName)
        { 
            if(string.IsNullOrEmpty(ImageName))
            {
                picAvatar.Image = null;
            }
            else
            {
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images", ImageName);
                picAvatar.Image = Image.FromFile(imagePath);
                picAvatar.Refresh();
            } 
        }

        private void SetDefaultForm()
        {
            txtMaSV.ResetText();
            txtHoTen.ResetText();
            cmbFaculty.ResetText();
            txtDTB.ResetText();
        }

        Student GetStudent()
        {
            Student s = new Student();
            s.StudentID = txtMaSV.Text.Trim();
            s.FullName = txtHoTen.Text.Trim();
            if (cmbFaculty.Text != "")
                s.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());
            if (txtDTB.Text != "")
                s.AverageScore = double.Parse(txtDTB.Text);
            return s;
        }
        private void btnAddUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMaSV.Text == "" || txtHoTen.Text == "" || cmbFaculty.Text == "" || txtDTB.Text == "")
                    throw new Exception("Vui lòng nhập đầy đủ các thông tin!");
                StudentService studentService = new StudentService();
                studentService.InsertUpdate(GetStudent());
                Form1_Load(sender, e);
                SetDefaultForm();
                MessageBox.Show("Insert update thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
          StudentModel qlsv = new StudentModel();
            Student dbDelete = qlsv.Students.FirstOrDefault(p => p.StudentID == txtMaSV.Text);
            if (dbDelete != null)
            {
                qlsv.Students.Remove(dbDelete);
                qlsv.SaveChanges();
                Form1_Load(sender, e);
                MessageBox.Show("Xóa thành công!");
            }
            
           
        }

        /*private void thôngTinKhoaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromFaculty form = new FromFaculty();
            form.Activate();
            form.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmRegister frmRegister = new frmRegister();
            frmRegister.ShowDialog();

        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            string filename;
            OpenFileDialog ofd = new OpenFileDialog();  
            ofd.ShowDialog();
            ofd.Filter =" &quot; JPEG files| *.jpg | PNG files | *.png | GIF Files | *.gif | TIFF Files | *.tif | BMP Files | *.bmp & quot"; 
            filename = ofd.FileName;
            Image img = Image.FromFile(filename);
            picAvatar.Image = img;
        }
        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkUnregisterMajor.Checked) 
                listStudents = studentService.GetAllHasNoMajor();
            else
                 listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
