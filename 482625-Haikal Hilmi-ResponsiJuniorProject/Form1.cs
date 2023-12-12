using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace _482625_Haikal_Hilmi_ResponsiJuniorProject
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection conn;
        string connstring = "Host=localhost;Port=5432;Username=postgres;Password=informatika;Database=responsi";
        public DataTable dt;
        public static NpgsqlCommand cmd;
        private string sql = null;
        private DataGridViewRow r;
        public Form1()
        {
            InitializeComponent();
            conn = new NpgsqlConnection(connstring);
        }
        private string GetID(string departemen)
        {
            try
            {
                sql = "select * from departemen";
                cmd = new NpgsqlCommand(sql, conn);
                cbDepartement.Items.Clear();

                using (NpgsqlDataReader departemenCMD = cmd.ExecuteReader())
                {
                    while (departemenCMD.Read())
                    {
                        if(departemen == departemenCMD["nama_dep"].ToString())
                        {
                            return departemenCMD["id_dep"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "FAIL!!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            return "";
        }
        private string GetNama(string idDepartemen)
        {
            try
            {
                sql = "select * from departemen";
                cmd = new NpgsqlCommand(sql, conn);
                cbDepartement.Items.Clear();

                using (NpgsqlDataReader departemenCMD = cmd.ExecuteReader())
                {
                    while (departemenCMD.Read())
                    {
                        if (idDepartemen == departemenCMD["id_dep"].ToString())
                        {
                            return departemenCMD["nama_dep"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "FAIL!!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            return "";
        }
        private void LoadCheckbox(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                
                sql = "select * from departemen";
                cmd = new NpgsqlCommand(sql, conn);
                cbDepartement.Items.Clear();

                using (NpgsqlDataReader departemenCMD = cmd.ExecuteReader())
                {
                    List<string> departemenList = new List<string>();
                    while (departemenCMD.Read())
                    {
                        string district = departemenCMD["nama_dep"].ToString();
                        departemenList.Add(district);
                    }
                    cbDepartement.Items.Clear();
                    cbDepartement.Items.AddRange(departemenList.ToArray());
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "FAIL!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }
        private void LoadData(object sender, EventArgs e)
        {
            try
            {
                conn.Open();
                dgvData.DataSource = null;
                sql = "select * from karyawan;";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                NpgsqlDataReader rd = cmd.ExecuteReader();
                dt.Load(rd);
                dgvData.DataSource = dt;
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "FAIL!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
            }
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {

                conn.Open();
                string id_dep = GetID(cbDepartement.Text);
                sql = @"insert into karyawan (nama, id_dep) values (:_nama, :_id_dep)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_nama", txtNama.Text);
                cmd.Parameters.AddWithValue("_id_dep", id_dep);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data Karyawan Berhasil diinputkan", "Well Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Close();
                    LoadData(null, null);   
                    txtNama.Text = null;
                    cbDepartement.Text = null;
                }
                else
                {
                    MessageBox.Show("Data Karyawan Gagal diinputkan", "Sorry!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Insert FAIL!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();

            }
        }

        public void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                conn.Open();
                r = dgvData.Rows[e.RowIndex];
                txtNama.Text = r.Cells["nama"].Value.ToString();
                cbDepartement.Text = GetNama(r.Cells["id_dep"].Value.ToString());
                conn.Close();

            }
        }

        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Mohon pilih baris data yang akan diupdate", "Good", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                conn.Open();
                string id_dep = GetID(cbDepartement.Text);
                sql = @"Update karyawan SET nama=:_nama, id_dep=:_id_dep where id_karyawan=:_id_karyawan";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_nama", txtNama.Text);
                cmd.Parameters.AddWithValue("_id_dep", id_dep);
                cmd.Parameters.AddWithValue("_id_karyawan", r.Cells["id_karyawan"].Value);
                MessageBox.Show(sql, "Well Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Data Karyawan Berhasil diEdit", "Well Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conn.Close();
                    LoadData(null, null);
                    txtNama.Text = null;
                    cbDepartement.Text = null;
                }
                else
                {
                    MessageBox.Show("Data Karyawan Gagal diedit", "Sorry!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Edit FAIL!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();

            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (r == null)
            {
                MessageBox.Show("Mohon pilih baris data yang akan dihapus", "Warning!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string nama = r.Cells["nama"].Value.ToString();
            if (MessageBox.Show("Apakah benar Anda ingin menghapus data " + nama + "?", "Hapus Data Terkonfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                try
                {
                    conn.Open();
                    sql = @"DELETE from  karyawan where id_karyawan=:_id_karyawan";
                    cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("_id_karyawan", r.Cells["id_karyawan"].Value);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data "+ nama + " berhasil di hapus", "Well Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cbDepartement.Text = null;
                        txtNama.Text = null;
                        r = null;
                        conn.Close();

                        LoadData(null, null);

                    }
                        conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Delete FAIL!!! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    conn.Close();

                }
        }
    }
}
