using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiTapThucHanh4
{
    public class Bai4_8Form : Form
    {
        private Button btnStartLoad = null!;
        private ProgressBar progressBar = null!;
        private Label lblPercentage = null!;
        private DataGridView dgvLoadedData = null!;

        // Delegate gửi cập nhật trạng thái lên StatusStrip của FormMain
        public Action<string>? OnStatusProgressUpdated;

        public Bai4_8Form()
        {
            SetupFormLayout();
        }

        private void SetupFormLayout()
        {
            this.Text = "BÀI 4.8: XỬ LÝ BẤT ĐỒNG BỘ (ASYNC/AWAIT & PROGRESSBAR)";
            this.Size = new Size(840, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);

            btnStartLoad = new Button
            {
                Text = "Nạp dữ liệu lớn (Async)",
                Location = new Point(20, 20),
                Size = new Size(200, 36),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            btnStartLoad.Click += async (s, e) => await StartLoadingDataAsync();

            progressBar = new ProgressBar
            {
                Location = new Point(235, 24),
                Size = new Size(460, 28),
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            lblPercentage = new Label
            {
                Text = "0%",
                Location = new Point(710, 28),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            dgvLoadedData = new DataGridView
            {
                Location = new Point(20, 75),
                Size = new Size(785, 400),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            this.Controls.AddRange(new Control[] { btnStartLoad, progressBar, lblPercentage, dgvLoadedData });
        }

        private async Task StartLoadingDataAsync()
        {
            btnStartLoad.Enabled = false;
            progressBar.Value = 0;
            lblPercentage.Text = "0%";
            dgvLoadedData.DataSource = null;

            var progressReporter = new Progress<int>(percent =>
            {
                progressBar.Value = percent;
                lblPercentage.Text = $"{percent}%";
                OnStatusProgressUpdated?.Invoke($"Đang nạp dữ liệu: {percent}%...");
            });

            try
            {
                List<Student> results = await LoadLargeDataSetAsync(progressReporter);

                dgvLoadedData.DataSource = results;
                OnStatusProgressUpdated?.Invoke($"Đã nạp hoàn tất {results.Count} dòng dữ liệu.");
                MessageBox.Show($"Nạp thành công {results.Count} bản ghi mà không đơ UI!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OnStatusProgressUpdated?.Invoke("Quá trình nạp dữ liệu thất bại.");
            }
            finally
            {
                btnStartLoad.Enabled = true;
            }
        }

        private Task<List<Student>> LoadLargeDataSetAsync(IProgress<int> progress)
        {
            return Task.Run(async () =>
            {
                List<Student> list = new List<Student>();
                int totalSteps = 20;

                for (int i = 1; i <= totalSteps; i++)
                {
                    await Task.Delay(120); // Giả lập độ trễ truy vấn hệ thống

                    list.Add(new Student
                    {
                        StudentId = $"DATA_{i:D3}",
                        FullName = $"Sinh viên Bất đồng bộ #{i}",
                        Age = 19 + (i % 4),
                        Gender = (i % 2 == 0) ? "Nam" : "Nữ",
                        AvatarPath = ""
                    });

                    int currentPercent = (int)((i / (float)totalSteps) * 100);
                    progress.Report(currentPercent);
                }

                return list;
            });
        }
    }
}