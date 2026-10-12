using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BaiTapThucHanh4
{
    public class Student : INotifyPropertyChanged
    {
        private string studentId = string.Empty;
        private string fullName = string.Empty;
        private int age;
        private string gender = "Nam";
        private string avatarPath = string.Empty;

        public string StudentId
        {
            get => studentId;
            set { if (studentId != value) { studentId = value; OnPropertyChanged(nameof(StudentId)); } }
        }

        public string FullName
        {
            get => fullName;
            set { if (fullName != value) { fullName = value; OnPropertyChanged(nameof(FullName)); } }
        }

        public int Age
        {
            get => age;
            set { if (age != value) { age = value; OnPropertyChanged(nameof(Age)); } }
        }

        public string Gender
        {
            get => gender;
            set { if (gender != value) { gender = value; OnPropertyChanged(nameof(Gender)); } }
        }

        public string AvatarPath
        {
            get => avatarPath;
            set { if (avatarPath != value) { avatarPath = value; OnPropertyChanged(nameof(AvatarPath)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class Bai4_6Form : Form
    {
        private TextBox txtId = null!;
        private TextBox txtName = null!;
        private TextBox txtAge = null!;
        private ComboBox cboGender = null!;
        private PictureBox picAvatar = null!;
        private Button btnChooseAvatar = null!;
        private Button btnAdd = null!;
        private Button btnDelete = null!;
        private Button btnUpdate = null!;
        private Button btnReset = null!;
        private Button btnExportCsv = null!;
        private DataGridView dgvStudents = null!;

        private BindingSource bsStudents = new BindingSource();
        private BindingList<Student> studentList = new BindingList<Student>();

        public Action<string>? OnStatusMessageUpdated;

        public Bai4_6Form()
        {
            SetupFormLayout();
            SetupBindings();
            InitSampleData();
        }

        private void SetupFormLayout()
        {
            this.Text = "BÀI 4.6: QUẢN LÝ DỮ LIỆU VỚI BINDINGSOURCE & XỬ LÝ FILE (AVATAR / CSV)";
            this.Size = new Size(950, 640);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);

            GroupBox grpInput = new GroupBox
            {
                Text = "Thông tin chi tiết sinh viên",
                Location = new Point(15, 12),
                Size = new Size(630, 220)
            };

            Label lblId = new Label { Text = "Mã SV:", Location = new Point(20, 32), AutoSize = true };
            txtId = new TextBox { Location = new Point(100, 28), Width = 160 };

            Label lblName = new Label { Text = "Họ tên:", Location = new Point(20, 72), AutoSize = true };
            txtName = new TextBox { Location = new Point(100, 68), Width = 250 };

            Label lblAge = new Label { Text = "Tuổi:", Location = new Point(20, 112), AutoSize = true };
            txtAge = new TextBox { Location = new Point(100, 108), Width = 100 };

            Label lblGender = new Label { Text = "Giới tính:", Location = new Point(20, 152), AutoSize = true };
            cboGender = new ComboBox { Location = new Point(100, 148), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cboGender.Items.AddRange(new object[] { "Nam", "Nữ" });

            grpInput.Controls.AddRange(new Control[] { lblId, txtId, lblName, txtName, lblAge, txtAge, lblGender, cboGender });

            GroupBox grpAvatar = new GroupBox
            {
                Text = "Ảnh đại diện",
                Location = new Point(660, 12),
                Size = new Size(255, 220)
            };

            picAvatar = new PictureBox
            {
                Location = new Point(45, 25),
                Size = new Size(165, 145),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.WhiteSmoke
            };

            btnChooseAvatar = new Button
            {
                Text = "Chọn ảnh...",
                Location = new Point(45, 178),
                Size = new Size(165, 30)
            };
            btnChooseAvatar.Click += BtnChooseAvatar_Click;

            grpAvatar.Controls.AddRange(new Control[] { picAvatar, btnChooseAvatar });

            btnAdd = new Button { Text = "Thêm mới", Location = new Point(15, 245), Size = new Size(105, 34), BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnUpdate = new Button { Text = "Cập nhật", Location = new Point(130, 245), Size = new Size(105, 34) };
            btnDelete = new Button { Text = "Xóa dòng", Location = new Point(245, 245), Size = new Size(105, 34), BackColor = Color.FromArgb(217, 83, 79), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnReset = new Button { Text = "Làm mới", Location = new Point(360, 245), Size = new Size(105, 34) };
            btnExportCsv = new Button { Text = "Xuất file CSV", Location = new Point(475, 245), Size = new Size(120, 34) };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnReset.Click += BtnReset_Click;
            btnExportCsv.Click += BtnExportCsv_Click;

            dgvStudents = new DataGridView
            {
                Location = new Point(15, 290),
                Size = new Size(900, 295),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            ContextMenuStrip cms = new ContextMenuStrip();
            ToolStripMenuItem menuDel = new ToolStripMenuItem("Xóa dòng được chọn");
            ToolStripMenuItem menuRefresh = new ToolStripMenuItem("Làm mới danh sách");
            menuDel.Click += (s, e) => btnDelete.PerformClick();
            menuRefresh.Click += (s, e) => bsStudents.ResetBindings(false);
            cms.Items.AddRange(new ToolStripItem[] { menuDel, menuRefresh });
            dgvStudents.ContextMenuStrip = cms;

            this.Controls.AddRange(new Control[] { grpInput, grpAvatar, btnAdd, btnUpdate, btnDelete, btnReset, btnExportCsv, dgvStudents });
        }

        private void SetupBindings()
        {
            bsStudents.DataSource = studentList;
            dgvStudents.DataSource = bsStudents;

            txtId.DataBindings.Add("Text", bsStudents, nameof(Student.StudentId), true, DataSourceUpdateMode.OnPropertyChanged);
            txtName.DataBindings.Add("Text", bsStudents, nameof(Student.FullName), true, DataSourceUpdateMode.OnPropertyChanged);
            txtAge.DataBindings.Add("Text", bsStudents, nameof(Student.Age), true, DataSourceUpdateMode.OnPropertyChanged);
            cboGender.DataBindings.Add("SelectedItem", bsStudents, nameof(Student.Gender), true, DataSourceUpdateMode.OnPropertyChanged);

            bsStudents.CurrentChanged += (s, e) => LoadCurrentAvatar();
        }

        private void LoadCurrentAvatar()
        {
            if (bsStudents.Current is Student current && !string.IsNullOrEmpty(current.AvatarPath) && File.Exists(current.AvatarPath))
            {
                try { picAvatar.Image = Image.FromFile(current.AvatarPath); }
                catch { picAvatar.Image = null; }
            }
            else
            {
                picAvatar.Image = null;
            }
        }

        private void InitSampleData()
        {
            studentList.Add(new Student { StudentId = "SV01", FullName = "Phạm Xuân Tiến", Age = 21, Gender = "Nam", AvatarPath = "" });
            studentList.Add(new Student { StudentId = "SV02", FullName = "Nguyễn Văn Đức", Age = 21, Gender = "Nam", AvatarPath = "" });
            studentList.Add(new Student { StudentId = "SV03", FullName = "Trần Hoàng Long", Age = 22, Gender = "Nam", AvatarPath = "" });
        }

        private void BtnChooseAvatar_Click(object? sender, EventArgs e)
        {
            if (bsStudents.Current is Student current)
            {
                using OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Chọn ảnh đại diện cho sinh viên",
                    Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    current.AvatarPath = ofd.FileName;
                    LoadCurrentAvatar();
                    OnStatusMessageUpdated?.Invoke($"Đã chọn ảnh cho sinh viên: {current.FullName}");
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Mã sinh viên và Họ tên không được để trống!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtAge.Text.Trim(), out int age) || age <= 0 || age > 100)
            {
                MessageBox.Show("Tuổi phải là số nguyên hợp lệ (1 - 100)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var s in studentList)
            {
                if (s.StudentId.Trim().Equals(txtId.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Mã sinh viên này đã tồn tại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            Student newStudent = new Student
            {
                StudentId = txtId.Text.Trim(),
                FullName = txtName.Text.Trim(),
                Age = age,
                Gender = cboGender.SelectedItem?.ToString() ?? "Nam",
                AvatarPath = ""
            };

            studentList.Add(newStudent);
            bsStudents.MoveLast();
            OnStatusMessageUpdated?.Invoke($"Đã thêm sinh viên: {newStudent.StudentId}");
            MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (bsStudents.Current == null) return;

            if (string.IsNullOrWhiteSpace(txtId.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Mã sinh viên và Họ tên không được để trống!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtAge.Text.Trim(), out int age) || age <= 0 || age > 100)
            {
                MessageBox.Show("Tuổi phải là số nguyên hợp lệ (1 - 100)!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bsStudents.EndEdit();
            dgvStudents.Refresh();
            OnStatusMessageUpdated?.Invoke("Đã cập nhật thông tin sinh viên.");
            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (bsStudents.Current is Student current)
            {
                var dr = MessageBox.Show($"Xác nhận xóa sinh viên [{current.FullName}]?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    string name = current.FullName;
                    bsStudents.RemoveCurrent();
                    OnStatusMessageUpdated?.Invoke($"Đã xóa sinh viên: {name}");
                }
            }
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            txtId.Clear();
            txtName.Clear();
            txtAge.Clear();
            cboGender.SelectedIndex = 0;
            picAvatar.Image = null;
            txtId.Focus();
        }

        private void BtnExportCsv_Click(object? sender, EventArgs e)
        {
            using SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Lưu file CSV",
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = "DanhSachSinhVien.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Mã SV,Họ và Tên,Tuổi,Giới tính,Đường dẫn ảnh");
                    foreach (Student s in studentList)
                    {
                        sb.AppendLine($"\"{s.StudentId}\",\"{s.FullName}\",{s.Age},\"{s.Gender}\",\"{s.AvatarPath}\"");
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    OnStatusMessageUpdated?.Invoke($"Đã xuất file CSV: {Path.GetFileName(sfd.FileName)}");
                    MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi ghi file: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}