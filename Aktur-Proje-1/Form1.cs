using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aktur_Proje_1
{
    public partial class Form1 : Form
    {
        // MSSQL Server veritabanı bağlantı dizesi
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        private int selectedRowIndex = -1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnUpdate.Click += btnUpdate_Click;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            // TextBox'lardan girilen değerleri al
            string ad = txtAd.Text;
            string soyad = txtSoyad.Text;
            string departman = txtDepartman.Text;

            // Veritabanına bağlan
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Ekleme sorgusu
                    string query = "INSERT INTO Kullanicilar (Ad, Soyad, Departman) VALUES (@Ad, @Soyad, @Departman)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parametreleri ata
                        command.Parameters.AddWithValue("@Ad", ad);
                        command.Parameters.AddWithValue("@Soyad", soyad);
                        command.Parameters.AddWithValue("@Departman", departman);

                        // Sorguyu çalıştır
                        command.ExecuteNonQuery();

                        MessageBox.Show("Veritabanına kaydedildi.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            // Silme işlemi için TextBox'lardan girilen değerleri al
            string ad = txtAd.Text;
            string soyad = txtSoyad.Text;

            // Veritabanına bağlan
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Silme sorgusu
                    string query = "DELETE FROM Kullanicilar WHERE Ad = @Ad AND Soyad = @Soyad";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parametreleri ata
                        command.Parameters.AddWithValue("@Ad", ad);
                        command.Parameters.AddWithValue("@Soyad", soyad);

                        // Sorguyu çalıştır
                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Kayıt silindi.");
                        }
                        else
                        {
                            MessageBox.Show("Kayıt bulunamadı.");
                            MessageBox.Show("Silmek için en az ad ve soyad girmelisiniz");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void btnTumBilgileriGetir_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Verileri getiren sorgu
                    string query = "SELECT Id, Ad, Soyad, Departman FROM Kullanicilar";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // DataGridView kontrolünü temizle
                            dataGridView1.Rows.Clear();

                            // Verileri oku ve DataGridView'e ekle
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string ad = reader.GetString(1);
                                string soyad = reader.GetString(2);
                                string departman = reader.GetString(3);

                                // Hücrelere verileri ekle
                                dataGridView1.Rows.Add(id, ad, soyad, departman);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Seçili satırın indeksini al
            int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;

            // Güncellenecek değerleri al
            string ad = txtAd.Text;
            string soyad = txtSoyad.Text;
            string departman = txtDepartman.Text;

            // Veritabanına bağlan
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Güncelleme sorgusu
                    string query = "UPDATE Kullanicilar SET Ad = @Ad, Soyad = @Soyad, Departman = @Departman WHERE Id = @SelectedId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parametreleri ata
                        command.Parameters.AddWithValue("@Ad", ad);
                        command.Parameters.AddWithValue("@Soyad", soyad);
                        command.Parameters.AddWithValue("@Departman", departman);
                        string selectedId = dataGridView1.Rows[selectedRowIndex].Cells["Id"].Value.ToString();
                        command.Parameters.AddWithValue("@SelectedId", selectedId);

                        // Sorguyu çalıştır
                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Kayıt güncellendi.");

                            // DataGridView'de güncellenen satırın bilgilerini güncelle
                            dataGridView1.Rows[selectedRowIndex].Cells["Ad"].Value = ad;
                            dataGridView1.Rows[selectedRowIndex].Cells["Soyad"].Value = soyad;
                            dataGridView1.Rows[selectedRowIndex].Cells["Departman"].Value = departman;
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme işlemi başarısız.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Seçili satırın indeksini al
            int selectedRowIndex = e.RowIndex;

            // Seçili satırın verilerini TextBox'lara doldur
            if (selectedRowIndex >= 0 && selectedRowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
                txtAd.Text = selectedRow.Cells["Ad"].Value.ToString();
                txtSoyad.Text = selectedRow.Cells["Soyad"].Value.ToString();
                txtDepartman.Text = selectedRow.Cells["Departman"].Value.ToString();
            }
        }
    }
}
