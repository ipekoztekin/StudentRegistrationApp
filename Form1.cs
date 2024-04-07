using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace StudentRegistrationSystem
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server=DESKTOP-NFUGI3G\\SQLEXPRESS;Database=StudentRegistration;User Id=sa;Password=996633;";

        public Form1()
        {
            InitializeComponent();
        }

        private void listing_Click(object sender, EventArgs e)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Students";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                DataTable table = new DataTable();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
                dataGridView1.DataSource = table;
                connection.Close();
            }
        }

        private void enrollment_Click(object sender, EventArgs e)
        {
            // Girişlerin boş olup olmadığını kontrol edin
             if (string.IsNullOrWhiteSpace(name.Text) ||
                 string.IsNullOrWhiteSpace(surname.Text) ||
                 string.IsNullOrWhiteSpace(email.Text) ||
                 string.IsNullOrWhiteSpace(maskedTextBox1.Text))
             {
                 MessageBox.Show("Lütfen Öğrencimizin Tüm Bilgilerini Doldurunuz veya Hataları Düzeltiniz.", "Eksik veya Hatalı Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return; // Eğer herhangi bir alan boş ise işlemi durdur
             }

             // Öğrencinin var olup olmadığını kontrol etmek için sorguyu oluştur
             string checkQuery = "SELECT COUNT(*) FROM Students WHERE FirstName = @FirstName AND LastName = @LastName AND Email = @Email AND DateOfBirth = @DateOfBirth";

             // SqlConnection ve SqlCommand nesnelerini oluştur
             using (SqlConnection connection = new SqlConnection(connectionString))
             using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
             {
                 // Parametre ekleyerek güvenli bir sorgu yapın
                 checkCommand.Parameters.AddWithValue("@FirstName", name.Text);
                 checkCommand.Parameters.AddWithValue("@LastName", surname.Text);
                 checkCommand.Parameters.AddWithValue("@Email", email.Text);
                 checkCommand.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(maskedTextBox1.Text));

                 // Bağlantıyı açın ve sorguyu çalıştırın
                 connection.Open();
                 int count = (int)checkCommand.ExecuteScalar();

                 // Eğer öğrenci varsa uyarı ver ve işlemi durdur
                 if (count > 0)
                 {
                     MessageBox.Show("Bu öğrenci zaten sistemde kayıtlıdır.", "Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     return;
                 }
             }

             // Ekleme sorgusunu oluştur
             string query = "INSERT INTO Students (FirstName, LastName, Email, DateOfBirth, EnrolledDate) VALUES (@FirstName, @LastName, @Email, @DateOfBirth, @EnrolledDate)";

             // SqlConnection ve SqlCommand nesnelerini oluştur
             using (SqlConnection connection = new SqlConnection(connectionString))
             using (SqlCommand command = new SqlCommand(query, connection))
             {
                 // Parametre ekleyerek güvenli bir sorgu yapın
                 command.Parameters.AddWithValue("@FirstName", name.Text);
                 command.Parameters.AddWithValue("@LastName", surname.Text);
                 command.Parameters.AddWithValue("@Email", email.Text);
                 command.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(maskedTextBox1.Text));
                 command.Parameters.AddWithValue("@EnrolledDate", DateTime.Now); // Günün tarihini al

                 // Bağlantıyı açın ve sorguyu çalıştırın
                 connection.Open();
                 command.ExecuteNonQuery();
             }

             // Ekleme işlemi tamamlandıktan sonra DataGridView'i güncelle
             listing_Click(sender, e);

             MessageBox.Show("Öğrencimiz Sisteme Başarı İle Kayıt Edilmiştir.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
         
        }

    }
}
