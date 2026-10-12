using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BaiTapThucHanh4
{
    public class Bai4_7Form : Form
    {
        private MenuStrip menuStrip = null!;
        private ToolStrip toolStrip = null!;
        private StatusStrip statusStrip = null!;
        private ToolStripStatusLabel lblUserStatus = null!;
        private ToolStripStatusLabel lblActionStatus = null!;
        private ToolStripStatusLabel lblTimeStatus = null!;
        private System.Windows.Forms.Timer systemTimer = null!;

        public Bai4_7Form()
        {
            SetupMdiContainer();
            SetupMenuAndToolbars();
            SetupTimer();
        }

        private void SetupMdiContainer()
        {
            this.IsMdiContainer = true;
            this.Text = "BÀI 4.7: KHUNG ỨNG DỤNG MDI & QUẢN LÝ ĐA CỬA SỔ";
            this.Size = new Size(1150, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);
        }

        private void SetupMenuAndToolbars()
        {
            // 1. MenuStrip
            menuStrip = new MenuStrip();

            // Menu Hệ thống
            ToolStripMenuItem mnuSystem = new ToolStripMenuItem("Hệ thống (&S)");
            ToolStripMenuItem mnuLogout = new ToolStripMenuItem("Đăng xuất", null, (s, e) => MessageBox.Show("Đã đăng xuất phiên làm việc!", "Thông báo"));
            ToolStripMenuItem mnuExit = new ToolStripMenuItem("Thoát", null, (s, e) => Application.Exit());
            mnuSystem.DropDownItems.AddRange(new ToolStripItem[] { mnuLogout, new ToolStripSeparator(), mnuExit });

            // Menu Danh mục
            ToolStripMenuItem mnuCatalog = new ToolStripMenuItem("Danh mục (&M)");
            ToolStripMenuItem mnuStudents = new ToolStripMenuItem("Quản lý Sinh viên (Bài 4.6)", null, (s, e) => OpenChildForm<Bai4_6Form>());
            ToolStripMenuItem mnuAsync = new ToolStripMenuItem("Tiến trình Async (Bài 4.8)", null, (s, e) => OpenChildForm<Bai4_8Form>());
            mnuCatalog.DropDownItems.AddRange(new ToolStripItem[] { mnuStudents, mnuAsync });

            // Menu Cửa sổ (Window)
            ToolStripMenuItem mnuWindow = new ToolStripMenuItem("Cửa sổ (&W)");
            ToolStripMenuItem mnuCascade = new ToolStripMenuItem("Sắp xếp Cascade", null, (s, e) => this.LayoutMdi(MdiLayout.Cascade));
            ToolStripMenuItem mnuTileH = new ToolStripMenuItem("Tile Ngang", null, (s, e) => this.LayoutMdi(MdiLayout.TileHorizontal));
            ToolStripMenuItem mnuTileV = new ToolStripMenuItem("Tile Dọc", null, (s, e) => this.LayoutMdi(MdiLayout.TileVertical));
            mnuWindow.DropDownItems.AddRange(new ToolStripItem[] { mnuCascade, mnuTileH, mnuTileV });

            menuStrip.Items.AddRange(new ToolStripItem[] { mnuSystem, mnuCatalog, mnuWindow });
            this.MainMenuStrip = menuStrip;

            // 2. ToolStrip
            toolStrip = new ToolStrip();
            ToolStripButton btnToolStudent = new ToolStripButton("Sinh Viên", null, (s, e) => OpenChildForm<Bai4_6Form>());
            ToolStripButton btnToolAsync = new ToolStripButton("Tiến Trình Async", null, (s, e) => OpenChildForm<Bai4_8Form>());
            toolStrip.Items.AddRange(new ToolStripItem[] { btnToolStudent, new ToolStripSeparator(), btnToolAsync });

            // 3. StatusStrip
            statusStrip = new StatusStrip();
            lblUserStatus = new ToolStripStatusLabel("Người dùng: Quản trị viên (Admin)") { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblActionStatus = new ToolStripStatusLabel("Sẵn sàng") { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            lblTimeStatus = new ToolStripStatusLabel() { BorderSides = ToolStripStatusLabelBorderSides.Left };

            statusStrip.Items.AddRange(new ToolStripItem[] { lblUserStatus, lblActionStatus, lblTimeStatus });

            this.Controls.Add(toolStrip);
            this.Controls.Add(menuStrip);
            this.Controls.Add(statusStrip);
        }

        private void SetupTimer()
        {
            systemTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            systemTimer.Tick += (s, e) =>
            {
                lblTimeStatus.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            };
            systemTimer.Start();
        }

        // Kiểm tra mở Form con: nếu mở rồi thì Activate, chưa mở thì tạo mới
        public void OpenChildForm<T>() where T : Form, new()
        {
            var openForm = this.MdiChildren.OfType<T>().FirstOrDefault();

            if (openForm != null)
            {
                openForm.Activate();
                UpdateActionMessage($"Kích hoạt lại cửa sổ: {openForm.Text}");
            }
            else
            {
                T newForm = new T();
                newForm.MdiParent = this;

                // Giao tiếp MDI Child -> StatusStrip MDI Parent
                if (newForm is Bai4_8Form asyncForm)
                {
                    asyncForm.OnStatusProgressUpdated = (msg) => UpdateActionMessage(msg);
                }

                newForm.Show();
                UpdateActionMessage($"Đã mở cửa sổ: {newForm.Text}");
            }
        }

        public void UpdateActionMessage(string message)
        {
            lblActionStatus.Text = message;
        }
    }
}