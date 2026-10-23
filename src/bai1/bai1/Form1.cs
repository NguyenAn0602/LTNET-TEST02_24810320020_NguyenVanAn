using GymMemberManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bai1
{
    public partial class frmGymMemberManager : Form
    {
        private BindingList<MemberModel> members =
    new BindingList<MemberModel>();

        private BindingSource bindingSource =
            new BindingSource();
        public frmGymMemberManager()
        {
            InitializeComponent();

            SetupForm();
            SetupGrid();
            SetupBinding();
            SetupEvents();

            ShowMember(null);
        }
        private int nextMemberNumber = 1;

        private string GenerateMemberId()
        {
            return "HV" + nextMemberNumber.ToString("D3");
        }
        private void lblMemberId_Click(object sender, EventArgs e)
        {

        }
        private void SetupForm()
        {
            txtMemberId.ReadOnly = true;

            cboPackage.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboPackage.Items.Clear();
            cboPackage.Items.Add("Theo tháng");
            cboPackage.Items.Add("6 tháng");
            cboPackage.Items.Add("1 năm");

            errorProvider1.BlinkStyle =
                ErrorBlinkStyle.NeverBlink;
        }
        private void SetupGrid()
        {
            dgvMembers.AutoGenerateColumns = false;
            dgvMembers.Columns.Clear();

            dgvMembers.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvMembers.MultiSelect = false;
            dgvMembers.ReadOnly = true;
            dgvMembers.AllowUserToAddRows = false;
            dgvMembers.RowHeadersVisible = false;

            dgvMembers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMemberId",
                HeaderText = "Mã HV",
                DataPropertyName = "MemberId"
            });

            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFullName",
                HeaderText = "Tên hội viên",
                DataPropertyName = "FullName"
            });

            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPhone",
                HeaderText = "SĐT",
                DataPropertyName = "Phone"
            });

            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPackage",
                HeaderText = "Gói tập",
                DataPropertyName = "Package"
            });

            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStartDate",
                HeaderText = "Ngày ĐK",
                DataPropertyName = "StartDate",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy"
                }
            });

            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFee",
                HeaderText = "Phí dịch vụ",
                DataPropertyName = "Fee",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N0"
                }
            });
        }
        private void SetupBinding()
        {
            bindingSource.DataSource = members;
            dgvMembers.DataSource = bindingSource;
        }

        private void ShowMember(MemberModel member)
        {
            if (member == null)
            {
                txtMemberId.Clear();
                txtFullName.Clear();
                txtPhone.Clear();
                cboPackage.SelectedIndex = -1;
                dtpStartDate.Value = DateTime.Today;
                txtFee.Clear();
                return;
            }

            txtMemberId.Text = member.MemberId;
            txtFullName.Text = member.FullName;
            txtPhone.Text = member.Phone;
            cboPackage.SelectedItem = member.Package;
            dtpStartDate.Value = member.StartDate;
            txtFee.Text = member.Fee.ToString(
                CultureInfo.InvariantCulture);
        }

        private void dgvMembers_SelectionChanged(
            object sender, EventArgs e)
        {
            MemberModel member =
                bindingSource.Current as MemberModel;

            ShowMember(member);
        }
        private void SetupEvents()
        {
            dgvMembers.SelectionChanged +=
                dgvMembers_SelectionChanged;

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
        }

        private bool ValidateFullName()
        {
            string name = txtFullName.Text.Trim();

            bool valid =
                !string.IsNullOrWhiteSpace(name) &&
                Regex.IsMatch(
                    name,
                    @"^[\p{L}\p{M}]+(?:[ '\-][\p{L}\p{M}]+)*$");

            errorProvider1.SetError(
                txtFullName,
                valid ? "" :
                "Tên không được trống hoặc chứa ký tự đặc biệt.");

            return valid;
        }
        private bool ValidatePhone()
        {
            string phone = txtPhone.Text.Trim();

            bool valid = Regex.IsMatch(phone, @"^\d{10}$");

            errorProvider1.SetError(
                txtPhone,
                valid ? "" :
                "Số điện thoại phải gồm đúng 10 chữ số.");

            return valid;
        }

        private bool ValidateFee()
        {
            decimal fee;

            bool valid = decimal.TryParse(
                txtFee.Text.Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out fee)
                && fee > 0;

            errorProvider1.SetError(
                txtFee,
                valid ? "" :
                "Phí dịch vụ phải là số nguyên dương.");

            return valid;
        }
        private bool ValidateInput()
        {
            bool nameValid = ValidateFullName();
            bool phoneValid = ValidatePhone();
            bool feeValid = ValidateFee();

            bool packageValid = cboPackage.SelectedIndex >= 0;

            errorProvider1.SetError(
                cboPackage,
                packageValid ? "" : "Vui lòng chọn gói tập.");

            return nameValid && phoneValid
                && feeValid && packageValid;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show(
                    "Vui lòng kiểm tra lại thông tin.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MemberModel member = new MemberModel();

            member.MemberId = GenerateMemberId();
            member.FullName = txtFullName.Text.Trim();
            member.Phone = txtPhone.Text.Trim();
            member.Package = cboPackage.SelectedItem.ToString();
            member.StartDate = dtpStartDate.Value.Date;
            member.Fee = decimal.Parse(
                txtFee.Text.Trim(),
                CultureInfo.InvariantCulture);

            members.Add(member);
            nextMemberNumber++;

            bindingSource.Position = members.Count - 1;
            ShowMember(member);

            MessageBox.Show(
                "Thêm hội viên thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            MemberModel member =
                bindingSource.Current as MemberModel;

            if (member == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn hội viên cần sửa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput())
            {
                MessageBox.Show(
                    "Dữ liệu chưa hợp lệ. Vui lòng kiểm tra lại.",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            member.FullName = txtFullName.Text.Trim();
            member.Phone = txtPhone.Text.Trim();
            member.Package = cboPackage.SelectedItem.ToString();
            member.StartDate = dtpStartDate.Value.Date;
            member.Fee = decimal.Parse(
                txtFee.Text.Trim(),
                CultureInfo.InvariantCulture);

            bindingSource.ResetCurrentItem();

            ShowMember(member);

            MessageBox.Show(
                "Cập nhật hội viên thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MemberModel member =
                bindingSource.Current as MemberModel;

            if (member == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn hội viên cần xóa.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa hội viên "
                + member.FullName + " (" + member.MemberId + ")?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                members.Remove(member);

                if (members.Count > 0)
                {
                    bindingSource.Position = Math.Min(
                        bindingSource.Position,
                        members.Count - 1);
                }
                else
                {
                    bindingSource.Position = -1;
                    ShowMember(null);
                }
            }
        }
        private void ClearControls()
        {
            txtMemberId.Clear();
            txtFullName.Clear();
            txtPhone.Clear();
            txtFee.Clear();

            cboPackage.SelectedIndex = -1;
            dtpStartDate.Value = DateTime.Today;

            errorProvider1.Clear();
        }
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            dgvMembers.ClearSelection();

            bindingSource.Position = -1;

            ClearControls();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvMembers.ClearSelection();

            bindingSource.Position = -1;

            ClearControls();
        }




    }
}
